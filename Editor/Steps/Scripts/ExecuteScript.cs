using Endava.BuildAndDeploy.Logging;
using NUnit.Framework.Internal;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    /// <summary>
    /// This BuildStep allows you to execute platform dependant external scripts (linux | windows | osx)
    /// </summary>
    [Serializable, BuildStep("Scripts/Execute Script")]
    public class ExecuteScript : NewBuildStep
    {
        public override string Description => "Executes an external script, which depends on the system.";

        [SerializeField]
        private string windowsScript = string.Empty;

        [SerializeField]
        private string osxScript = string.Empty;

        [SerializeField]
        private string linuxScript = string.Empty;

        protected override Task<BuildStepResult> ExecuteStep()
        {
            try
            {
                string usedScript = string.Empty;
#if UNITY_EDITOR_WIN
                usedScript = windowsScript;
#elif UNITY_EDITOR_OSX
                usedScript = osxScript;
#elif UNITY_EDITOR_LINUX
                usedScript = linuxScript;
#else
                usedScript = string.Empty;
#endif
                if (string.IsNullOrEmpty(usedScript))
                {
                    return Task.FromResult(BuildStepResult.CreateError($"Unknown/Unhandled platform or script not found!"));
                }

                var result = Helpers.StartExternalProcess(usedScript);
                if (result.Item1 != 0)
                {
                    return Task.FromResult(BuildStepResult.CreateError("Process \"{usedScript}\" returns with error code {result.Item1} ({result.Item2})!"));
                }
                else
                {
                    BuildLogger.LogInformation($"Process \"{usedScript}\" succeeded! ({result.Item2})");
                    return Task.FromResult(BuildStepResult.Successfull);
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(BuildStepResult.CreateError(ex.ToString()));
            }
        }

        public override BuildValidation Validate()
        {
            if (!HasAnyScript())
            {
                return BuildValidation.CreateIssued("Please add at least one script to the scripts or deactivate it!");
            }

            string absFilePath;
            if (!string.IsNullOrEmpty(windowsScript) && !CheckScriptExistance(windowsScript, out absFilePath))
            {
                return BuildValidation.CreateInvalid($"Windows Script not existing => {absFilePath}");
            }
            if (!string.IsNullOrEmpty(osxScript) && !CheckScriptExistance(osxScript, out absFilePath))
            {
                return BuildValidation.CreateInvalid($"OSX Script not existing => {absFilePath}");
            }
            if (!string.IsNullOrEmpty(linuxScript) && !CheckScriptExistance(linuxScript, out absFilePath))
            {
                return BuildValidation.CreateInvalid($"Linux Script not existing => {absFilePath}");
            }

            return BuildValidation.Valid;
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            DrawScriptPathProperty(stepContentContainer, serializedProperty, nameof(windowsScript), "Windows Script", () => OnAssignScript(ref windowsScript, new string[] { "Batch script", "bat,cmd" }));
            DrawScriptPathProperty(stepContentContainer, serializedProperty, nameof(osxScript), "OSX Script", () => OnAssignScript(ref osxScript, new string[] { "Shell script", "sh" }));
            DrawScriptPathProperty(stepContentContainer, serializedProperty, nameof(linuxScript), "Linux Script", () => OnAssignScript(ref linuxScript, new string[] { "Shell script", "sh" }));

            void OnAssignScript(ref string targetProperty, string[] filter)
            {
                string path = !string.IsNullOrEmpty(targetProperty) ? targetProperty : FileUtilities.AbsoluteProjectPath();
                var newPath = FileUtilities.ConvertPathRelativeToProject(EditorUtility.OpenFilePanelWithFilters("Choose script", path, filter));
                targetProperty = FileUtilities.NormalizeWindowsToUnix(!string.IsNullOrEmpty(newPath) ? newPath : targetProperty);
            }
        }

        protected bool HasAnyScript()
        {
            return !string.IsNullOrEmpty(windowsScript) || !string.IsNullOrEmpty(osxScript) || !string.IsNullOrEmpty(linuxScript);
        }

        protected static bool CheckScriptExistance(string scriptPath, out string fileAbsPath)
        {
            string projectPath = FileUtilities.AbsoluteProjectPath();
            var filePath = Path.Combine(projectPath, scriptPath);
            fileAbsPath = filePath;
            return File.Exists(filePath);
        }

        protected void DrawScriptPathProperty(VisualElement root, SerializedProperty serializedProperty, string propertyName, string headline, Action onButtonClicked)
        {
            var line = new VisualElement();
            line.style.flexGrow = 1;
            line.style.flexDirection = FlexDirection.Row;

            var propertyField = AddPropertyFieldWithLabel(line, serializedProperty, propertyName, headline);
            propertyField.style.flexGrow = 1;

            var chooseScriptButton = new Button();
            chooseScriptButton.name = "chooseScriptBtn";
            chooseScriptButton.style.width = 32;
            chooseScriptButton.text = "...";
            chooseScriptButton.clicked += onButtonClicked;
            line.Add(chooseScriptButton);
            root.Add(line);
        }
    }
}