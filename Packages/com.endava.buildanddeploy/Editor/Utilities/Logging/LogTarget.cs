using System;

namespace Endava.BuildAndDeploy.Logging
{
    [Flags]
    public enum LogTargets
    {
        None = 0,

        Unity = 1 << 0,
        File = 1 << 1,

        Everything = Unity | File,
    }
}
