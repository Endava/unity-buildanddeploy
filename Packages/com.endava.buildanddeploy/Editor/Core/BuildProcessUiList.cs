using Endava.BuildAndDeploy.UxmlBindings;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    /// <summary>
    /// Contains all build process UI assets for easy accessability (instead of file lookup)
    /// </summary>
    /// <remarks>
    /// You can create an instance by using a "Window/Build Process/Generate build ui object" (should not needed, since we only need one)
    /// </remarks>
    [Serializable]
    public class BuildProcessUiList : ScriptableObject
    {
        [SerializeField]
        private VisualTreeAsset m_processMainUi;
        public VisualTreeAsset ProcessMainUi => m_processMainUi;

        [SerializeField]
        private VisualTreeAsset m_moduleFrameUi;
        public VisualTreeAsset moduleFrameUi => m_moduleFrameUi;

        [SerializeField]
        private VisualTreeAsset m_mainModuleContentUi;
        public VisualTreeAsset mainModuleContentUi => m_mainModuleContentUi;

        [SerializeField]
        private VisualTreeAsset m_errorContentUi;
        public VisualTreeAsset errorContentUi => m_errorContentUi;

        [SerializeField]
        private VisualTreeAsset m_settingsModule;
        public VisualTreeAsset settingsModule => m_settingsModule;

        [SerializeField]
        private VisualTreeAsset m_scenesModule;
        public VisualTreeAsset scenesModule => m_scenesModule;

        [SerializeField]
        private VisualTreeAsset m_scenesModuleListEntryUi;
        public VisualTreeAsset scenesModuleListEntryUi => m_scenesModuleListEntryUi;

        [SerializeField]
        private VisualTreeAsset m_buildStepsModule;
        public VisualTreeAsset buildStepsModule => m_buildStepsModule;

        [SerializeField]
        private VisualTreeAsset m_buildStepFrame;
        public VisualTreeAsset buildStepFrame => m_buildStepFrame;

        [SerializeField]
        private VisualTreeAsset m_unityBuildModule;
        public VisualTreeAsset UnityBuildModule => m_unityBuildModule;

        [SerializeField]
        private VisualTreeAsset m_buildOverviewUi;
        public VisualTreeAsset buildOverviewUi => m_buildOverviewUi;

        [SerializeField]
        private VisualTreeAsset m_buildOverviewProcessEntryUi;
        public VisualTreeAsset buildOverviewProcessEntryUi => m_buildOverviewProcessEntryUi;


        [SerializeField]
        private StyleSheet m_commonStyles;
        public StyleSheet commonStyles => m_commonStyles;

        public VisualElement CreateErrorContent(string errorMessage = "An error occured")
        {
            if (m_errorContentUi != null)
            {
                var error = m_errorContentUi.Instantiate();
                var errorBinding = new ErrorBinding(error);
                errorBinding.errorLabel.text = errorMessage;
                return error;
            }
            else
            {
                var result = new VisualElement();
                result.style.alignItems = Align.Center;
                result.style.justifyContent = Justify.Center;
                var textLabel = new Label();
                textLabel.text = errorMessage;
                textLabel.style.color = Color.red;
                result.Add(textLabel);
                return result;
            }
        }
    }
}