using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    /// <summary>
    /// This BuildStep allows you to overwrite/change the product name.
    /// </summary>
    [Serializable, BuildStep("General/SetProductName")]
    public class SetProductName : TestableBuildStep<string>
    {
        public override string Description => "Sets the product name (shared between multiple platforms)";

        [SerializeField]
        private string productName = string.Empty;
        public override string FoldedParameterPreviewText => productName;


        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(productName))
                return BuildValidation.CreateInvalid($"Productname cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"ProductName before {PlayerSettings.productName}");
            PlayerSettings.productName = productName;
            BuildLogger.LogDebug($"ProductName changed to {productName}");

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(productName), "Product Name");
        }

        protected override Task<string> BeforeTestExecution()
        {
            Debug.Log($"Before Test State: {PlayerSettings.productName}");
            return Task.FromResult(PlayerSettings.productName);
        }

        protected override Task AfterTestExecution(string beforeState)
        {
            PlayerSettings.productName = beforeState;
            Debug.Log($"After Test State: {PlayerSettings.productName}");
            return Task.CompletedTask;
        }
    }
}