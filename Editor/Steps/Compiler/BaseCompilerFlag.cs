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
    public abstract class BaseCompilerFlag : TestableBuildStep<string>
    {
        [SerializeField]
        protected string CompilerFlags = string.Empty;
        public override string FoldedParameterPreviewText => CompilerFlags;

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(CompilerFlags))
                return BuildValidation.CreateInvalid("CompilerFlag(s) cannot be null or empty!");

            return BuildValidation.Valid;
        }

        protected static List<string> GetCompilerFlagsList(string flags) => new List<string>(flags.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
        protected static string GetCompilerFlagsString(List<string> flags) => String.Join(";", flags);

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(CompilerFlags), "Compiler Flag");
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