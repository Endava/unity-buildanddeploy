using UnityEngine;

namespace Endava.BuildAndDeploy.Logging
{
    public class UnityLogger : LogBase
    {
        protected override void LogInternally(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Error: Debug.LogError(message); break;
                case LogLevel.Warning: Debug.LogWarning(message); break;
                default: Debug.Log(message); break;
            }
        }
    }
}
