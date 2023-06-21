using Endava.BuildAndDeploy.UxmlBindings;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    [CustomEditor(typeof(BuildProcess))]
    public partial class BuildProcessInspector : Editor
    {
        public override bool UseDefaultMargins() => false;

        [SerializeField]
        private BuildProcessUiList m_uiList;
        public BuildProcessUiList uiList => m_uiList;

        private BuildProcess buildProcess = null;
        private ProcessMainBinding binding;

        protected virtual void OnEnable() => buildProcess = target as BuildProcess;

        protected override void OnHeaderGUI() { }

        public override VisualElement CreateInspectorGUI()
        {
            var inspectorRoot = new VisualElement();
            inspectorRoot.styleSheets.Add(uiList.commonStyles); // add the common stylesheet

            buildProcess?.EnableFromInspector();
            serializedObject.Update();

            var view = CreateView();
            if (view != null)
                inspectorRoot.Add(view);

            inspectorRoot.TrackSerializedObjectValue(serializedObject, (obj) =>
            {
                UpdateUI();
                EditorUtility.SetDirty(buildProcess);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                UpdateValidation();
            });

            return inspectorRoot;
        }

        protected virtual VisualElement CreateView()
        {
            if (serializedObject == null) return null;

            var view = uiList.ProcessMainUi.Instantiate();
            binding = new ProcessMainBinding(view);


            // render headline
            binding.headline.text = buildProcess.Name;

            // render command toolbar
            var canBuild = buildProcess.IsBuildable().Succeeded;
            //binding.ctr_root.SetEnabled(canBuild);
            binding.buildBtn.text = "Build";
            binding.buildBtn.tooltip = canBuild ? string.Empty : "One or more issues occured within your build setup. Please try to fix all errors first!";
            binding.buildBtn.clicked += OnBuildButtonPressed;

            binding.buildAndRunBtn.text = "Build & Run";
            binding.buildAndRunBtn.tooltip = canBuild ? string.Empty : "One or more issues occured within your build setup. Please try to fix all errors first!";
            binding.buildAndRunBtn.clicked += OnBuildAndRunButtonPressed;

            binding.cmdlineBtn.tooltip = "Command Line Help";
            binding.cmdlineBtn.clicked += OnCommandLineButtonPressed;


            // render each module separately
            for (int i = 0; i < buildProcess.Modules.Count; i++)
            {
                var module = buildProcess.Modules[i];

                if (module != null)
                    module?.CreateInspectorUI(binding.ctrmodules, serializedObject.FindProperty("modules").GetArrayElementAtIndex(i), uiList, this);
                else
                    binding.ctrmodules.Add(uiList.CreateErrorContent("Module invalid or not existing!"));
            }

            UpdateValidation();

            return view;
        }

        protected virtual void UpdateUI()
        {
            foreach (var m in buildProcess.Modules)
                m.UpdateInspectorUI();
        }

        protected virtual void UpdateValidation()
        {
            var validations = new List<BuildValidation>();
            var status = ValidationStatus.Valid;

            for (int i = 0; i < buildProcess.Modules.Count; i++)
            {
                var module = buildProcess.Modules[i];

                if (module != null)
                {
                    var validation = module.Validate();
                    module.UpdateValidationUI(validation);
                    validations.Add(validation);

                    if (validation.Status > status)
                        status = validation.Status;
                }
            }

            var tooltipValidationText = "";
            for (int i = 0; i < validations.Count; i++)
            {
                var validation = validations[i];
                if (validation.Status > ValidationStatus.Valid)
                {
                    var statusType = validation.Status == ValidationStatus.Invalid ? "-Error: " : "Warning: ";
                    var newLine = i < validations.Count - 1 ? Environment.NewLine : string.Empty;
                    tooltipValidationText += $"{statusType} {validation.Message}{newLine}";
                }
            }
                        
            binding.icon.tooltip = "";
            Styles.AssignValidationStateToTargetIcon(binding.icon, status, true);

            switch (status)
            {
                case ValidationStatus.Issued:
                {
                    binding.validationInfo.style.display = DisplayStyle.Flex;
                    binding.validationInfo.messageType = HelpBoxMessageType.Warning;
                    binding.validationInfo.text = $"<b>Issue(s) found inside the Build Process. Some unwanted side effects might appear - but it can be builded!</b>{Environment.NewLine}{Environment.NewLine}{tooltipValidationText}";
                }
                break;
                case ValidationStatus.Invalid:
                {
                    binding.validationInfo.style.display = DisplayStyle.Flex;
                    binding.validationInfo.messageType = HelpBoxMessageType.Error;
                    binding.validationInfo.text = $"<b>Issue(s) found which blocks the Build Process. Fix the issue(s) to continue.</b>{Environment.NewLine}{Environment.NewLine}{tooltipValidationText}";
                }
                break;
                default:
                {
                    binding.validationInfo.style.display = DisplayStyle.None;
                    binding.validationInfo.messageType = HelpBoxMessageType.None;
                    binding.validationInfo.text = string.Empty;
                }
                break;
            }
        }

        protected virtual async void OnBuildButtonPressed()
        {
            await buildProcess.TryBuildFromEditor();
        }

        protected virtual async void OnBuildAndRunButtonPressed()
        {
            await buildProcess.TryBuildFromEditor(true);
        }

        protected virtual void OnCommandLineButtonPressed()
        {
            var command = $"-batchmode -projectPath \"{FileUtilities.AbsoluteProjectPath()}\" -executeMethod {CommandLineParser.ExecuteMethod} {CommandLineParser.BuildProcessKey}\"{AssetDatabase.GetAssetPath(buildProcess)}\"";
            //-buildTarget {buildProcess.Main.Target} Add later, but there are only some strings given.
            //https://docs.unity3d.com/Manual/EditorCommandLineArguments.html

            var message = $@"{CommandLineParser.ListArgumentsAsStrings()}

This is the command which was copied to your clipboard and you need to execute this within Unity batch mode:
""{command}""
";

            EditorUtility.DisplayDialog("CommandLine Help", message, "OK");
            EditorGUIUtility.systemCopyBuffer = command;
        }
    }
}