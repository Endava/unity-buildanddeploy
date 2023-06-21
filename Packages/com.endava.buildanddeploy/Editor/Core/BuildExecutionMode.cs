using System;

namespace Endava.BuildAndDeploy
{
    /// <summary>
    /// BuildExecutionMode allows you to specify the order of your BuildModule execution
    /// </summary>
    public enum BuildExecutionMode
    {
        /// <summary>
        /// Setup is done before anything else gets executes (like the MainModule or the Settings)
        /// </summary>
        Setup = 0,
        /// <summary>
        /// Executed after the setup, but before the actual Unity Build starts
        /// </summary>
        BeforeBuild = 1,
        /// <summary>
        /// This is reserverd for the actual Unity Build Module and starts the Unity Build.
        /// </summary>
        Build = 2,
        /// <summary>
        /// Executed at the end of the whole Build Process.
        /// </summary>
        AfterBuild = 4,
    }
}
