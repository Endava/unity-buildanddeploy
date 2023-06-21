using Endava.BuildAndDeploy.Logging;
using System.Collections.Generic;

namespace Endava.BuildAndDeploy
{
    /// <summary>
    /// This class can be used to extend the overwriteable properties within the build execution process.
    /// Once the BuildProcess gets started, a possible number of editable properties can be overwriten by this class
    /// </summary>
    public class OverwriteBuildProcessProperties
    {
        public LogTargets? LogTargets;
        public LogLevel? LogLevel;
        public bool? UseAutoRun;
        public List<BuildExecutionMode> ExecutedModules;
        public string FullOutputPath;

        public string CloudDeploymentPath;
        public string CloudExecutableName;
        public bool? IsCloudBuild;

        public override string ToString()
        {
            return
                $"{nameof(LogTargets)}: {LogTargets}," +
                $"{nameof(LogLevel)}: {LogLevel}," +
                $"{nameof(UseAutoRun)}: {UseAutoRun}," +
                $"{nameof(FullOutputPath)}: {FullOutputPath}," +
                $"{nameof(ExecutedModules)}: {(ExecutedModules?.Count > 0 ? $"({string.Join(',', ExecutedModules)})" : "null")}," +
                $"{nameof(CloudDeploymentPath)}: {CloudDeploymentPath}," +
                $"{nameof(CloudExecutableName)}: {CloudExecutableName}," +
                $"{nameof(IsCloudBuild)}: {IsCloudBuild}";
        }
    }
}