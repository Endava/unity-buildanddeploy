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
    /// This BuildStep deletes a folder from the given path. If the required flag is true, non existance will raise an exception.
    /// </summary>
    [Serializable, BuildStep("FilesAndFolders/DeleteDirectory")]
    public class DeleteDirectory : TestableBuildStep<string>
    {
        public override string Description => "Deletes a directory.";

        [SerializeField]
        private string sourcePath = string.Empty;

        [SerializeField]
        private bool required = true;

        public override string FoldedParameterPreviewText => $"{sourcePath}";

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(sourcePath))
                return BuildValidation.CreateInvalid("Source cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            var finalSourcePath = Helpers.ReplacePossibleTemplatesInString(sourcePath, Process.Main.DeploymentPath);

            if (!required && !FileUtilities.DirectoryExists(finalSourcePath))
            {
                BuildLogger.LogInformation($"Directory not existing but seems to be ok! {finalSourcePath}");
            }
            else
            {
                BuildLogger.LogDebug($"Delete directory {sourcePath}");

                if (!FileUtilities.DeleteDirectory(finalSourcePath))
                    return Task.FromResult(BuildStepResult.CreateError($"Could not delete directory {finalSourcePath}"));

                BuildLogger.LogInformation($"Directory deletion succeeded! {finalSourcePath}");
            }

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(sourcePath), "Source Directory");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(required), "Required");
        }

        protected override Task<string> BeforeTestExecution()
        {
            var finalSourcePath = Helpers.ReplacePossibleTemplatesInString(sourcePath, Process.Main.DeploymentPath);
            var finalDestinationPath = $"{Application.temporaryCachePath}/__Tmp";

            if (FileUtilities.DirectoryExists(finalSourcePath))
            {
                var result = FileUtilities.CopyDirectory(finalSourcePath, finalDestinationPath, true);
                Debug.Log($"Test Copy temporary state in {finalDestinationPath} is {result}");
                return Task.FromResult(finalDestinationPath);
            }
            else
            {
                if (required)
                    throw new DirectoryNotFoundException(finalSourcePath);
                else
                {
                    Debug.Log($"Skipping directory backup, since file not required and not found: {finalDestinationPath}");
                    return Task.FromResult(string.Empty);
                }
            }
        }

        protected override Task AfterTestExecution(string beforeState)
        {
            // directory might not be found and therefore no backup was created
            if (string.IsNullOrEmpty(beforeState))
                return Task.CompletedTask;

            var result = FileUtilities.DeleteDirectory(beforeState);
            Debug.Log($"Test Copy temporary rollback in {beforeState} is {result}");
            return Task.CompletedTask;
        }
    }
}