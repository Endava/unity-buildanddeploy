using System;

namespace Endava.BuildAndDeploy.Logging
{
    public abstract class LogBase
    {
        protected abstract void LogInternally(LogLevel level, string message);

        public void Log(LogLevel level, string message)
        {
            LogInternally(level, GenerateLog(level, message));
        }

        protected virtual string GenerateLog(LogLevel level, string message)
        {
            // TRACE - 12.12.2012:14.50.12: ..message..
            return $"[{level.ToString().ToUpper()} - {DateTime.Now.ToString()}] {message}";
        }

    }
}
