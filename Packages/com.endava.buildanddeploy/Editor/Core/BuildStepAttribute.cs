using System;

namespace Endava.BuildAndDeploy
{
    /// <summary>
    /// New BuildStep Attribute propagates your custom build step to our buildstep selection with a specific "path".
    /// </summary>
    public class BuildStepAttribute : Attribute
    {
        /// <summary>
        /// Allows you to group every step within the Step-Creation ContextMenu.
        /// This is used once a build step is created and group all steps within a sorted context menu.
        /// </summary>
        /// <example>
        /// BuildStep("Compiler/AddCompilerFlag")
        /// </example>
        public string Path = null;
        public BuildStepAttribute() => Path = null;
        public BuildStepAttribute(string path) => Path = path;
    }
}
