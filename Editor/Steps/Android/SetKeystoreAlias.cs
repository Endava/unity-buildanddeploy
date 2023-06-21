using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("Android/SetKeystoreAlias")]
    public class SetKeystoreAlias : TestableBuildStep<string>
    {
        [SerializeField]
        private string AliasName = string.Empty;

        public override string Description => "Set Android Keystore Alias.";
        public override string FoldedParameterPreviewText => AliasName;

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(AliasName))
                return BuildValidation.CreateInvalid("AliasName cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"Android AliasName before: {PlayerSettings.Android.keyaliasName}");
            PlayerSettings.Android.keyaliasName = AliasName;
            BuildLogger.LogInformation($"Set Android Keystore Alias: {AliasName}");
            BuildLogger.LogDebug($"Android AliasName after: {PlayerSettings.Android.keyaliasName}");

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(AliasName), "Keystore Alias");
        }

        protected override Task<string> BeforeTestExecution()
        {
            Debug.Log($"Before Test State: {PlayerSettings.Android.keyaliasName}");
            return Task.FromResult(PlayerSettings.Android.keyaliasName);
        }

        protected override Task AfterTestExecution(string beforeState)
        {
            PlayerSettings.Android.keyaliasName = beforeState;
            Debug.Log($"After Test State: {PlayerSettings.Android.keyaliasName}");
            return Task.CompletedTask;
        }
    }
}
