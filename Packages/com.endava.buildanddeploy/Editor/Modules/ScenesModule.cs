using Endava.BuildAndDeploy.Logging;
using Endava.BuildAndDeploy.UxmlBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    [BuildModule(BuildExecutionMode.BeforeBuild, 0)]
    public class ScenesModule : BuildProcessModule
    {
        public override string SectionName => "Scenes";
        public override bool CanBeDisabled => true;
        public override string HelpIconTooltip => UseScenesInBuildSettings 
            ? "Using the configured Build Settings scenes, since module is disabled." 
            : "Build Settings scenes will be overwritten by the module scene list entries.";

        public bool UseScenesInBuildSettings => !IsEnabled;

        [SerializeField]
        private List<SceneAsset> sceneAssets = new List<SceneAsset>();
        public List<SceneAsset> SceneAssets => sceneAssets;

        public ScenesModuleBinding scenesModuleBinding { get; set; }

        public override Task<bool> Execute()
        {
            // we overwrite the scenes to make the scenes used obvious in the build settings as well
            if (!OverwriteScenes())
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        public override BuildPlayerOptions OnChangeBuildPlayerOptions(BuildPlayerOptions input)
        {
            if (UseScenesInBuildSettings)
                input.scenes = Helpers.CreateSceneListFromBuildSettings();
            else
                input.scenes = ConvertedSceneListForBuild();

            return input;
        }

        public string[] ConvertedSceneListForBuild()
        {
            if (UseScenesInBuildSettings) return Helpers.CreateSceneListFromBuildSettings();

            if (SceneAssets == null) return null;

            var result = new List<string>(SceneAssets.Count);
            for (var i = 0; i < SceneAssets.Count; ++i)
            {
                var scene = SceneAssets[i];
                result.Insert(i, scene != null ? AssetDatabase.GetAssetPath(scene) : null);
            }

            return result.ToArray();
        }

        private bool OverwriteScenes()
        {
            if (SceneAssets == null || SceneAssets.Count == 0)
            {
                BuildLogger.LogError(ConstLogs.ErrorSceneAssets);
                return false;
            }

            var scenesAsString = ConvertedSceneListForBuild();
            var scenes = new EditorBuildSettingsScene[scenesAsString.Length];
            for (int i = 0; i < scenesAsString.Length; i++)
            {
                var ebss = new EditorBuildSettingsScene(scenesAsString[i], true);
                scenes[i] = ebss;
            }
            EditorBuildSettings.scenes = scenes;

            var scenesStringPathListing = string.Empty;
            for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                scenesStringPathListing += $"\t{i}: {EditorBuildSettings.scenes[i].path}\n";
            }

            BuildLogger.LogDebug($"EditorBuildSettings changed to:\n{scenesStringPathListing}");
            BuildLogger.LogDebug(ConstLogs.DebugSceneAssets);

            return true;
        }

        public override VisualElement CreateContentUI(SerializedProperty serializedProperty, BuildProcessUiList editorUiList, Editor editor)
        {
            var moduleUi = editorUiList.scenesModule.Instantiate();
            scenesModuleBinding = new ScenesModuleBinding(moduleUi);

            scenesModuleBinding.scenesList.makeItem = () => editorUiList.scenesModuleListEntryUi.Instantiate();
            scenesModuleBinding.scenesList.bindItem = (view, index) => 
            {
                if (sceneAssets.Count <= index) return;

                var entryView = new ScenesModuleListItemBinding(view);
                var sceneList = serializedProperty.FindPropertyRelative(nameof(sceneAssets));
                entryView.sceneObjectField.BindProperty(sceneList.GetArrayElementAtIndex(index));
            };
            scenesModuleBinding.scenesList.BindProperty(serializedProperty.FindPropertyRelative(nameof(sceneAssets)));
            scenesModuleBinding.optionsBtnLabel.text = "Options";
            scenesModuleBinding.optionsBtn.clicked += OnSceneOptionsBtnClicked;

            UpdateContentUI();

            return moduleUi;
        }

        private void OnSceneOptionsBtnClicked()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Open Scene"), false, AddOpenScenesBtn_clicked);
            menu.AddItem(new GUIContent("Copy Scenes From Settings"), false, CopyScenesFromBuildSettingsBtn_clicked);
            menu.AddItem(new GUIContent("Clear All Scenes"), false, ClearAllBtn_clicked);

            menu.ShowAsContext();
        }

        private void CopyScenesFromBuildSettingsBtn_clicked()
        {
            SceneAssets.Clear();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if (scene == null) continue;
                if (string.IsNullOrEmpty(scene.path)) continue;

                SceneAssets.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path));
            }
        }

        private void AddOpenScenesBtn_clicked()
        {
            if (SceneManager.GetActiveScene() != null &&
                    !string.IsNullOrEmpty(SceneManager.GetActiveScene().path))
            {
                var path = SceneManager.GetActiveScene().path;
                var sceneObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (sceneObject != null &&
                    !SceneAssets.Contains(sceneObject))
                {
                    SceneAssets.Add(sceneObject);
                }
                else
                {
                    Debug.LogError(ConstLogs.ErrorSceneAsset);
                }
            }
            else
            {
                Debug.LogError(ConstLogs.ErrorSceneAssetCannotBeAdded);
            }
        }

        private void ClearAllBtn_clicked()
        {
            SceneAssets.Clear();
        }

        public override BuildValidation Validate()
        {
            // using build settings scenes with no scenes in build settings configured
            if(UseScenesInBuildSettings && EditorBuildSettings.scenes.Length <= 0) 
                return BuildValidation.CreateIssued(ConstValidations.InvalidSceneModuleEmptyBuildSettings);

            // using build settings scenes with a null scene object inside
            if (UseScenesInBuildSettings && EditorBuildSettings.scenes.Any(x => x == null))
                return BuildValidation.CreateInvalid(ConstValidations.InvalidSceneModuleBuildSettingsNull);

            // overwrite the build setting settings within this module
            if(!UseScenesInBuildSettings)
            {
                // at least one scene needs to be added
                if (SceneAssets == null || SceneAssets.Count == 0)
                    return BuildValidation.CreateInvalid(ConstValidations.InvalidSceneModuleScenesNull);

                // no null scene objects should exist
                if (SceneAssets.Any(x => x == null))
                    return BuildValidation.CreateInvalid(ConstValidations.InvalidSceneModuleScenesNotExisiting);
            }

            return BuildValidation.Valid;
        }
    }
}