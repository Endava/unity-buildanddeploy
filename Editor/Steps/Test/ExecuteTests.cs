using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("Test/ExecuteTest")]
    public class ExecuteTests : TestableBuildStep<bool>
    {
        public override string Description => "Executes UnitTest(s) from the Unity TestTools";

        [SerializeField]
        private TestMode testMode = TestMode.EditMode | TestMode.PlayMode;
        public override string FoldedParameterPreviewText => CreatePreviewText();

        private static Tuple<bool, string> testResults;
        private static bool testCompleted = false;

        public override BuildValidation Validate()
        {
            return BuildValidation.Valid;
        }

        protected override async Task<BuildStepResult> ExecuteStep()
        {
            if (testMode == 0) // nothing to test
                return BuildStepResult.Successfull;

            await RunTests(testMode);

            if (testResults != null && testResults.Item1)
                return BuildStepResult.Successfull;
            else
                return BuildStepResult.CreateError(testResults.Item2);
        }

        protected override Task<bool> BeforeTestExecution()
        {
            return Task.FromResult(true);
        }

        protected override Task AfterTestExecution(bool beforeState)
        {
            return Task.CompletedTask;
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(testMode), "Tests");
        }

        private static async Task RunTests(TestMode testModeToRun)
        {
            testCompleted = false;
            testResults = null;
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testRunnerApi.RegisterCallbacks(new TestResultCallback());
            var filter = new Filter()
            {
                testMode = testModeToRun
            };

            testRunnerApi.Execute(new ExecutionSettings(filter));

            await WaitWhile(TestCompleted, timeout: 1000 * 60);

            bool TestCompleted() => !testCompleted;
        }

        private static async Task WaitWhile(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }

        private string CreatePreviewText()
        {
            if (testMode.HasFlag(TestMode.EditMode) && testMode.HasFlag(TestMode.PlayMode))
                return "Tests: All";
            else if (testMode.HasFlag(TestMode.EditMode))
                return "Tests: Editor only";
            else
                return "Tests: PlayMode only";
        }

        private class TestResultCallback : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                testCompleted = true;

                if (!result.HasChildren && result.ResultState != "Passed")
                {
                    var output = string.Format("Test {0} {1}", result.Test.Name, result.ResultState);
                    //Debug.Log(output);
                    testResults = new(false, output);
                }
                else
                {
                    testResults = new(true, "Passed");
                }
            }

            public void TestStarted(ITestAdaptor test)
            {

            }

            public void TestFinished(ITestResultAdaptor result)
            {

            }
        }
    }
}