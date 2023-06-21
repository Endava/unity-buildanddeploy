using Endava.BuildAndDeploy.Logging;
using Endava.BuildAndDeploy.UxmlBindings;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    [BuildModule(BuildExecutionMode.Setup, 0)]
    public class MainModule : BuildProcessModule
    {
        public override string SectionName => "Build Settings";
        public override bool CanBeDisabled => false;
        public override bool HideHelpIcon => true;
        public override string ModuleIconId => "_Popup@2x";

        [SerializeField] protected BuildTarget target = BuildTarget.NoTarget;
        public BuildTarget Target { get => target; set => target = value; }
        [SerializeField] protected bool cleanBuild = true;
        public bool CleanBuild => cleanBuild;
        [SerializeField] protected string path = null;
        public string DeploymentPath {
            get
            {
                // if the full path was overwritten, extract the folder name from it
                if (!string.IsNullOrEmpty(Process?.OverwrittenProperties?.FullOutputPath))
                {
                    var fullOutputPath = Process.OverwrittenProperties.FullOutputPath;
                    if (Path.HasExtension(fullOutputPath))
                        return FileUtilities.NormalizeWindowsToUnix(Path.GetDirectoryName(fullOutputPath));
                    else
                        return FileUtilities.NormalizeWindowsToUnix(fullOutputPath);
                }
                // if the deployment directory path is overwritten
                else if (!string.IsNullOrEmpty(Process?.OverwrittenProperties?.CloudDeploymentPath))
                {
                    return FileUtilities.NormalizeWindowsToUnix(Process.OverwrittenProperties.CloudDeploymentPath);
                }
                else
                {
                    return path;
                }
            }
            set => path = value; 
        }
        [SerializeField] protected string fileName = null;
        public string DeploymentFilename => fileName;
        [SerializeField] protected bool showDeploymentPathWarnings = true;
        public bool ShowDeploymentPathWarnings => showDeploymentPathWarnings;

        [SerializeField]
        private LogLevel logLevel = LogLevel.Information;
        public LogLevel LogLevel => Process.OverwrittenProperties?.LogLevel ?? logLevel;

        [SerializeField]
        private LogTargets logTargets = LogTargets.Unity;
        public LogTargets LogTargets => Process.OverwrittenProperties?.LogTargets ?? logTargets;

        private MainModuleBinding m_binding;

        public string FullAbsoluteOutputPath
        {
            get
            {
                if (!string.IsNullOrEmpty(Process.OverwrittenProperties?.FullOutputPath))
                {
                    return FileUtilities.NormalizeWindowsToUnix(Process.OverwrittenProperties?.FullOutputPath);
                }
                else if (!string.IsNullOrEmpty(Process.OverwrittenProperties?.CloudDeploymentPath))
                {
                    var path = FileUtilities.NormalizeWindowsToUnix(Process.OverwrittenProperties?.CloudDeploymentPath);
                    if (!path.EndsWith(FileUtilities.UnixSeparator)) path += FileUtilities.UnixSeparator;

                    var extension = !string.IsNullOrEmpty(Helpers.BuildTargetPlatformExtension(target)) 
                        ? $".{Helpers.BuildTargetPlatformExtension(target)}" 
                        : "";
                    
                    if (!string.IsNullOrEmpty(Process.OverwrittenProperties?.CloudExecutableName))
                        return $"{path}{Process.OverwrittenProperties.CloudExecutableName}{extension}";
                    else
                        return $"{path}{fileName}{extension}";
                }
                else
                {
                    var path = !string.IsNullOrEmpty(DeploymentPath) ? FileUtilities.NormalizeWindowsToUnix(Path.GetFullPath(DeploymentPath)) : ".";
                    if (!path.EndsWith(FileUtilities.UnixSeparator)) path += FileUtilities.UnixSeparator;

                    var extension = !string.IsNullOrEmpty(Helpers.BuildTargetPlatformExtension(target))
                        ? $".{Helpers.BuildTargetPlatformExtension(target)}"
                        : "";

                    return $"{path}{fileName}{extension}";
                }
            }
        }

        public MainModule()
        {
            // main module should be enabled and unfolded on construction
            IsEnabled = true;
            IsFolded = false;
        }

        public override Task<bool> Execute()
        {
            BuildLogger.LogDebug($"Override LogLevel: {Process.OverwrittenProperties?.LogLevel ?? LogLevel}");
            BuildLogger.LogDebug($"Override LogTargets: {Process.OverwrittenProperties?.LogTargets ?? LogTargets}");

            // assign the logging properties (use overwritten properties if specified)
            BuildLogger.LogLevel = LogLevel;
            BuildLogger.LogTargets = LogTargets;

            // cleanup build_log on each build
            if (LogTargets.HasFlag(LogTargets.File))
            {
                FileLogger.CleanLogFile();
            }

            // clear folder first, if requested
            if(CleanBuild && Directory.Exists(DeploymentPath))
            {
                Directory.Delete(DeploymentPath, true);
                BuildLogger.LogInformation($"Cleaned DeploymentPath {DeploymentPath}");
            }

            // make sure the deployment path exists
            if(!Directory.Exists(DeploymentPath))
            {
                Directory.CreateDirectory(DeploymentPath);
                BuildLogger.LogInformation($"Created DeploymentPath {DeploymentPath}");
            }

            return Task.FromResult(true);
        }

        public override BuildPlayerOptions OnChangeBuildPlayerOptions(BuildPlayerOptions input)
        {
            input.locationPathName = FullAbsoluteOutputPath;
            input.targetGroup = Helpers.GroupFromBuildTarget(Target);
            input.target = Target;

            if (Process.OverwrittenProperties?.UseAutoRun == true)
                input.options |= BuildOptions.AutoRunPlayer;
            else
                input.options &= ~BuildOptions.AutoRunPlayer;

            return input;
        }

        public override void OnCreate()
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = Application.productName;

            if (string.IsNullOrEmpty(path))
                path = "Builds/";
        }

        public override VisualElement CreateContentUI(SerializedProperty serializedProperty, BuildProcessUiList editorUiList, Editor editor)
        {
            var mainModuleUi = editorUiList.mainModuleContentUi.Instantiate();
            m_binding = new MainModuleBinding(mainModuleUi);
            var firstCreation = true;

            //mainModuleUiBinding.buildTargetField.value = target;
            m_binding.buildTargetField.BindProperty(serializedProperty.FindPropertyRelative(nameof(target)));
            m_binding.buildTargetField.RegisterValueChangedCallback(evt =>
            {
                // once the target changed, update the properties relying on this
                m_binding.filenameExtensionField.value = Helpers.BuildTargetPlatformExtension(target);
                UpdateFullPath();

                if (!firstCreation)
                    Process.Recreate();
                else
                    firstCreation = false;
            });

            m_binding.cleanBuildToggle.BindProperty(serializedProperty.FindPropertyRelative(nameof(cleanBuild)));
            m_binding.showWarningsToggle.BindProperty(serializedProperty.FindPropertyRelative(nameof(showDeploymentPathWarnings)));
            m_binding.outputPathField.BindProperty(serializedProperty.FindPropertyRelative(nameof(path)));
            m_binding.exportNameField.BindProperty(serializedProperty.FindPropertyRelative(nameof(fileName)));
            m_binding.exportNameField.RegisterValueChangedCallback(evt =>
            {
                UpdateFullPath();
            });

            m_binding.chooseDirectoryBtn.clicked += OnChooseDirectoryButtonClicked;
            m_binding.filenameExtensionField.value = Helpers.BuildTargetPlatformExtension(target);
            m_binding.fullPathField.value = FullAbsoluteOutputPath;
            m_binding.pathCopyBtn.clicked += OnCopyFullPathClicked;


            Helpers.AssignInBuildEditorUnityTextureToVisualElement(m_binding.pathCopyIcon, "TreeEditor.Duplicate");

            m_binding.showWarningsToggle.tooltip = "If disabled, all possible deployment path warnings (relative or absolute path issues) will be ignored. Toggle this off, if you know that your path is valid and need no additional validation!";
            m_binding.cleanBuildHelpIcon.tooltip = "If true, the output path will be deleted on build execution. This might cause longer build times, since caching is bypassed!";
            m_binding.fullPathHelpIcon.tooltip = "Contains the absolute path the project will be builded to.";
            m_binding.preferencesHelpIcon.style.display = DisplayStyle.None;
            m_binding.loggingHelpIcon.tooltip = @"This will allow you where the Build logging events are reported to. You can f.e. create an isolated logging file beside the usual Unity log.";
            m_binding.logFileHint.text = $"A Build Logger file will be written to <b>\"{FileUtilities.NormalizeWindowsToUnix(Path.GetFullPath(FileLogger.LogFilePath))}\"</b>";


            m_binding.logLevelField.Init(LogLevel);
            m_binding.logLevelField.BindProperty(serializedProperty.FindPropertyRelative(nameof(logLevel)));

            m_binding.logTargetsField.Init(LogTargets);
            m_binding.logTargetsField.BindProperty(serializedProperty.FindPropertyRelative(nameof(logTargets)));

            m_binding.logFileHint.style.display = LogTargets.HasFlag(LogTargets.File) ? DisplayStyle.Flex : DisplayStyle.None;

            m_binding.logTargetsField.RegisterValueChangedCallback(evt =>
            {
                if (evt.target == m_binding.logTargetsField)
                    m_binding.logFileHint.style.display = LogTargets.HasFlag(LogTargets.File) ? DisplayStyle.Flex : DisplayStyle.None;
            });

            return mainModuleUi;
        }

        private void OnCopyFullPathClicked()
        {
            EditorGUIUtility.systemCopyBuffer = FullAbsoluteOutputPath;
        }

        private void OnChooseDirectoryButtonClicked()
        {
            var defaultPath = path;

            if (string.IsNullOrEmpty(defaultPath))
                defaultPath = Application.dataPath.Replace("/Assets", "");

            var choosenFolder = EditorUtility.OpenFolderPanel("Select Output Path", defaultPath, "Builds");
            var newPath = FileUtilities.NormalizeWindowsToUnix(FileUtilities.ConvertPathRelativeToProject(choosenFolder)) ?? choosenFolder;

            // check if the folder selection was aborted
            path = !string.IsNullOrEmpty(newPath) ? newPath : path;
            UpdateFullPath();
        }

        public override BuildValidation Validate()
        {
            if (!Helpers.IsBuildTargetSupported(target))
                return BuildValidation.CreateInvalid("Platform target is not supported! Please install the missing Unity platform module.");

            if (Target == BuildTarget.NoTarget)
                return BuildValidation.CreateInvalid("BuildTarget cannot be NoTarget!");

            if (!string.IsNullOrEmpty(DeploymentPath) && DeploymentPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return BuildValidation.CreateIssued("Deployment Path contains invalid characters.");

            if (!string.IsNullOrEmpty(DeploymentFilename) && DeploymentFilename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return BuildValidation.CreateIssued("Filename contains invalid characters.");

            // handle possible deployment path issue validation extra
            if (ShowDeploymentPathWarnings)
            {
                if (string.IsNullOrEmpty(DeploymentPath))
                    return BuildValidation.CreateIssued("Deployment path is empty! Might cause unwanted side effects to build on project root!");
                else
                {
                    if (Path.IsPathFullyQualified(DeploymentPath))
                        return BuildValidation.CreateIssued("Absolute Path can cause issues on different environments.");
                    else
                    {
                        var sameLevelAsUnity = FileUtilities.NormalizeWindowsToUnix(Path.GetFullPath(Path.Combine(FileUtilities.AbsoluteProjectPath(), "..")));

                        if (!FullAbsoluteOutputPath.Contains(sameLevelAsUnity))
                            return BuildValidation.CreateIssued("Using relative path which are far outside of the Unity directory can cause issues.");
                    }
                }
            }

            return BuildValidation.Valid;
        }

        private void UpdateFullPath()
        {
            m_binding.fullPathField.value = FullAbsoluteOutputPath;
        }
    }
}