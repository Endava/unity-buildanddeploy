using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable]
    public abstract class TestableBuildStep<T> : NewBuildStep
    {
        public override bool IsTestable => true;
        public virtual int DelayDurationAfterSuccessInMs => 0;

        protected abstract Task<T> BeforeTestExecution();
        protected abstract Task AfterTestExecution(T beforeState);

        protected async override Task<BuildStepResult> ExecuteTestStep()
        {
            if (IsTestable)
            {
                BuildStepResult result;
                T cachedObject = default;
                try
                {
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Prewarm!");
                    cachedObject = await BeforeTestExecution();
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Prewarm result = {cachedObject}!");
                }
                catch (Exception e)
                {
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Prewarm failed! Reason: {e.Message}!");
                    return BuildStepResult.CreateError($"Exception: {e.Message}");
                }

                try
                {
                    var testResult = await ExecuteStep();
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Test succeeded!");
                    result = BuildStepResult.Successfull;

                    if (DelayDurationAfterSuccessInMs > 0)
                    {
                        BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Delay TearDown for {DelayDurationAfterSuccessInMs}!");
                        await Task.Delay(DelayDurationAfterSuccessInMs);
                    }
                }
                catch (Exception e)
                {
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Test failed! Reason: {e.Message}!");
                    result = BuildStepResult.CreateError($"Exception: {e.Message}");
                }

                try
                {
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - TearDown!");
                    await AfterTestExecution(cachedObject);
                }
                catch (Exception e)
                {
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - TearDown failed! Reason: {e.Message}!");
                    result = BuildStepResult.CreateError($"Exception: {e.Message}");
                }

                return result;
            }
            else
                return BuildStepResult.Successfull; // success if not testable but executed accidently                
        }
    }
}