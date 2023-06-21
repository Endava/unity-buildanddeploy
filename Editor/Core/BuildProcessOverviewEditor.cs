using Endava.BuildAndDeploy.UxmlBindings;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    public class BuildProcessOverviewEditor : EditorWindow
    {
        [SerializeField]
        private BuildProcessUiList uiList;

        private BuildProcess[] buildProcessesInProject;
        private BuildOverviewBinding uiBinding;
        private string searchString;

        public void CreateGUI()
        {
            buildProcessesInProject = FindAllBuildProcessObjects();
            searchString = string.Empty;

            var overviewUi = uiList.buildOverviewUi.Instantiate();
            uiBinding = new BuildOverviewBinding(overviewUi);

            uiBinding.processList.makeItem = OnMakeBuildProcessItem;
            uiBinding.processList.bindItem = OnBindBuildProcessItem;
            uiBinding.processList.unbindItem = OnUnbindBuildProcessItem;
            uiBinding.processList.itemsSource = buildProcessesInProject;

            if (buildProcessesInProject != null)
            {
                uiBinding.processList.style.display = buildProcessesInProject.Length > 0 ? DisplayStyle.Flex : DisplayStyle.None;
                uiBinding.ctrempty.style.display = buildProcessesInProject.Length == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            }
            else
            {
                uiBinding.processList.style.display = DisplayStyle.None;
                uiBinding.ctrempty.style.display = DisplayStyle.Flex;
            }

            uiBinding.searchfield.value = searchString;
            uiBinding.searchfield.RegisterValueChangedCallback(OnSearchFieldValueChanged);

            Helpers.AssignInBuildEditorUnityTextureToVisualElement(uiBinding.refreshBtnIcon, "Refresh@2x");
            uiBinding.refreshBtn.clicked += OnRefreshListBtnClicked;
            uiBinding.refreshBtn.tooltip = "Refresh the process list";

            rootVisualElement.Add(overviewUi);
        }

        private void OnRefreshListBtnClicked()
        {
            buildProcessesInProject = FindAllBuildProcessObjects();

            UpdateList();
        }

        private void OnSearchFieldValueChanged(ChangeEvent<string> evt)
        {
            if (buildProcessesInProject == null) return;

            searchString = evt.newValue;
            UpdateList();
        }

        private void UpdateList()
        {
            BuildProcess[] filteredProcesses = string.IsNullOrEmpty(searchString)
                ? buildProcessesInProject
                : buildProcessesInProject
                    .Where(x => x.Name.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    .ToArray();

            uiBinding.processList.itemsSource = filteredProcesses;

            uiBinding.processList.style.display = filteredProcesses.Length != 0 ? DisplayStyle.Flex : DisplayStyle.None;
            uiBinding.ctrempty.style.display = filteredProcesses.Length == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private VisualElement OnMakeBuildProcessItem()
        {
            return uiList.buildOverviewProcessEntryUi.Instantiate();
        }

        private void OnBindBuildProcessItem(VisualElement view, int index)
        {
            var itemBinding = new BuildOverviewListItemBinding(view);

            var process = buildProcessesInProject[index];
            var platformIcon = Styles.GetPlatformIconName(process.Main.Target);
            if (!string.IsNullOrEmpty(platformIcon))
                Helpers.AssignInBuildEditorUnityTextureToVisualElement(itemBinding.icon, platformIcon);
            else
                Helpers.AssignInBuildEditorUnityTextureToVisualElement(itemBinding.icon, "console.warnicon");

            itemBinding.name.text = process.Name;
            var buildState = process.IsBuildable();
            Styles.AssignValidationStateToTargetIcon(itemBinding.stateIcon, buildState, false);
            itemBinding.GoToBtn.clickable = new Clickable(() => { Selection.activeObject = process; });
            itemBinding.BuildBtn.clickable = new Clickable(async () => { await process.TryBuildFromEditor(); });
        }

        private void OnUnbindBuildProcessItem(VisualElement view, int index)
        {
            var itemBinding = new BuildOverviewListItemBinding(view);
            itemBinding.GoToBtn.clickable = null;
            itemBinding.BuildBtn.clickable = null;
        }

        private static BuildProcess[] FindAllBuildProcessObjects()
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(BuildProcess).Name);
            var result = new BuildProcess[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                result[i] = AssetDatabase.LoadAssetAtPath<BuildProcess>(path);
            }

            return result;
        }

        [MenuItem("Window/Build and Deploy/Builds Process Overview")]
        public static void ShowBuildProcessesOverviewWindow()
        {
            // This method is called when the user selects the menu item in the Editor
            BuildProcessOverviewEditor wnd = GetWindow<BuildProcessOverviewEditor>();
            wnd.titleContent = new GUIContent("Build Process Overview");
        }
    }
}