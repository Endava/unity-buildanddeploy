using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{

    [Serializable, BuildStep("Android/SetKeystorePassword")]
    public class SetKeystorePassword : TestableBuildStep<string>
    {
        [SerializeField]
        private string keystorePassword = string.Empty;
        public override string Description => "Sets the password for the currently set keystore to a given value";

        public override string FoldedParameterPreviewText => string.IsNullOrEmpty(keystorePassword) ? "" : new string('*', keystorePassword.Length);

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(keystorePassword))
                return BuildValidation.CreateInvalid("KeystorePassword cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"Android keystorePassword before: {PlayerSettings.Android.keystorePass}");
            PlayerSettings.Android.keystorePass = keystorePassword;
            BuildLogger.LogInformation($"Set KeystorePassword: {keystorePassword}");
            BuildLogger.LogDebug($"Android keystorePassword after: {PlayerSettings.Android.keystorePass}");

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(keystorePassword), "Keystore  Password");
        }

        protected override Task<string> BeforeTestExecution()
        {
            Debug.Log($"Before Test State: {PlayerSettings.Android.keystorePass}");
            return Task.FromResult(PlayerSettings.Android.keystorePass);
        }

        protected override Task AfterTestExecution(string beforeState)
        {
            PlayerSettings.Android.keystorePass = beforeState;
            Debug.Log($"After Test State: {PlayerSettings.Android.keystorePass}");
            return Task.CompletedTask;
        }
    }
}
