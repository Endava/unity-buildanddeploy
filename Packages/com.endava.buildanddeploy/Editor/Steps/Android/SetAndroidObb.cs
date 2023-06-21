using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("Android/SetUseAPKExpansionFiles")]
    public class SetAndroidObb : TestableBuildStep<bool>
    {
        [SerializeField]
        private bool useObb = false;

        public override string Description => "Use Obb split in this build";

        public override string FoldedParameterPreviewText => useObb.ToString();

        public override BuildValidation Validate() => BuildValidation.Valid;

        protected override Task<BuildStepResult> ExecuteStep()
        {
            try
            {
                BuildLogger.LogDebug($"Android obb split before: {PlayerSettings.Android.useAPKExpansionFiles}");
                PlayerSettings.Android.useAPKExpansionFiles = useObb;
                BuildLogger.LogInformation($"Android SetUseAPKExpansionFiles: {useObb}");
                BuildLogger.LogDebug($"Android obb split after: {PlayerSettings.Android.useAPKExpansionFiles}");
            }
            catch (Exception ex)
            {
                return Task.FromResult(BuildStepResult.CreateError(ex.ToString()));
            }

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(useObb), "Use Obb");
        }

        protected override Task<bool> BeforeTestExecution()
        {
            Debug.Log($"Before Test State: {PlayerSettings.Android.useAPKExpansionFiles}");
            return Task.FromResult(PlayerSettings.Android.useAPKExpansionFiles);
        }

        protected override Task AfterTestExecution(bool beforeState)
        {
            PlayerSettings.Android.useAPKExpansionFiles = beforeState;
            Debug.Log($"After Test State: {PlayerSettings.Android.useAPKExpansionFiles}");
            return Task.CompletedTask;
        }
    }
}
