using Endava.BuildAndDeploy.UxmlBindings;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    [Serializable]
    public abstract class BuildProcessModule
    {
        /// <summary>
        /// Specify the name of your module (headline of your module).
        /// </summary>
        public abstract string SectionName { get; }
        /// <summary>
        /// If true, a toggle will appear next to the headline, which allows the module to be disabled by the user.
        /// </summary>
        public abstract bool CanBeDisabled { get; }
        /// <summary>
        /// You can prevent the module to have foldout content (only headline).
        /// </summary>
        protected virtual bool CanFoldout => true;

        /// <summary>
        /// You can assign a custom icon next to the headline. (See: https://github.com/halak/unity-editor-icons)
        /// </summary>
        public virtual string ModuleIconId { get; } = string.Empty;
        /// <summary>
        /// Should a help icon be shown at the end of the headline.
        /// </summary>
        public virtual bool HideHelpIcon => false;
        /// <summary>
        /// You can display display some tooltip text, once the user hovers over the help icon.
        /// </summary>
        public virtual string HelpIconTooltip { get; } = string.Empty;

        [SerializeField]
        protected bool isEnabled = true;
        public bool IsEnabled { get => !CanBeDisabled || isEnabled; protected set => isEnabled = value; } // if it can't be disabled, its always enabled.

        [SerializeField]
        protected bool isFolded = true;
        public bool IsFolded { get => CanFoldout ? isFolded : true; protected set => isFolded = value; } // if it can't be folded, always return true;

        [SerializeField]
        protected BuildProcess process = null;
        public BuildProcess Process { get => process; set => process = value; }

        protected ModuleFrameBinding frameBinding;

        public abstract Task<bool> Execute();
        public abstract BuildValidation Validate();

        /// <summary>
        /// This method updates the state of the validation icon, once the state changed.
        /// </summary>
        public virtual void UpdateValidationUI(BuildValidation validation)
        {
            if (frameBinding.validationIcon == null)
                return;

            frameBinding.validationIcon.tooltip = validation.Message;
            Styles.AssignValidationStateToTargetIcon(frameBinding.validationIcon, validation);
        }

        /// <summary>
        /// Use this to handle module creation (only done once on construction)
        /// </summary>
        public virtual void OnCreate() { }
        /// <summary>
        /// Once the module is enabled in the inspector, this method gets called.
        /// </summary>
        public virtual void OnInititalize() { }

        /// <summary>
        /// You can override this method to edit the BuildPlayerOptions, which are used within the Unity Build.
        /// </summary>
        /// <param name="input">The BuildPlayerOptions which each of the modules can edit.</param>
        /// <returns>Return your changed BuildPlayerOptions, if required.</returns>
        public virtual BuildPlayerOptions OnChangeBuildPlayerOptions(BuildPlayerOptions input) => input;

        public void CreateInspectorUI(VisualElement root, SerializedProperty serializedProperty, BuildProcessUiList editorUiList, Editor editor)
        {
            if (editorUiList.moduleFrameUi == null)
            {
                root.Add(editorUiList.CreateErrorContent());
            }
            else
            {
                var moduleFrame = editorUiList.moduleFrameUi.Instantiate();
                frameBinding = new ModuleFrameBinding(moduleFrame);

                // renaming the root container of every module to make easier accessability (the frameBinding should still work since it was parsed in first place)
                frameBinding.ctr_root.name = $"{this.GetType().Name}_module";
                frameBinding.foldout_module.text = string.Empty;
                frameBinding.foldout_module.RegisterValueChangedCallback(evt =>
                {
                    if (evt.target == frameBinding.foldout_module)
                        IsFolded = !evt.newValue;
                });

                if(!CanFoldout)
                {
                    frameBinding.foldout_module.style.visibility = Visibility.Hidden;
                }
                
                frameBinding.enabled.BindProperty(serializedProperty.FindPropertyRelative(nameof(isEnabled)));
                var moduleContent = CreateContentUI(serializedProperty, editorUiList, editor);
                if (moduleContent != null)
                    frameBinding.moduleContent.Add(moduleContent);
                else
                    frameBinding.moduleContent.Add(editorUiList.CreateErrorContent("module content is null!"));

                if (!string.IsNullOrEmpty(ModuleIconId))
                {
                    Helpers.AssignInBuildEditorUnityTextureToVisualElement(frameBinding.moduleIcon, ModuleIconId);
                }

                root.Add(moduleFrame);

                UpdateInspectorUI();
            }
        }

        public void UpdateInspectorUI()
        {
            frameBinding.foldout_module.value = !IsFolded;
            frameBinding.headlineLabel.text = SectionName;
            frameBinding.enabled.style.display = CanBeDisabled ? DisplayStyle.Flex : DisplayStyle.None;
            frameBinding.moduleIcon.style.display = !string.IsNullOrEmpty(ModuleIconId) ? DisplayStyle.Flex : DisplayStyle.None;
            frameBinding.moduleContent.SetEnabled(isEnabled);

            UpdateContentUI();
            UpdateHelpIcon();
            UpdateValidationUI(this.Validate());
        }

        protected virtual void UpdateHelpIcon()
        {
            frameBinding.helpIcon.EnableInClassList("hidden", HideHelpIcon);
            if (!HideHelpIcon)
            {
                // the help icon (if not hidden) is semi alpha if no text specified
                //frameBinding.helpIcon.style.unityBackgroundImageTintColor = !IsEnabled ? Color.white : new Color(1, 1, 1, 0.5f);
                frameBinding.helpIcon.tooltip = HelpIconTooltip;
            }
        }

        public abstract VisualElement CreateContentUI(SerializedProperty serializedProperty, BuildProcessUiList editorUiList, Editor editor);

        public virtual void UpdateContentUI() { }
    }
}