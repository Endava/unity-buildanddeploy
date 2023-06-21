using System;

namespace Endava.BuildAndDeploy
{
    /// <summary>
    /// This attribute allows the Build Process to "find" existing build modules within your code.
    /// </summary>
    public class BuildModuleAttribute : Attribute
    {
        /// <summary>
        /// Defines at which execution order this module is positioned.
        /// </summary>
        public BuildExecutionMode ExecutionMode = BuildExecutionMode.Setup;
        /// <summary>
        /// Allows you to order similar ExecutionMode modules.
        /// </summary>
        public int Order = 0;

        public BuildModuleAttribute() : this(BuildExecutionMode.Setup, 0) { }
        public BuildModuleAttribute(BuildExecutionMode executionMode) : this(executionMode, 0) { }
        public BuildModuleAttribute(BuildExecutionMode executionMode, int order)
        {
            ExecutionMode = executionMode;
            Order = order;
        }
    }
}
