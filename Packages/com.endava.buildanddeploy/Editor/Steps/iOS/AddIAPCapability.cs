using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("iOS/AddIAPCapability")]
    public class AddIAPCapability : BaseCapability
    {
        public override string Description => "Enable In-App-Purchases on iOS";
        protected override string DescriptionLabel => "Use IAP";

        protected override Task<BuildStepResult> ExecuteStep()
        {
#if UNITY_IOS
            var manager = GetCapabilityManager();

            manager.AddGameCenter();
            manager.WriteToFile();

            BuildLogger.LogInformation($"Added IAP capabilities!");

            return Task.FromResult(BuildStepResult.Successfull);
#else
            return Task.FromResult(BuildStepResult.CreateError("AddGameCenterCapability is only valid in IOS builds!"));
#endif
        }
    }
}