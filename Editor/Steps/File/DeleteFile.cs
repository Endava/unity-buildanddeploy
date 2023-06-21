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
    /// This BuildStep deletes a file from the given path. If the required flag is true, non existance will raise an exception.
    /// </summary>
    [Serializable, BuildStep("FilesAndFolders/DeleteFile")]
    public class DeleteFile : TestableBuildStep<string>
    {
        public override string Description => "Delete a file.";

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
            if (!required && !FileUtilities.FileExists(finalSourcePath))
            {
                BuildLogger.LogInformation($"File not existing but seems to be ok! {finalSourcePath}");
                return Task.FromResult(BuildStepResult.Successfull);
            }
            else
            {
                BuildLogger.LogDebug($"Deleting file {sourcePath}");

                var success = FileUtilities.DeleteFile(finalSourcePath);
                if (success)
                {
                    BuildLogger.LogInformation($"File deletion succeeded! {finalSourcePath}");
                    return Task.FromResult(BuildStepResult.Successfull);
                }
                else
                {
                    return Task.FromResult(BuildStepResult.CreateError($"Could not delete file {finalSourcePath}"));
                }
            }
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(sourcePath), "Source");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(required), "Required");
        }

        protected override Task<string> BeforeTestExecution()
        {
            var finalSourcePath = Helpers.ReplacePossibleTemplatesInString(sourcePath, Process.Main.DeploymentPath);
            var finalDestinationPath = $"{Application.temporaryCachePath}/__Tmp";

            if(FileUtilities.FileExists(finalSourcePath))
            {
                var result = FileUtilities.CopyFile(finalSourcePath, finalDestinationPath);
                Debug.Log($"Test Copy temporary state in {finalDestinationPath} is {result}");
                return Task.FromResult(finalDestinationPath);
            }
            else
            {
                if (required)
                    throw new FileNotFoundException(finalSourcePath);
                else
                {
                    Debug.Log($"Skipping file backup, since file not required and not found: {finalDestinationPath}");
                    return Task.FromResult(string.Empty);
                }
            }
        }

        protected override Task AfterTestExecution(string beforeState)
        {
            // file might not be found and therefore no backup was created
            if(string.IsNullOrEmpty(beforeState))
                return Task.CompletedTask;

            var result = FileUtilities.DeleteDirectory(beforeState);
            Debug.Log($"Test Copy temporary rollback in {beforeState} is {result}");
            return Task.CompletedTask;
        }
    }
}