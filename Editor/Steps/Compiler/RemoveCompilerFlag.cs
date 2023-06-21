using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;

namespace Endava.BuildAndDeploy.BuildSteps
{
    /// <summary>
    /// This BuildStep removes a single compiler flags to the given platform player settings.
    /// </summary>
    [Serializable, BuildStep("Compiler/RemoveCompilerFlag")]
    public class RemoveCompilerFlag : BaseCompilerFlag
    {
        public override string Description => "Removes a compiler flag from Unity's ScriptingDefineSymbols.";

        protected override Task<BuildStepResult> ExecuteStep()
        {
            try
            {
                var list = GetCompilerFlagsList(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup));

                if (list.Contains(CompilerFlags))
                {
                    list.Remove(CompilerFlags);
                    BuildLogger.LogDebug($"Compilerflags before {PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)}");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, GetCompilerFlagsString(list));
                    BuildLogger.LogDebug($"Removed Compilerflag {CompilerFlags}");
                    BuildLogger.LogDebug($"Compilerflags after {PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)}");
                }
                else
                {
                    BuildLogger.LogWarning($"Skipping Compilerflag \"{CompilerFlags}\" because it doesn't exists {PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)}!");
                }

                return Task.FromResult(BuildStepResult.Successfull);
            }
            catch (Exception ex)
            {
                return Task.FromResult(BuildStepResult.CreateError(ex.ToString()));
            }
        }
    }
}