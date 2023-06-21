using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("Android/SetKeystoreAliasPassword")]
    public class SetKeystoreAliasPassword : TestableBuildStep<string>
    {
        [SerializeField]
        private string AliasPassword = string.Empty;

        public override string Description => "Set Android Keystore Alias password.";
        public override string FoldedParameterPreviewText => string.IsNullOrEmpty(AliasPassword) ? "" : new string('*', AliasPassword.Length);


        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(AliasPassword))
                return BuildValidation.CreateInvalid("AliasPassword cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"Android AliasPassword before: {PlayerSettings.Android.keyaliasPass}");
            PlayerSettings.Android.keyaliasPass = AliasPassword;
            BuildLogger.LogInformation($"Set Android Keystore Password Alias: {AliasPassword}");
            BuildLogger.LogDebug($"Android AliasPassword after: {PlayerSettings.Android.keyaliasPass}");

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(AliasPassword), "Keystore Alias Password");
        }

        protected override Task<string> BeforeTestExecution()
        {
            Debug.Log($"Before Test State: {PlayerSettings.Android.keyaliasPass}");
            return Task.FromResult(PlayerSettings.Android.keyaliasPass);
        }

        protected override Task AfterTestExecution(string beforeState)
        {
            PlayerSettings.Android.keyaliasPass = beforeState;
            Debug.Log($"After Test State: {PlayerSettings.Android.keyaliasPass}");
            return Task.CompletedTask;
        }
    }
}
