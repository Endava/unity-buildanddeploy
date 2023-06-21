using Endava.BuildAndDeploy.BuildSteps;
using Endava.BuildAndDeploy.Logging;
using Endava.BuildAndDeploy.UxmlBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    public abstract class BaseBuildStepsModule : BuildProcessModule
    {
        public override bool CanBeDisabled => true;

        [SerializeReference]
        private List<NewBuildStep> buildSteps = new();
        public List<NewBuildStep> BuildSteps => buildSteps;

        private List<(string, Type)> buildStepTypeMap = new List<(string, Type)>();
        private BuildStepsModuleBinding buildStepBinding;

        public override void OnInititalize() 
        {
            // recreate buildStep type mapping to search for all available
            buildStepTypeMap.Clear();
            var types = Helpers.GetAllSubTypes(typeof(NewBuildStep));
            var displays = GetBuildStepWithAttribute(types);

            foreach (var d in displays)
                buildStepTypeMap.Add((d.Item2, d.Item1));
        }

        public override async Task<bool> Execute()
        {
            //var settings = process.BuildSettings;
            for (int i = 0; i < BuildSteps.Count; i++)
            {
                var step = BuildSteps[i];
                if (step == null) continue;

                // skipping inactive steps
                if (!step.IsEnabled)
                    continue;

                if (!Application.isBatchMode)
                {
                    float progress = ((float)i / (float)BuildSteps.Count);
                    var cancel = EditorUtility.DisplayCancelableProgressBar($"Build ({i + 1}/{BuildSteps.Count + 1})", step.Name, progress);

                    if (cancel)
                        break;
                }

               var stepResult = await step.Execute(/*settings*/); // TODO:???

                if (!stepResult.Succeeded)
                {
                    BuildLogger.LogError($"{step.Name} failed: {stepResult}");
                    //abort
                    return false;
                }
            }

            return true;
        }

        public override VisualElement CreateContentUI(SerializedProperty serializedProperty, BuildProcessUiList editorUiList, Editor editor)
        {
            var moduleUi = editorUiList.buildStepsModule.Instantiate();
            buildStepBinding = new BuildStepsModuleBinding(moduleUi);
            
            buildStepBinding.removebutton.clicked += () => {
                if (buildStepBinding.buildStepList.selectedIndex != -1)
                {
                    this.BuildSteps.RemoveAt(buildStepBinding.buildStepList.selectedIndex);
                    serializedProperty.serializedObject.Update();
                    buildStepBinding.buildStepList.RefreshItems();
                }
            };
            buildStepBinding.addbutton.clicked += () =>
            {
                GenericMenu menu = new GenericMenu();

                foreach (var entry in buildStepTypeMap)
                    menu.AddItem(new GUIContent(entry.Item1), false, OnAddBuildStep, (entry.Item2, serializedProperty));

                menu.ShowAsContext();
            };

            // make and bind list
            buildStepBinding.buildStepList.makeItem = () => editorUiList.buildStepFrame.Instantiate();
            buildStepBinding.buildStepList.itemsSource = BuildSteps;
            buildStepBinding.buildStepList.bindItem = (view, index) => OnBindBuildStepEntry(view, index, serializedProperty.FindPropertyRelative(nameof(buildSteps)).GetArrayElementAtIndex(index));
            buildStepBinding.buildStepList.unbindItem = (view, index) => OnUnbindBuildStepEntry(view, index, serializedProperty.FindPropertyRelative(nameof(buildSteps)).GetArrayElementAtIndex(index));


            return moduleUi;
        }

        public override void UpdateContentUI()
        {
            foreach (var step in BuildSteps)
            {
                if(step != null)
                    step.UpdateUI();
            }
        }


        private void OnBindBuildStepEntry(VisualElement buildStepItemView, int index, SerializedProperty serializedProperty)
        {
            if (buildSteps[index] != null)
            {
                buildSteps[index].BindBuildStepListItemUi(buildStepItemView, serializedProperty);
            }
            else
            {
                var binding = new BuildStepEntryFrameBinding(buildStepItemView);
                binding.stepCtr.style.display = DisplayStyle.None;
                binding.errorCtr.style.display = DisplayStyle.Flex;
                binding.errorLabel.text = "BuildStep class invalid or not existing!";
            }
        }

        private void OnUnbindBuildStepEntry(VisualElement buildStepItemView, int index, SerializedProperty serializedProperty)
        {
            buildSteps[index].UnbindBuildStepListItemUi(buildStepItemView, serializedProperty);
        }

        public override BuildValidation Validate()
        {
            if (!IsEnabled) return BuildValidation.Valid;
            if (BuildSteps.Count == 0) return BuildValidation.CreateIssued($"{SectionName} should contain steps, when enabled.");

            // this might happen, when the step is not code referenced (no code found!)
            if (BuildSteps.Where(x => x == null).ToList().Count > 0) 
                return BuildValidation.CreateInvalid("Steps contains unreferenced code entries!");


            var validation = new BuildValidation(ValidationStatus.Valid, $"{SectionName}{Environment.NewLine}");

            foreach (var step in BuildSteps)
            {
                if (!step.IsEnabled) 
                    continue;

                validation = BuildValidation.Concat(validation, step.Validate());
            }

            return validation;
        }

        public override void UpdateValidationUI(BuildValidation validation)
        {
            base.UpdateValidationUI(validation);

            foreach (var step in BuildSteps)
            {
                if(step != null)
                    step.UpdateValidationState();
            }
        }

        private void OnAddBuildStep(object arg)
        {
            var argument = ((Type buildStepType, SerializedProperty prop))arg;
            var type = argument.buildStepType;
            var newBuildStep = (NewBuildStep)Activator.CreateInstance(type);

            if (newBuildStep == null)
                return;

            newBuildStep.Process = Process;
            this.BuildSteps.Add(newBuildStep);

            argument.prop.serializedObject.Update();
            buildStepBinding.buildStepList.RefreshItems();
        }

        protected static List<(Type, string)> GetBuildStepWithAttribute(Type[] types)
        {
            List<(Type, string)> result = new List<(Type, string)>();

            foreach (var t in types)
            {
                var attribute = (BuildStepAttribute)Attribute.GetCustomAttribute(t, typeof(BuildStepAttribute));

                if (attribute == null || attribute.Path == null)
                    continue;

                result.Add((t, attribute.Path));
            }

            return result;
        }
    }
}