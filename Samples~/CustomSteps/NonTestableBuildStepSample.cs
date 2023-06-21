using Endava.BuildAndDeploy;
using Endava.BuildAndDeploy.BuildSteps;
using Endava.BuildAndDeploy.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BuildAndDeploy.SampleCode
{
    [Serializable, NewBuildStep("Samples/NonTestable Custom Step")]
    public class NonTestableBuildStepSample : NewBuildStep
    {
        // add a short description to make it clear what this step is doing
        public override string Description => "This is just a test";

        // add a preview (if possible), so that the step can be clearly identified in folded state
        public override string FoldedParameterPreviewText => fileName;

        [SerializeField]
        private string fileName = "test.txt";

        public override BuildValidation Validate()
        {
            // within the validation override you can take care of possible paramterization issues and mark them as issued(issued but ok) or invalid(can't build)

            // just as sample - having a filename length smaller x causes a warning with explanation text
            if (fileName.Length < 6)
            {
                return BuildValidation.CreateIssued($"Filename might be too small. That could cause issues!");
            }

            // empty filename is handled as error
            if (string.IsNullOrEmpty(fileName))
            {
                return BuildValidation.CreateInvalid($"Filename cannot be null or empty!");
            }

            return BuildValidation.Valid;
        }

        protected override async Task<BuildStepResult> ExecuteStep()
        {
            // within execute step you can do any kind of custom tasks
            // use the Process to get some details from the builded process itself (like output path in this case)
            var buildOutputPath = Path.GetDirectoryName(Process.Main.FullAbsoluteOutputPath);

            // do logs with the BuildLogger to make the build process transparent
            BuildLogger.LogDebug($"Some test output file {fileName} is created at {buildOutputPath}");

            // handle errors within your step execution if unexpected issues appear
            if (string.IsNullOrEmpty(buildOutputPath))
                return BuildStepResult.CreateError("Cannot be empty!");

            // do whatever you want and do it async if needed
            using (var file = File.Create(Path.Combine(buildOutputPath, fileName)))
            {

            }
            await Task.Delay(new TimeSpan(0, 0, 1));

            // return that everything is alright at the end - or stop with
            return BuildStepResult.Successfull;
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            // this renders the filename within the buildstep itself and bind it with the variable.
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(fileName), "Filename");
        }
    }
}