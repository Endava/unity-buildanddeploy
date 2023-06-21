using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("iOS/AddGameCenterCapability")]
    public class AddGameCenterCapability : BaseCapability
    {
        public override string Description => "Use GameCenter on iOS";
        protected override string DescriptionLabel => "Use GameCenter";

        protected override Task<BuildStepResult> ExecuteStep()
        {
#if UNITY_IOS
            var manager = GetCapabilityManager();

            manager.AddGameCenter();
            manager.WriteToFile();

            BuildLogger.LogInformation($"Added GameCenter capabilities!");

            return Task.FromResult(BuildStepResult.Successfull);
#else
            return Task.FromResult(BuildStepResult.CreateError("AddGameCenterCapability is only valid in IOS builds!"));
#endif
        }
    }
}
