using Endava.BuildAndDeploy;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BuildAndDeploy.SampleCode
{

    // The BuildModule attribute allows the BuildProcess to detect your module automatically
    // Define the order at which this module might be executes by assign the properties.
    [BuildModule(BuildExecutionMode.BeforeBuild, 10)]
    public class CustomModule : BuildProcessModule
    {
        public override string SectionName => "Custom Module";

        public override bool CanBeDisabled => true;

        [SerializeField]
        private int sampleInteger = 2;

        [SerializeField]
        private bool isToggled = false;

        public override VisualElement CreateContentUI(SerializedProperty serializedProperty, BuildProcessUiList editorUiList, Editor editor)
        {
            // this will render your modules content within the inspector by using the UI Toolkit's VisualElement

            var root = new VisualElement();

            var headline = new Label();
            headline.text = "<b>This is a custom module</b>";
            root.Add(headline);

            var toggleControl = new Toggle("Toggle Sample");
            toggleControl.BindProperty(serializedProperty.FindPropertyRelative(nameof(sampleInteger))); 
            root.Add(toggleControl);

            var propertyField = new PropertyField(serializedProperty.FindPropertyRelative(nameof(sampleInteger)), "Integer Sample");
            root.Add(propertyField);

            return root;
        }

        public override async Task<bool> Execute()
        {
            // execute anything you want within this module

            return true;
        }

        public override BuildValidation Validate()
        {
            // validate the module integrity and return incorrectnesses.
            if (sampleInteger == 0) return BuildValidation.CreateInvalid("SampleInteger with value zero is not allowed!");
            if (sampleInteger < 0) return BuildValidation.CreateIssued("SampleInteger lower than zero could cause issues!");

            return BuildValidation.Valid; // everything is alright
        }

        public override BuildPlayerOptions OnChangeBuildPlayerOptions(BuildPlayerOptions input)
        {
            // you can override OnChangeBuildPlayerOptions to manipulate the "BuildPlayerOptions" which are used within Unity once the Unity Build starts.

            return base.OnChangeBuildPlayerOptions(input);
        }

        public override void UpdateContentUI()
        {
            // you can override UpdateContentUI to update the properties within the hierarchy once the data has changed or anything outside was updated

            base.UpdateContentUI();
        }
    }
}
