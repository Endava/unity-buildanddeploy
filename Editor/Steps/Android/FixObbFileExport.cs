using Endava.BuildAndDeploy.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("Android/FixObbFileExport")]
    public class FixObbFileExport : NewBuildStep
    {
        public override string Description => "Renames the generated *.obb file to met the required naming pattern.";
        public override BuildValidation Validate() => BuildValidation.Valid;

        protected override Task<BuildStepResult> ExecuteStep()
        {
            try
            {
                if (!PlayerSettings.Android.useAPKExpansionFiles)
                {
                    BuildLogger.LogWarning($"Skipping FixObbFileExport since useAPKExpansionFiles is not active!");
                    return Task.FromResult(BuildStepResult.Successfull);
                }

                FileInfo file = new FileInfo(Process.Main.DeploymentPath);
                BuildLogger.LogDebug($"Outpath is {Process.Main.DeploymentPath} with extension {file.Extension}");

                if (!file.Exists)
                    return Task.FromResult(BuildStepResult.CreateError("Cannot find exported apk file."));

                var directory = new DirectoryInfo(Path.Combine(FileUtilities.AbsoluteProjectPath(), file.DirectoryName));
                if (!directory.Exists)
                    return Task.FromResult(BuildStepResult.CreateError("Directory cannot be found."));

                var obbFiles = directory.GetFiles("*.obb");
                BuildLogger.LogDebug($"Directory {directory.FullName} contains {obbFiles.Length} obb files");

                if (obbFiles.Length == 0)
                    return Task.FromResult(BuildStepResult.CreateError($"No Obb file found at {directory.FullName}"));

                if (obbFiles.Length != 1)
                    return Task.FromResult(BuildStepResult.CreateError($"Only one obb file should exists at {directory.FullName}"));

                var obbFile = obbFiles[0];
                string newFilename = string.Format("{0}/main.{1}.{2}.obb", directory,
                                                    PlayerSettings.Android.bundleVersionCode,
                                                    PlayerSettings.applicationIdentifier);

                BuildLogger.LogInformation($"Rename {obbFile.FullName} to {newFilename}");
                obbFile.MoveTo(newFilename);
            }
            catch (Exception ex)
            {
                return Task.FromResult(BuildStepResult.CreateError(ex.ToString()));
            }

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {

        }
    }
}
