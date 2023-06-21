using Endava.BuildAndDeploy.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    /// <summary>
    /// This BuildStep allows you to to copy a file from source to target.
    /// </summary>
    [Serializable, BuildStep("FilesAndFolders/CopyFile")]
    public class CopyFile : TestableBuildStep<string>
    {
        public override string Description => "Copies a file to a specific destination path.";

        [SerializeField]
        private string sourcePath = string.Empty;

        [SerializeField]
        private string destinationPath = string.Empty;

        public override string FoldedParameterPreviewText => $"{sourcePath} -> {destinationPath}";

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(sourcePath))
                return BuildValidation.CreateInvalid("Source cannot be null or empty!");

            if (string.IsNullOrEmpty(destinationPath))
                return BuildValidation.CreateInvalid("Destination cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"Copy file from {sourcePath} to {destinationPath}");
            var finalSourcePath = Helpers.ReplacePossibleTemplatesInString(sourcePath, Process.Main.DeploymentPath);
            var finalDestinationPath = Helpers.ReplacePossibleTemplatesInString(destinationPath, Process.Main.DeploymentPath);
            BuildLogger.LogDebug($"Copy file after possible template replacement {finalSourcePath} to {finalDestinationPath}");
            var success = FileUtilities.CopyFile(finalSourcePath, finalDestinationPath);

            if (!success)
                return Task.FromResult(BuildStepResult.CreateError($"Could not copy file {finalSourcePath} -> {finalDestinationPath}"));

            BuildLogger.LogInformation($"File copy succeeded! {finalSourcePath} -> {finalDestinationPath}");
            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(sourcePath), "Source");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(destinationPath), "Destination");
        }

        protected override Task<string> BeforeTestExecution() => Task.FromResult(string.Empty); // we dont need this, because it is given in the parameters
        protected override Task AfterTestExecution(string beforeState)
        {
            var finalDestinationPath = Helpers.ReplacePossibleTemplatesInString(destinationPath, Process.Main.DeploymentPath);
            if (FileUtilities.DeleteFile(finalDestinationPath))
            {
                Debug.Log($"Deleted copied file at: {finalDestinationPath}");
                return Task.CompletedTask;
            }
            else
                throw new FileNotFoundException(finalDestinationPath);
        }
    }
}