using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("Android/SetKeystorePath")]
    public class SetKeystorePath : TestableBuildStep<string>
    {
        [SerializeField]
        private string keystoreName = string.Empty;
        public override string Description => "Sets the password for the currently set keystore to a given value";
        public override string FoldedParameterPreviewText => keystoreName;

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(keystoreName))
                return BuildValidation.CreateInvalid("keystoreName cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"Android keystoreName before: {PlayerSettings.Android.keystoreName}");
            PlayerSettings.Android.keystoreName = keystoreName;
            BuildLogger.LogInformation($"Set Android Keystore Alias: {keystoreName}");
            BuildLogger.LogDebug($"Android keystoreName after: {PlayerSettings.Android.keystoreName}");

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(keystoreName), "Keystore  Name");
        }

        protected override Task<string> BeforeTestExecution()
        {
            Debug.Log($"Before Test State: {PlayerSettings.Android.keystoreName}");
            return Task.FromResult(PlayerSettings.Android.keystoreName);
        }

        protected override Task AfterTestExecution(string beforeState)
        {
            PlayerSettings.Android.keystoreName = beforeState;
            Debug.Log($"After Test State: {PlayerSettings.Android.keystoreName}");
            return Task.CompletedTask;
        }
    }
}
