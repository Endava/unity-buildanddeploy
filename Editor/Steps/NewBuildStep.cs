using Endava.BuildAndDeploy.Logging;
using Endava.BuildAndDeploy.UxmlBindings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable]
    public abstract class NewBuildStep
    {
        [SerializeField]
        private string customStepName = string.Empty;
        public string Name { get { return string.IsNullOrEmpty(customStepName) ? DefaultName : customStepName; } set { customStepName = value; } }

        [SerializeField]
        private bool isEnabled = true;
        public bool IsEnabled => isEnabled;

        //[SerializeField]
        private bool isFolded = true;
        public bool IsFolded => isFolded;

        [SerializeField]
        public BuildProcess Process = null;

        protected virtual string DefaultName => GetType().Name;
        public virtual string Description { get; } = null;
        public virtual string FoldedParameterPreviewText => null; // use this to provide a text, which will be shown as parameter preview

        public virtual bool HasProperties => true;
        protected abstract Task<BuildStepResult> ExecuteStep();
        public abstract BuildValidation Validate();
        protected abstract void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty);

        public virtual bool IsTestable => false;
        protected virtual Task<BuildStepResult> ExecuteTestStep() => throw new NotImplementedException();

        protected BuildStepEntryFrameBinding buildStepFrameBinding;

        public async Task<BuildStepResult> Execute()
        {
            if (!IsEnabled)
            {
                BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Disabled & Skipped!");
                return BuildStepResult.Successfull;
            }

            BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Validation start!");
            var validationResult = Validate();
            if (!validationResult.Succeeded)
            {
                BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Validation FAILED! - Reason: {validationResult.Message}");
                return BuildStepResult.CreateError($"Validation FAILED! {validationResult}");
            }
            else
            {
                BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Validation succeeded!");
                BuildLogger.LogInformation($"[BuildStep] \"{Name}\" - Execution start!");
                try
                {
                    var BuildResult = await ExecuteStep();
                    if (BuildResult.Succeeded)
                        BuildLogger.LogInformation($"[BuildStep] \"{Name}\" - Execution succeeded!");
                    else
                        BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Execution failed! Reason: {BuildResult.ErrorMessage}");

                    return BuildResult;
                }
                catch (Exception ex)
                {
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Execution throws! - {ex}");
                    return BuildStepResult.CreateError(ex.ToString());
                }
            }
        }

        public void BindBuildStepListItemUi(VisualElement buildStepRoot, SerializedProperty serializedProperty)
        {
            isFolded = true; // always fold on creation

            buildStepFrameBinding = new BuildStepEntryFrameBinding(buildStepRoot);
            buildStepFrameBinding.errorCtr.style.display = DisplayStyle.None;
            buildStepFrameBinding.stepCtr.style.display = DisplayStyle.Flex;
            buildStepFrameBinding.enableStepToggle.BindProperty(serializedProperty.FindPropertyRelative(nameof(isEnabled)));

            buildStepFrameBinding.foldoutBtn.clickable = new Clickable(OnStepFoldoutChanged);

            buildStepFrameBinding.buildStepDescription.EnableInClassList(Styles.Hidden, string.IsNullOrEmpty(Description));
            buildStepFrameBinding.buildStepDescription.text = Description;

            var executableState = this.Validate().Status != ValidationStatus.Invalid;
            buildStepFrameBinding.openCodeBtn.clickable = new Clickable(OnOpenCodeButtonClicked);
            buildStepFrameBinding.openCodeBtn.tooltip = "Open the code reference of this step.";
            buildStepFrameBinding.testStepBtn.SetEnabled(IsTestable && executableState);
            buildStepFrameBinding.testStepBtn.tooltip = IsTestable ? "Apply the build step and revert it back!" : "This step does not support testing!";
            buildStepFrameBinding.testStepBtn.clickable = new Clickable(OnTestStepButtonClicked);
            buildStepFrameBinding.applyStepBtn.SetEnabled(executableState);
            buildStepFrameBinding.applyStepBtn.tooltip = buildStepFrameBinding.applyStepBtn.enabledSelf ? "Apply the step and change your current project" : "You have to fix possible errors first!";
            buildStepFrameBinding.applyStepBtn.clickable = new Clickable(OnApplyStepButtonClicked);
            buildStepFrameBinding.emptyPropertiesLabel.style.display = HasProperties ? DisplayStyle.None : DisplayStyle.Flex;
            buildStepFrameBinding.editableStepTitleField.BindProperty(serializedProperty.FindPropertyRelative(nameof(customStepName)));

            UpdateFoldoutState(buildStepFrameBinding);
            UpdateValidationState();

            // create and update build step properties content
            CreateBuildStepContentUi(buildStepFrameBinding.buildStepContent, serializedProperty);
            UpdateUI();
        }

        public virtual void UpdateUI()
        {
            if (buildStepFrameBinding.GetRootElement() == null)
                return;

            buildStepFrameBinding.readonlyStepTitleLabel.text = Name;
            buildStepFrameBinding.editableStepTitleField.value = Name;

            buildStepFrameBinding.parameterPreviewLabel.value = !string.IsNullOrEmpty(FoldedParameterPreviewText) ? FoldedParameterPreviewText : "No Data";

            buildStepFrameBinding.parameterPreviewLabel.SetEnabled(!string.IsNullOrEmpty(FoldedParameterPreviewText));
        }

        protected void OnStepFoldoutChanged()
        {
            isFolded = !isFolded;
            UpdateFoldoutState(buildStepFrameBinding);
        }

        public void UnbindBuildStepListItemUi(VisualElement buildStepRoot, SerializedProperty serializedProperty)
        {
            var buildStepFrameBinding = new BuildStepEntryFrameBinding(buildStepRoot);
            buildStepFrameBinding.foldoutBtn.clickable = null;
            buildStepFrameBinding.openCodeBtn.clickable = null;
            buildStepFrameBinding.testStepBtn.clickable = null;
            buildStepFrameBinding.applyStepBtn.clickable = null;
        }

        private void UpdateFoldoutState(BuildStepEntryFrameBinding binding)
        {
            buildStepFrameBinding.foldoutIcon.EnableInClassList("unity-foldout-folded", isFolded);
            buildStepFrameBinding.foldoutIcon.EnableInClassList("unity-foldout-unfolded", !isFolded);
            binding.foldoutContent.EnableInClassList(Styles.Hidden, IsFolded);

            binding.readonlyStepTitleLabel.EnableInClassList(Styles.Hidden, !IsFolded);
            binding.editableStepTitleField.EnableInClassList(Styles.Hidden, IsFolded);
        }

        public void UpdateValidationState()
        {
            if (buildStepFrameBinding.validationIcon == null)
                return;

            var validationResult = Validate();

            var executableState = validationResult.Status != ValidationStatus.Invalid;
            buildStepFrameBinding.testStepBtn.SetEnabled(IsTestable && executableState);
            buildStepFrameBinding.applyStepBtn.SetEnabled(executableState);
            Styles.AssignValidationStateToTargetIcon(buildStepFrameBinding.validationIcon, validationResult, false);
            buildStepFrameBinding.validationIcon.tooltip = string.IsNullOrEmpty(validationResult.Message) ? "All fine!" : validationResult.Message;
        }

        private void OnOpenCodeButtonClicked()
        {
            string[] guids = AssetDatabase.FindAssets($"{GetType().Name} t:Script");
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var obj = EditorGUIUtility.Load(path);
            AssetDatabase.OpenAsset(obj);
        }

        private async void OnApplyStepButtonClicked()
        {
            try
            {
                BuildLogger.LogInformation($"[BuildStep] \"{Name}\" - Apply started!");
                var testResult = await ExecuteStep();
                if (testResult.Succeeded)
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Apply succeeded!");
                else
                    BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Apply failed! Reason: {testResult.ErrorMessage}!");
            }
            catch (Exception e)
            {
                BuildLogger.LogDebug($"[BuildStep] \"{Name}\" - Apply failed! Reason: {e.Message}!");
            }
        }

        private async void OnTestStepButtonClicked()
        {
            if (!IsTestable) return;

            buildStepFrameBinding.testStepBtn.SetEnabled(false);

            // only use debug log, since this is an editor only feature and unity logging is all you need
            Debug.Log($"[BuildStep] \"{Name}\" - Test started!");
            var result = await ExecuteTestStep();
            if (result.Succeeded)
                Debug.Log($"[BuildStep] \"{Name}\" - Test succeeded!");
            else
                Debug.Log($"[BuildStep] \"{Name}\" - Test failed! Reason {result.ErrorMessage}");

            buildStepFrameBinding.testStepBtn.SetEnabled(true);
        }

        protected VisualElement AddPropertyFieldWithLabel(VisualElement target, SerializedProperty serializedProperty, string propertyName, string label)
        {
            List<VisualElement> childs = new(target.Children());

            if (childs.Find(c => c.name == propertyName) != null)
                return null;
        
            var so = serializedProperty.FindPropertyRelative(propertyName);
            var pf = new PropertyField(so, string.IsNullOrEmpty(label) ? propertyName : label);
            pf.name = propertyName;

            target.Add(pf);
            target.Bind(so.serializedObject);
            return pf;
        }

        protected void AddContentLabel(VisualElement target, string labelText)
        {
            var p = new Label(labelText);
            target.Add(p);
        }
    }
}