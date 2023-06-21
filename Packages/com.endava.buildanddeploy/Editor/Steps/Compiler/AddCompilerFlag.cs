using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;

namespace Endava.BuildAndDeploy.BuildSteps
{
    /// <summary>
    /// This BuildStep adds a single compiler flags to the given platform player settings.
    /// </summary>
    [Serializable, BuildStep("Compiler/AddCompilerFlag")]
    public class AddCompilerFlag : BaseCompilerFlag
    {
        public override string Description => "Adds a compiler flag to Unity's ScriptingDefineSymbols.";

        protected override Task<BuildStepResult> ExecuteStep()
        {
            try
            {
                var list = GetCompilerFlagsList(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup));

                if (!list.Contains(CompilerFlags))
                {
                    list.Add(CompilerFlags);
                    BuildLogger.LogDebug($"Compilerflags before {PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)}");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, GetCompilerFlagsString(list));
                    BuildLogger.LogDebug($"Added Compilerflag {CompilerFlags}");
                    BuildLogger.LogDebug($"Compilerflags after {PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)}");
                }
                else
                {
                    BuildLogger.LogWarning($"Skipping Compilerflag \"{CompilerFlags}\" because it is already existing {PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)}!");
                }

                return Task.FromResult(BuildStepResult.Successfull);
            }
            catch(Exception ex)
            {
                return Task.FromResult(BuildStepResult.CreateError(ex.ToString()));
            }
        }
    }
}