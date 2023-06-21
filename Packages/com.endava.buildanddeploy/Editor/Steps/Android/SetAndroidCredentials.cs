using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("Android/SetAndroidCredentials")]
    public class SetAndroidCredentials : TestableBuildStep<Tuple<string, string, string, string>>
    {
        [SerializeField]
        private string setKeystoreAlias = string.Empty;
        [SerializeField]
        private string setKeystoreAliasPassword = string.Empty;
        [SerializeField]
        private string setKeystorePassword = string.Empty;
        [SerializeField]
        private string setKeystorePath = string.Empty;

        public override string FoldedParameterPreviewText => "Unfold step to see more!";

        public override string Description => "Set Android Credentials";

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(setKeystoreAlias))
                return BuildValidation.CreateInvalid("Keystore Alias cannot be null or empty!");

            if (string.IsNullOrEmpty(setKeystoreAliasPassword))
                return BuildValidation.CreateInvalid("Keystore Alias Password cannot be null or empty!");

            if (string.IsNullOrEmpty(setKeystorePassword))
                return BuildValidation.CreateInvalid("Keystore Password cannot be null or empty!");

            if (string.IsNullOrEmpty(setKeystorePath))
                return BuildValidation.CreateInvalid("Keystore Name cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"AndroidSettings before AliasName: {PlayerSettings.Android.keyaliasName}, AliasPass: {PlayerSettings.Android.keyaliasPass}, KeystorePass: {PlayerSettings.Android.keystorePass}, keystoreName: {PlayerSettings.Android.keystoreName}");

            PlayerSettings.Android.keyaliasName = setKeystoreAlias;
            PlayerSettings.Android.keyaliasPass = setKeystoreAliasPassword;
            PlayerSettings.Android.keystorePass = setKeystorePassword;
            PlayerSettings.Android.keystoreName = setKeystorePath;

            BuildLogger.LogDebug($"Settings applied {setKeystoreAlias}, {setKeystoreAliasPassword}, {setKeystorePassword}, {setKeystorePath}");
            BuildLogger.LogDebug($"AndroidSettings before AliasName: {PlayerSettings.Android.keyaliasName}, AliasPass: {PlayerSettings.Android.keyaliasPass}, KeystorePass: {PlayerSettings.Android.keystorePass}, keystoreName: {PlayerSettings.Android.keystoreName}");

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(setKeystoreAlias), "Keystore Alias");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(setKeystoreAliasPassword), "Keystore Alias Password");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(setKeystorePassword), "Keystore Password");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(setKeystorePath), "Keystore Path");
        }

        protected override Task<Tuple<string, string, string, string>> BeforeTestExecution()
        {
            Debug.Log($"Before Test State: {PlayerSettings.Android.keyaliasName}, {PlayerSettings.Android.keyaliasPass}, {PlayerSettings.Android.keystorePass}, {PlayerSettings.Android.keystoreName}");
            return Task.FromResult(new Tuple<string, string, string, string>(PlayerSettings.Android.keyaliasName, PlayerSettings.Android.keyaliasPass, PlayerSettings.Android.keystorePass, PlayerSettings.Android.keystoreName));
        }

        protected override Task AfterTestExecution(Tuple<string, string, string, string> beforeState)
        {
            PlayerSettings.Android.keyaliasName = beforeState.Item1;
            PlayerSettings.Android.keyaliasPass = beforeState.Item2;
            PlayerSettings.Android.keystorePass = beforeState.Item3;
            PlayerSettings.Android.keystoreName = beforeState.Item4;
            Debug.Log($"After Test State: {PlayerSettings.Android.keyaliasName}, {PlayerSettings.Android.keyaliasPass}, {PlayerSettings.Android.keystorePass}, {PlayerSettings.Android.keystoreName}");
            return Task.CompletedTask;
        }
    }
}
