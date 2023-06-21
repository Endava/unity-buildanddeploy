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
    /// This BuildStep allows you to to copy a directory from source to target. Specify the recursion flag if needed.
    /// </summary>
    [Serializable, BuildStep("FilesAndFolders/CopyDirectory")]
    public class CopyDirectory : TestableBuildStep<string>
    {
        public override string Description => "Copies a directory to a specific destination path.";

        [SerializeField]
        private string sourcePath = string.Empty;

        [SerializeField]
        private string destinationPath = string.Empty;

        [SerializeField]
        private bool recursive = true;

        public override string FoldedParameterPreviewText => $"{sourcePath}{((recursive) ? "/*" : "")} -> {destinationPath}";

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
            BuildLogger.LogDebug($"Copy directory {sourcePath} to {destinationPath} {((recursive) ? "recursively" : "")}");
            var finalSourcePath = Helpers.ReplacePossibleTemplatesInString(sourcePath, Process.Main.DeploymentPath);
            var finalDestinationPath = Helpers.ReplacePossibleTemplatesInString(destinationPath, Process.Main.DeploymentPath);
            BuildLogger.LogDebug($"Copy file after possible template replacement {finalSourcePath} to {finalDestinationPath}");

            var success = FileUtilities.CopyDirectory(finalSourcePath, finalDestinationPath, recursive);

            if (!success)
                return Task.FromResult(BuildStepResult.CreateError($"Could not copy directory {finalSourcePath} -> {finalDestinationPath}"));

            BuildLogger.LogInformation($"Directory copy succeeded! {finalSourcePath} -> {finalDestinationPath}");
            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(sourcePath), "Source Directory");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(destinationPath), "Destination Directory");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(recursive), "Include Subfolders");
        }

        protected override Task<string> BeforeTestExecution() => Task.FromResult(string.Empty); // we dont need this, because it is given in the parameters
        protected override Task AfterTestExecution(string beforeState)
        {
            var finalDestinationPath = Helpers.ReplacePossibleTemplatesInString(destinationPath, Process.Main.DeploymentPath);
            if (FileUtilities.DeleteDirectory(finalDestinationPath))
            {
                Debug.Log($"Deleted copied folder at: {finalDestinationPath}");
                return Task.CompletedTask;
            }
            else
                throw new DirectoryNotFoundException(finalDestinationPath);
        }
    }
}