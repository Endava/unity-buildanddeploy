using Endava.BuildAndDeploy.Logging;
using System;
using System.Text;

namespace Endava.BuildAndDeploy
{
    /// <summary>
    /// Parser class, which is used to parse the command line arguments and filter possible build setup properties.
    /// </summary>
    public class CommandLineParser
    {
        internal static readonly string BuildTargetKey = "-buildTarget";
        internal static readonly string AndroidSdkKey = "-android-sdk=";
        internal static readonly string AndroidNdkKey = "-android-ndk=";

        internal static readonly string BuildProcessKey = "-buildProcess=";
        internal static readonly string OverrideLogTargetsKey = "-overrideLogTargets=";
        internal static readonly string OverrideLogLevelKey = "-overrideLogLevel=";
        internal static readonly string OverrideFullOutputPathKey = "-overrideFullOutputPath=";


        public static string ExecuteMethod
        {
            get { return $"{nameof(Endava)}.{nameof(BuildAndDeploy)}.{nameof(BuildFromCommandLine)}.{nameof(BuildFromCommandLine.Build)}"; }
        }

        public bool HasBuildTargetSet { get; private set; } = false;
        public string TargetProcessPath { get; private set; } = null;
        public string AndroidSdkPath { get; private set; } = null;
        public string AndroidNdkPath { get; private set; } = null;
        public LogTargets? OverrideLogTargets { get; private set; } = null;
        public LogLevel? OverrideLogLevel { get; private set; } = null;
        public string OverrideFullOutputPath { get; private set; } = null;
        public string[] Arguments{ get; private set; } = null;

        /// <summary>
        /// Parse the passed arguments and creates a CommandLineParser from it.
        /// </summary>
        /// <param name="arguments">The commandline arguments array you want to check.</param>
        /// <param name="result">The result of the argument parsing, can be null.</param>
        /// <returns>Returns true, if the parser succeeded without errors, otherwise false.</returns>
        public static bool TryParseArguments(string[] arguments, out CommandLineParser result)
        {
            if (arguments == null)
            {
                result = new CommandLineParser();
                return true;
            }

            try
            {
                result = new CommandLineParser();
                result.Arguments = arguments;
                foreach (var s in arguments)
                {
                    if (s.StartsWith(BuildTargetKey))
                        result.HasBuildTargetSet = true;
                    else if (s.StartsWith(BuildProcessKey, StringComparison.InvariantCultureIgnoreCase))
                        result.TargetProcessPath = s.Substring(BuildProcessKey.Length).Trim();
                    else if (s.StartsWith(AndroidSdkKey, StringComparison.InvariantCultureIgnoreCase))
                        result.AndroidSdkPath = s.Substring(AndroidSdkKey.Length).Trim();
                    else if (s.StartsWith(AndroidNdkKey, StringComparison.InvariantCultureIgnoreCase))
                        result.AndroidNdkPath = s.Substring(AndroidNdkKey.Length).Trim();
                    else if (s.StartsWith(OverrideLogLevelKey, StringComparison.InvariantCultureIgnoreCase))
                        result.OverrideLogLevel = CreateLogLevelFromString(s.Substring(OverrideLogLevelKey.Length));
                    else if (s.StartsWith(OverrideLogTargetsKey, StringComparison.InvariantCultureIgnoreCase))
                        result.OverrideLogTargets = CreateLogTargetsFromString(s.Substring(OverrideLogTargetsKey.Length));
                    else if (s.StartsWith(OverrideFullOutputPathKey, StringComparison.InvariantCultureIgnoreCase))
                        result.OverrideFullOutputPath = s.Substring(OverrideFullOutputPathKey.Length).Trim();
                }
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private static LogLevel? CreateLogLevelFromString(string logLevel)
        {
            if (string.IsNullOrEmpty(logLevel)) return null;

            if (Enum.TryParse(logLevel, true, out LogLevel result))
                return result;
            else
                return null;
        }

        private static LogTargets? CreateLogTargetsFromString(string logTargets)
        {
            if (string.IsNullOrEmpty(logTargets)) return null;

            LogTargets result = LogTargets.None;
            try
            {
                var targets = logTargets.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var target in targets)
                {
                    foreach (LogTargets value in Enum.GetValues(typeof(LogTargets)))
                    {
                        if (target.Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            result |= value;
                            break;
                        }
                    }
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        public static string ListArgumentsAsStrings()
        {
            return
                $"To create a build from command line use the following additional argument:\n\n" +
                $"Unity.exe -executeMethod {ExecuteMethod} <additionalArguments>\n\n" +
                $"Beside the known Unity Command Line Arguments the following additional arguments are available:\n" +
                $"{BuildProcessKey}<path> [required*]\nThe path to a single build process asset\n\n" +
                $"{OverrideLogTargetsKey}File,Unity [optional]\nAllows you to override the log targets as comma seperated list\n {string.Join(",", Enum.GetNames(typeof(LogTargets)))}\n\n" +
                $"{OverrideLogLevelKey}Warning [optional]\nAllows you to override the log level\n{string.Join(",", Enum.GetNames(typeof(LogLevel)))}\n\n" +
                $"{OverrideFullOutputPathKey}<path> [optional]\nAllows you to override the build output path of the build process\n\n" +
                $"{AndroidSdkKey}<sdk-path> [optional - Android Only] \nAllows you to set the android sdk path\n\n" +
                $"{AndroidNdkKey}<ndk-path> [optional - Android Only] \nAllows you to set the android ndk path\n\n";
        }

        public override string ToString()
        {
            StringBuilder __sb = new StringBuilder("CommandLineParser(");

            __sb.Append(nameof(HasBuildTargetSet)); __sb.Append(": "); __sb.Append(HasBuildTargetSet);
            __sb.Append(", ");

            if (TargetProcessPath != null)
            {
                __sb.Append(nameof(TargetProcessPath)); __sb.Append(": "); __sb.Append(TargetProcessPath);
                __sb.Append(", ");
            }
            
            if (OverrideLogTargets != null)
            {
                __sb.Append(nameof(OverrideLogTargets)); __sb.Append(": "); __sb.Append(OverrideLogTargets.Value);
                __sb.Append(", ");
            }

            if (OverrideLogLevel != null)
            {
                __sb.Append(nameof(OverrideLogLevel)); __sb.Append(": "); __sb.Append(OverrideLogLevel.Value);
                __sb.Append(", ");
            }

            if (OverrideFullOutputPath != null)
            {
                __sb.Append(nameof(OverrideFullOutputPath)); __sb.Append(": "); __sb.Append(OverrideFullOutputPath);
                __sb.Append(", ");
            }

            if (AndroidSdkPath != null)
            {
                __sb.Append(nameof(AndroidSdkPath)); __sb.Append(": "); __sb.Append(AndroidSdkPath);
                __sb.Append(", ");
            }

            if (AndroidNdkPath != null)
            {
                __sb.Append(nameof(AndroidNdkPath)); __sb.Append(": "); __sb.Append(AndroidNdkPath);
                __sb.Append(", ");
            }
            if (Arguments != null)
            {
                __sb.Append(nameof(Arguments)); __sb.Append(": "); __sb.Append(string.Join(", ", Arguments));
                __sb.Append(", ");
            }
            __sb.Append(")");
            return __sb.ToString();
        }
    }
}