using Endava.BuildAndDeploy.UxmlBindings;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    [BuildModule(BuildExecutionMode.Setup, 1)]
    public class SettingsModule : BuildProcessModule
    {
        public override string SectionName => "Platform Settings";
        public override bool CanBeDisabled => true;
        public override string HelpIconTooltip => !IsEnabled
            ? "Enable the Platform settings to overwrite platform specific build options."
            : "Build Settings platform settings will be overwritten by the module settings.";

        [SerializeReference]
        public List<BuildOptionsBase> buildOption = new();

        public BuildOptionsBase BuildOption => buildOption.Count > 0 ? buildOption[0] : null;

        public override Task<bool> Execute()
        {
            if(IsEnabled)
                BuildOption.Execute();

            return Task.FromResult(true);
        }

        public override BuildPlayerOptions OnChangeBuildPlayerOptions(BuildPlayerOptions input)
        {
            if (BuildOption != null)
                input.options = BuildOption.GetBuildOptions(input.options);

            return input;
        }

        public override VisualElement CreateContentUI(SerializedProperty serializedProperty, BuildProcessUiList editorUiList, Editor editor)
        {
            UpdateBuildOptions(serializedProperty);
            var moduleUI = editorUiList.settingsModule.Instantiate();
            var bindingUI = new SettingsModuleBinding(moduleUI);
            var prop = serializedProperty.FindPropertyRelative(nameof(buildOption)).GetArrayElementAtIndex(0);

            BuildOption.CreateUI(prop, bindingUI.GetRootElement());

            return bindingUI.GetRootElement();
        }

        private void UpdateBuildOptions(SerializedProperty serializedProperty)
        {
            var group = Helpers.GroupFromBuildTargetWithoutObsolete(Process.Main.Target);
            var types = new List<System.Type>(Helpers.GetAllSubTypes(typeof(BuildOptionsBase), true));
            var type = types.Find(t => t.Name.Contains($"BuildOptions{group}"));

            type ??= types.Find(t => t.Name.Contains($"BuildOptionsBase"));

            if (BuildOption?.GetType() != type)
            {
                buildOption.Clear();
                buildOption.Add((BuildOptionsBase)System.Activator.CreateInstance(type));
                serializedProperty.serializedObject.Update();
            }
        }

        public override BuildValidation Validate() => BuildValidation.Valid;
    }
}