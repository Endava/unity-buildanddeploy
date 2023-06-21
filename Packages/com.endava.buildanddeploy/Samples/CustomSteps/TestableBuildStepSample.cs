using Endava.BuildAndDeploy;
using Endava.BuildAndDeploy.BuildSteps;
using Endava.BuildAndDeploy.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BuildAndDeploy.SampleCode
{
    [Serializable, NewBuildStep("Samples/Testable Custom Step")]
    public class TestableBuildStepSample : TestableBuildStep<int>
    {
        // add a short description to make it clear what this step is doing
        public override string Description => "This is just a test for a testable step";

        // add a preview (if possible), so that the step can be clearly identified in folded state
        public override string FoldedParameterPreviewText => $"{TimeDelayInSec} sec";

        [SerializeField]
        protected int TimeDelayInSec = 10;

        public override BuildValidation Validate()
        {
            // within the validation override you can take care of possible paramterization issues and mark them as issued(issued but ok) or invalid(can't build)

            // time below zero are invalid
            if (TimeDelayInSec < 0)
            {
                return BuildValidation.CreateInvalid($"{nameof(TimeDelayInSec)} cannot be negativ!");
            }

            return BuildValidation.Valid;
        }

        protected override async Task<BuildStepResult> ExecuteStep()
        {
            // within execute step you can do any kind of custom tasks

            BuildLogger.LogDebug($"UnityTime Now before: {EditorApplication.timeSinceStartup}");
            await Task.Delay(new TimeSpan(0, 0, TimeDelayInSec));
            BuildLogger.LogDebug($"UnityTime Now after: {EditorApplication.timeSinceStartup}");

            // return that everything is alright at the end - or stop with
            return BuildStepResult.Successfull;
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            // under normal circumstances a render of a PropertyField might be the common scenario, but
            // if you want to do custom rendering, feel free to do it like this
            List<VisualElement> childs = new(stepContentContainer.Children());
            var propertyName = nameof(TimeDelayInSec);
            var label = "Time in Sec";

            if (childs.Find(c => c.name == propertyName) != null)
                return;

            // creates a custom slider
            var so = serializedProperty.FindPropertyRelative(propertyName);
            var pf = new SliderInt(string.IsNullOrEmpty(label) ? propertyName : label, 0, 120);
            pf.bindingPath = so.propertyPath;
            pf.name = propertyName;

            // bind and add it
            stepContentContainer.Add(pf);
            stepContentContainer.Bind(so.serializedObject);
        }

        protected override Task<int> BeforeTestExecution()
        {
            // if you click the test button, this is the "Warmup" phase of the test, where you can forward code/save possible changes within the execution/configure the steps test env.
            BuildLogger.LogDebug($"Time Now before: {DateTime.Now.Ticks}");
            return Task.FromResult(0); // return a cached object, which you can read within the teardown
        }

        protected override Task AfterTestExecution(int beforeState)
        {
            // this is the tests teardown, where you can revert changes, do cleanups or just do some post reporting
            BuildLogger.LogDebug($"Time Now after: {DateTime.Now.Ticks}");
            return Task.CompletedTask;
        }
    }
}