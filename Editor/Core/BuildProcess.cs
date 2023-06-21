using Endava.BuildAndDeploy.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Endava.BuildAndDeploy
{
    [Serializable, CreateAssetMenu(menuName = "Build and Deploy/Build Process")]
    public partial class BuildProcess : ScriptableObject
    {
        protected const string CloudBuildOutputPathEnvKey = "OUTPUT_DIRECTORY";
        protected const string CloudBuildExecutableNameEnvKey = "BUILD_EXECUTABLE_NAME";
        protected const string CloudBuildProcessTargetPathEnvKey = "BUILD_PROCESS_TARGET_PATH";
        protected const string CloudBuildProcessTargetGUIDEnvKey = "BUILD_PROCESS_TARGET_GUID";
        

        public string Name => this.name;
        public OverwriteBuildProcessProperties OverwrittenProperties { get; protected set; } = null;

        [SerializeReference]
        protected List<BuildProcessModule> modules = new();
        public List<BuildProcessModule> Modules => modules;

        public T GetModule<T>() where T : BuildProcessModule => (T)Modules.Find(m => m is T);

        public MainModule Main => GetModule<MainModule>();

        public async void Recreate()
        {
            //is ugly for now, cause it repaints everything, but cause we using generic stuff on UICreate we need some architecture changes to make it work only with repaint.
            Selection.activeObject = null;
            await Task.Yield();
            Selection.activeObject = this;
        }

        public void EnableFromInspector()
        {
            //Modules.RemoveAll(m => m == null);

            var types = Helpers.GetAllSubTypes(typeof(BuildProcessModule));

            foreach (var t in types)
            {
                var attribute = (BuildModuleAttribute)Attribute.GetCustomAttribute(t, typeof(BuildModuleAttribute));
                if (attribute == null) continue;

                var moduleInstance = Modules.Find(m => m != null && m.GetType() == t);
                if (moduleInstance != null)
                {
                    //moduleInstance.Process = this; // is persisted!
                    moduleInstance.OnInititalize();
                    continue;
                }
                else
                {
                    var instance = (BuildProcessModule)Activator.CreateInstance(t);
                    instance.Process = this;
                    instance.OnCreate();
                    instance.OnInititalize();
                    Modules.Add(instance);
                }
            }

            // sort the build modules by using the execution mode and the order
            modules = modules
                .Where(m => m != null)
                .OrderBy(m => ((BuildModuleAttribute)Attribute.GetCustomAttribute(m.GetType(), typeof(BuildModuleAttribute))).ExecutionMode)
                .ThenBy(m => ((BuildModuleAttribute)Attribute.GetCustomAttribute(m.GetType(), typeof(BuildModuleAttribute))).Order)
                .ToList();
        }

        public async Task<bool> TryBuildFromEditor(bool useAutoRun = false)
        {
            var overwriteProperties = new OverwriteBuildProcessProperties();
            overwriteProperties.UseAutoRun = useAutoRun;

            var result = await TryBuildInternal(overwriteProperties);
            EditorUtility.ClearProgressBar(); // clear possible progress bars in editor
            return result;
        }

        public static async Task<bool> TryBuildFromCommandLine(string processPath, string fullOutputPath = null, LogTargets? overwriteLogTargets = null, LogLevel? overwriteLogLevel = null)
        {
            var overwriteProperties = new OverwriteBuildProcessProperties();
            overwriteProperties.FullOutputPath = fullOutputPath;
            overwriteProperties.LogTargets = overwriteLogTargets;
            overwriteProperties.LogLevel = overwriteLogLevel;

            return await TryBuildFromCommandLine(processPath, overwriteProperties);
        }

        public static async Task<bool> TryBuildFromCommandLine(string processPath, OverwriteBuildProcessProperties overwrittenProperties)
        {
            BuildProcess process = GetActualBuildProcessByPath(processPath);

            if (process == null)
            {
                BuildLogger.LogError($"{processPath} BuildConfig not found!");
                return false;
            }

            return await process.TryBuildInternal(overwrittenProperties);
        }

        /// <summary>
        /// This method can be called within the UnityCloudBuild and allows execution of all modules, which are before the actual unity build
        /// </summary>
        /// <remarks>You can use various enviroment variables to identify within your build environment. See: https://unity-technologies.github.io/cloud-build-docs-public/environment-variables</remarks>
        /// <example>Endava.BuildAndDeploy.BuildProcess.TryPreExportForUnityCloud</example>
        public static async void TryPreExportForUnityCloud()
        {
            // if parameter parser will error - allow default unity logging
            BuildLogger.LogTargets = LogTargets.Unity;
            BuildLogger.LogLevel = LogLevel.Debug;

            var variables = $"Cloud Build Environment variables listing: \n";
            foreach (DictionaryEntry envVarPair in Environment.GetEnvironmentVariables())
            {
                variables += $"\t \"{envVarPair.Key}\" : \"{envVarPair.Value}\"\n";
            }
            BuildLogger.LogDebug(variables);

            BuildProcess process = GetBuildProcessFromCloudBuildConfiguration();
            if (process != null)
            {
                OverwriteBuildProcessProperties overwriteProperties = new();
                overwriteProperties.ExecutedModules = new List<BuildExecutionMode>() { BuildExecutionMode.BeforeBuild };
                overwriteProperties.LogLevel = LogLevel.Debug;
                overwriteProperties.LogTargets = LogTargets.Unity;
                overwriteProperties.IsCloudBuild = true;
                overwriteProperties.CloudDeploymentPath = Environment.GetEnvironmentVariable(CloudBuildOutputPathEnvKey);
                overwriteProperties.CloudExecutableName = Environment.GetEnvironmentVariable(CloudBuildExecutableNameEnvKey);

                var result = await process.TryBuildInternal(overwriteProperties);
                if (!result)
                    BuildLogger.LogError($"Build Process \"{process.Name}\" was started, but failed in execution!");
            }
            else
            {
                BuildLogger.LogError($"Process found empty! Stop execution!");
                EditorApplication.Exit(1);
            }
        }

        /// <summary>
        /// This method can be called within the UnityCloudBuild and allows execution of all process modules, which are AFTER the actual unity build.
        /// </summary>
        /// <example>Endava.BuildAndDeploy.BuildProcess.TryPostExportForUnityCloud</example>
        public static async void TryPostExportForUnityCloud()
        {
            BuildProcess process = GetBuildProcessFromCloudBuildConfiguration();
            if (process != null)
            {
                OverwriteBuildProcessProperties overwriteProperties = new();
                overwriteProperties.ExecutedModules = new List<BuildExecutionMode>() { BuildExecutionMode.AfterBuild };
                overwriteProperties.LogLevel = LogLevel.Debug;
                overwriteProperties.LogTargets = LogTargets.Unity;
                overwriteProperties.IsCloudBuild = true;
                overwriteProperties.CloudDeploymentPath = Environment.GetEnvironmentVariable(CloudBuildOutputPathEnvKey);
                overwriteProperties.CloudExecutableName = Environment.GetEnvironmentVariable(CloudBuildExecutableNameEnvKey);

                var result = await process.TryBuildInternal(overwriteProperties);
                if (!result)
                    BuildLogger.LogError($"Build Process \"{process.Name}\" was started, but failed in execution!");
            }
            else
            {
                BuildLogger.LogError($"Process found empty! Did you forgot to add the Environment variable \"{CloudBuildProcessTargetPathEnvKey}\" or \"{CloudBuildProcessTargetGUIDEnvKey}\" into your cloud build settings?");
            }
        }

        public virtual BuildValidation IsBuildable()
        {
            // main module is required!
            if (Main == null)
                return BuildValidation.CreateInvalid("Required \"MainModule\" not found.");

            foreach (var module in Modules)
            {
                var valid = module.Validate();
                if (!valid.Succeeded)
                    return valid;
            }

            return BuildValidation.Valid;
        }

        public virtual BuildPlayerOptions CreateBuildPlayerOptionsFromModules()
        {
            BuildPlayerOptions options = new BuildPlayerOptions();

            foreach (var module in Modules)
            {
                if (module != null && module.IsEnabled && module.Validate().Succeeded)
                    options = module.OnChangeBuildPlayerOptions(options);
            }

            return options;
        }

        protected virtual async Task<bool> TryBuildInternal() => await TryBuildInternal(null);
        protected virtual async Task<bool> TryBuildInternal(OverwriteBuildProcessProperties overwrite)
        {
            try
            {
                OverwrittenProperties = overwrite;

                // we need to execute the main logging module first, to activate the logging (or take the overwritten once)
                var executeMain = true;
                if (OverwrittenProperties?.ExecutedModules?.Count > 0 && OverwrittenProperties.ExecutedModules?.Contains(BuildExecutionMode.Setup) != true)
                {
                    executeMain = false;
                }

                if (executeMain)
                {
                    await Main.Execute();
                }

                var startReportLog = "---------- Start building ----------\n" +
                    $"BuildProcess: {this.Name}\n" +
                    $"LogLevel: {BuildLogger.LogLevel}\n" +
                    $"LogTargets: {BuildLogger.LogTargets}\n" +
                    $"Deployment Path: \"{Main.DeploymentPath}\"\n" +
                    $"Full Output Path: \"{Main.FullAbsoluteOutputPath}\"\n\n" +
                    $"OverwriteProperties: \"{overwrite}\"\n";
                BuildLogger.LogInformation(startReportLog);

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                foreach (var module in Modules)
                {
                    // skip logging module
                    if (module.GetType() == typeof(MainModule)) continue;

                    if (!await ValidateAndExecuteModuleInternally(module))
                    {
                        BuildLogger.LogInformation("---------- Building process stopped! ----------");
                        BuildLogger.LogInformation($"Stopped at module \"{module.SectionName}\"");
                        return false;
                    }
                }

                stopWatch.Stop();
                BuildLogger.LogInformation("---------- Building process finished successfully! ----------");
                BuildLogger.LogInformation($"Took {stopWatch.Elapsed.ToString(@"hh\:mm\:ss")}");

                return true;
            }
            catch (Exception e)
            {
                BuildLogger.LogError($"Exception was raised {e.Message}\n{e.StackTrace}!");
                return false;
            }
            finally
            {
                OverwrittenProperties = null;
            }
        }

        protected virtual async Task<bool> ValidateAndExecuteModuleInternally(BuildProcessModule module)
        {
            if (!module.IsEnabled)
            {
                BuildLogger.LogDebug($"Skipping {module.SectionName}, since it is disabled!");
                return true; // dont do anything when the module is disabled
            }

            if (OverwrittenProperties?.ExecutedModules?.Count > 0)
            {
                var modulesExecutionMode = ((BuildModuleAttribute)Attribute.GetCustomAttribute(module.GetType(), typeof(BuildModuleAttribute))).ExecutionMode;
                if (!OverwrittenProperties.ExecutedModules.Contains(modulesExecutionMode))
                {
                    BuildLogger.LogDebug($"Skipping {module.SectionName}, since the executed modules excludes it from execution {string.Join(',', OverwrittenProperties.ExecutedModules)}!");
                    return true; // dont do anything when the module is disabled
                }
            }

            var validation = module.Validate();
            if (validation.Succeeded)
            {
                BuildLogger.LogInformation($"---------- Executing {module.SectionName}! ----------");
                var result = await module.Execute();
                if (!result)
                {
                    BuildLogger.LogError($"Build stopped after failing in {module.SectionName}!");
                    return false;
                }
            }
            else
            {
                BuildLogger.LogError($"Build stopped after failing in {module.SectionName} validation! {validation}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a build process by its path
        /// </summary>
        /// <remarks>
        /// might be required, when unity command line builds looses the "asset context"
        /// </remarks>
        /// <returns>Returns the buildprocess after trying to recreate it.</returns>
        protected static BuildProcess GetActualBuildProcessByPath(string targetProcessPath)
        {
            if (!Path.HasExtension(targetProcessPath))
                targetProcessPath = $"{targetProcessPath}.asset";

            return AssetDatabase.LoadAssetAtPath(targetProcessPath, typeof(BuildProcess)) as BuildProcess;
        }

        /// <summary>
        /// Gets a build process by its path
        /// </summary>
        /// <returns>Returns the build process or null.</returns>
        protected static BuildProcess GetActualBuildProcessByGUID(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(assetPath)) return GetActualBuildProcessByPath(guid);
            return null;
        }

        /// <summary>
        /// Gets a build process by using the cloud build environment variables
        /// </summary>
        /// <returns>Returns the build process or null.</returns>
        protected static BuildProcess GetBuildProcessFromCloudBuildConfiguration()
        {
            var buildProcessTargetPath = Environment.GetEnvironmentVariable(CloudBuildProcessTargetPathEnvKey);
            if (!string.IsNullOrEmpty(buildProcessTargetPath))
            {
                BuildLogger.LogDebug($"Process Path found: {buildProcessTargetPath}!");
                var process = GetActualBuildProcessByPath(buildProcessTargetPath);

                if (process != null)
                {
                    return process;
                }
                else
                {
                    BuildLogger.LogError($"No process found at path {buildProcessTargetPath}!");
                    return null;
                }
            }

            var buildProcessTargetGuid = Environment.GetEnvironmentVariable(CloudBuildProcessTargetGUIDEnvKey);
            if (!string.IsNullOrEmpty(buildProcessTargetGuid))
            {
                BuildLogger.LogDebug($"Process GUID found: {buildProcessTargetGuid}!");
                var process = GetActualBuildProcessByGUID(buildProcessTargetGuid);

                if (process != null)
                {
                    return process;
                }
                else
                {
                    BuildLogger.LogError($"No process found with GUID {buildProcessTargetGuid}!");
                    return null;
                }
            }
            else
            {
                BuildLogger.LogError($"You forgot to add the Environment variable \"{CloudBuildProcessTargetPathEnvKey}\" or \"{CloudBuildProcessTargetGUIDEnvKey}\" into your cloud build settings?");
            }

            return null;
        }
    }
}