using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    /// <summary>
    /// This BuildStep allows you to overwrite/change the application identifier.
    /// </summary>
    [Serializable, BuildStep("General/SetBundleIdentifier")]
    public class SetBundleIdentifier : TestableBuildStep<string>
    {
        public override string Description => "Sets the bundle identifier (shared between multiple platforms)";

        [SerializeField]
        private string identifierName = string.Empty;
        public override string FoldedParameterPreviewText => identifierName;

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(identifierName))
                return BuildValidation.CreateInvalid("IdentifierName cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"ApplicationIdentifier before {PlayerSettings.applicationIdentifier}");
            PlayerSettings.applicationIdentifier = identifierName;
            BuildLogger.LogDebug($"Identifier changed to {identifierName}");

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(identifierName), "BundleIdentifier");
        }

        protected override Task<string> BeforeTestExecution()
        {
            Debug.Log($"Before Test State: {PlayerSettings.applicationIdentifier}");
            return Task.FromResult(PlayerSettings.productName);
        }

        protected override Task AfterTestExecution(string beforeState)
        {
            PlayerSettings.applicationIdentifier = beforeState;
            Debug.Log($"After Test State: {PlayerSettings.applicationIdentifier}");
            return Task.CompletedTask;
        }
    }
}