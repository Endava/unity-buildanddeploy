using Endava.BuildAndDeploy.Logging;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    [BuildModule(BuildExecutionMode.Build, 0)]
    public class UnityBuildModule : BuildProcessModule
    {
        public override string SectionName => "Start Unity Build";
        public override bool CanBeDisabled => true;
        protected override bool CanFoldout => false;
        public override string HelpIconTooltip => "This module starts the actual Unity Build process (like you press Build in the Build Settings)";

        public override Task<bool> Execute()
        {
            BuildLogger.LogInformation("---------- Executing Unity Build! ----------");
            var report = BuildPipeline.BuildPlayer(Process.CreateBuildPlayerOptionsFromModules());
            if (report.summary.result != BuildResult.Succeeded)
            {
                BuildLogger.LogError($"Build stopped after failing in BuildPipeline.BuildPlayer (Result={report.summary.result}).\nErrors: {report.summary.totalErrors}\nWarnings: {report.summary.totalWarnings}");
                return Task.FromResult(false);
            }
            else
            {
                BuildLogger.LogInformation($"BuildPipeline.BuildPlayer succeeded!\nTime: {report.summary.totalTime}\nSize: {report.summary.totalSize}\nWarnings: {report.summary.totalWarnings}");
                return Task.FromResult(true);
            }
        }

        public override VisualElement CreateContentUI(SerializedProperty serializedProperty, BuildProcessUiList editorUiList, Editor editor)
        {
            return new VisualElement(); // no need to render anything in here, just show headline and make it toggleable
        }

        protected override void UpdateHelpIcon()
        {
            base.UpdateHelpIcon();

            frameBinding.helpIcon.style.unityBackgroundImageTintColor = Color.white;
        }

        public override BuildValidation Validate() => BuildValidation.Valid;
    }
}