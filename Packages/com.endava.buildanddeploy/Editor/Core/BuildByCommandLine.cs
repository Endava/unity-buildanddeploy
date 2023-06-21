using Endava.BuildAndDeploy.Logging;
using System;
using UnityEditor;

namespace Endava.BuildAndDeploy
{
    public static class BuildFromCommandLine
    {
        /// <summary>
        /// Use this method from commandline to trigger a BuildProcess
        /// <code>Unity.exe -executeMethod Endava.BuildAndDeploy.BuildFromCommandLine.Build someMoreArguments</code>
        /// </summary>
        public static async void Build()
        {
            string[] arguments = Environment.GetCommandLineArgs();

            // cleanup build_log on each new build
            FileLogger.CleanLogFile();

            // if parameter parser will error - allow default unity logging
            BuildLogger.LogTargets = LogTargets.Unity;
            BuildLogger.LogLevel = LogLevel.Debug;

            var succeeded = CommandLineParser.TryParseArguments(arguments, out CommandLineParser parsedArguments);
            if (succeeded)
            {
                // set loglevel and target on cmd build or fallback!
                BuildLogger.LogTargets = parsedArguments.OverrideLogTargets.HasValue ? parsedArguments.OverrideLogTargets.Value : LogTargets.Everything;
                BuildLogger.LogLevel = parsedArguments.OverrideLogLevel.HasValue ? parsedArguments.OverrideLogLevel.Value : LogLevel.Debug;

                BuildLogger.LogInformation($"Build from CommandLine parse succeeded!");
                BuildLogger.LogInformation(parsedArguments.ToString());

                if (!parsedArguments.HasBuildTargetSet)
                {
                    BuildLogger.LogWarning($"According to documentation, providing a buildTarget is essential!");
                }

                if (!string.IsNullOrEmpty(parsedArguments.AndroidSdkPath))
                {
                    EditorPrefs.SetString("AndroidSdkRoot", parsedArguments.AndroidSdkPath);
                    BuildLogger.LogDebug($"Set Android SDK root to: {parsedArguments.AndroidSdkPath}");
                }

                if (!string.IsNullOrEmpty(parsedArguments.AndroidNdkPath))
                {
                    EditorPrefs.SetString("AndroidNdkRoot", parsedArguments.AndroidNdkPath);
                    BuildLogger.LogDebug($"Set Android NDK root to: {parsedArguments.AndroidNdkPath}");
                }

                if (!string.IsNullOrEmpty(parsedArguments.TargetProcessPath))
                {
                    BuildLogger.LogInformation($"Loading Build Process: {parsedArguments.TargetProcessPath}");
                   
                    var result = await BuildProcess.TryBuildFromCommandLine(
                        parsedArguments.TargetProcessPath, 
                        parsedArguments.OverrideFullOutputPath, 
                        parsedArguments.OverrideLogTargets, 
                        parsedArguments.OverrideLogLevel);

                    EditorApplication.Exit(result ? 0 : 1);
                   
                }
                else
                {
                    BuildLogger.LogError($"No Build Process parameter found - don't know what to do! ({string.Join(", ", arguments)})");
                    EditorApplication.Exit(1);
                }
            }
            else
            {
                BuildLogger.LogError($"Parsing argument failed ({string.Join(", ", arguments)})");
                EditorApplication.Exit(1);
            }
        }
    }
}