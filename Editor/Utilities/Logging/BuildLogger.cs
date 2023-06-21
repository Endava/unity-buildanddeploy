namespace Endava.BuildAndDeploy.Logging
{
    public static class BuildLogger
    {
        public static LogTargets LogTargets { get; set; } = LogTargets.Everything;
        public static LogLevel LogLevel { get; set; } = LogLevel.Debug;

        private static LogBase _unityLogger = new UnityLogger();
        private static LogBase _fileLogger = new FileLogger();

        public static void LogDebug(object message) => Log(LogLevel.Debug, message?.ToString());
        public static void LogInformation(object message) => Log(LogLevel.Information, message?.ToString());
        public static void LogWarning(object message) => Log(LogLevel.Warning, message?.ToString());
        public static void LogError(object message) => Log(LogLevel.Error, message?.ToString());

        public static void Log(LogLevel level, string message)
        {
            if (level == LogLevel.None) return;

            if (level >= LogLevel)
            {
                if ((LogTargets & LogTargets.Unity) == LogTargets.Unity)
                {
                    _unityLogger.Log(level, message);
                }

                if ((LogTargets & LogTargets.File) == LogTargets.File)
                {
                    _fileLogger.Log(level, message);
                }
            }
        }
    }
}
