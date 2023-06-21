namespace Endava.BuildAndDeploy.Logging
{
    public enum LogLevel
    {
        /// <summary>
        /// Log level, which will allow verbose information, only used within debugging or testing scenarios.
        /// </summary>
        Debug,
        /// <summary>
        /// Log level, which will send information which can be useful for understanding the process itself.
        /// </summary>
        Information,
        /// <summary>
        /// Log level, which you can use to print anything not critical, but unexpected or unwanted.
        /// </summary>
        Warning,
        /// <summary>
        /// Log level, which will print anything critical/unintended and might break the program.
        /// </summary>
        Error,

        None
    }
}
