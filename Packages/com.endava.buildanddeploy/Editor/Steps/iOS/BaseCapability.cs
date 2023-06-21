using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable]
    public abstract class BaseCapability : NewBuildStep
    {
        protected virtual string DescriptionLabel => "";

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddContentLabel(stepContentContainer, DescriptionLabel);
        }

        public override BuildValidation Validate() => BuildValidation.Valid;

#if UNITY_IOS
        protected UnityEditor.iOS.Xcode.ProjectCapabilityManager GetCapabilityManager()
        {
            var outputpath = Process.Main.DeploymentPath;
            var entitlementFilePath = outputpath + "/entitlement";

            var pbxProjectPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(outputpath);
            var targetName = new UnityEditor.iOS.Xcode.PBXProject().GetUnityMainTargetGuid();

            return new UnityEditor.iOS.Xcode.ProjectCapabilityManager(pbxProjectPath, entitlementFilePath, targetName);
        }
#endif
    }
}
