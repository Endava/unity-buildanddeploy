using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Endava.BuildAndDeploy.BuildSteps
{

    [Serializable, BuildStep("iOS/SetPBXProjectSetting")]
    public class SetPBXProjectSetting : NewBuildStep
    {
        [SerializeField]
        protected string Key = string.Empty;
        [SerializeField]
        protected string Value = string.Empty;

        public override string FoldedParameterPreviewText => $"{Key} : {Value}";
        public override string Description => "Set a PBXProject Build Property";

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(Key), "Key");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(Value), "Value");
        }

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(Key))
                return BuildValidation.CreateInvalid("Key cannot be null or empty!");
            else if (string.IsNullOrEmpty(Value))
                return BuildValidation.CreateInvalid("Value cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"SetPBXProjectSetting started");

#if UNITY_IOS
            var outputpath = Process.Main.DeploymentPath;
            var pbxProjectPath = PBXProject.GetPBXProjectPath(outputpath);

            BuildLogger.LogDebug($"PBX project path: {pbxProjectPath}");
            var targetName = new PBXProject().GetUnityMainTargetGuid();
            var pbx = new PBXProject();

            pbx.ReadFromFile(pbxProjectPath);

            var targetGUID = pbx.TargetGuidByName(targetName);

            pbx.SetBuildProperty(targetGUID, Key, Value);
            BuildLogger.LogDebug($"PBX project build property \"{Key}\" set to {Value}");
            pbx.WriteToFile(pbxProjectPath);

            return Task.FromResult(BuildStepResult.Successfull);
#else
            return Task.FromResult(BuildStepResult.CreateError("PBXProjectSetting is only valid in IOS builds!"));
#endif
        }
    }
}