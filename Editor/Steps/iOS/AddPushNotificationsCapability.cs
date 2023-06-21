using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("iOS/AddPushNotificationsCapability")]
    public class AddPushNotificationsCapability : BaseCapability
    {
        [SerializeField]
        protected bool Development = false;

        public override string Description => "Enable Push notifications on iOS";

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(Development), "Development mode");
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
#if UNITY_IOS
            var manager = GetCapabilityManager();

            manager.AddPushNotifications(Development);
            manager.WriteToFile();

            BuildLogger.LogInformation($"Added PushNotification capabilities with development mode={Development}!");

            return Task.FromResult(BuildStepResult.Successfull);
#else
            return Task.FromResult(BuildStepResult.CreateError("AddPushNotificationsCapability is only valid in IOS builds!"));
#endif
        }
    }
}