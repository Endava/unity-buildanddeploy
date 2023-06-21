using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    /// <summary>
    /// This BuildStep clears/removes all compiler flags of the given platform player settings.
    /// </summary>
    [Serializable, BuildStep("Compiler/ClearCompilerFlags")]

    public class ClearCompilerFlags : TestableBuildStep<string>
    {
        public override string Description => "Removes all compiler flag from your Unity's ScriptingDefineSymbols.";
        public override bool HasProperties => false;

        protected override Task<BuildStepResult> ExecuteStep()
        {
            try
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Empty);

                BuildLogger.LogDebug($"Removed all Compiler flags! Compiler flags = {PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)}");

                return Task.FromResult(BuildStepResult.Successfull);
            }
            catch (Exception ex)
            {
                return Task.FromResult(BuildStepResult.CreateError(ex.ToString()));
            }
        }

        public override BuildValidation Validate()
        {
            return BuildValidation.Valid;
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {

        }

        protected override Task<string> BeforeTestExecution()
        {
            var beforeState = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            Debug.Log($"Before Test State: {beforeState}");
            return Task.FromResult(beforeState);
        }

        protected override Task AfterTestExecution(string beforeState)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, beforeState);
            Debug.Log($"After Test State: {PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)}");
            return Task.CompletedTask;
        }
    }
}
