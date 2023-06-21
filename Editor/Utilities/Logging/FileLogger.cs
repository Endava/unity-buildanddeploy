using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Endava.BuildAndDeploy.Logging
{
    public class FileLogger : LogBase
    {
        public static string LogFilePath = Path.Combine(Application.dataPath, "..", "Logs", "buildLog.log");

        protected readonly object lockObj = new object();

        public static bool CleanLogFile()
        {
            try
            {
                File.Delete(LogFilePath);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        protected override void LogInternally(LogLevel level, string message)
        {
            var success = TryCreateDirectory(Path.GetDirectoryName(LogFilePath));

            if (success)
            {
                lock (lockObj)
                {
                    using (var streamWriter = new StreamWriter(LogFilePath, append: true))
                    {
                        streamWriter.WriteLine(message);
                    }
                }
            }
        }

        private static string GetFullPath(string filePath)
        {
            var fullFilePath = string.IsNullOrEmpty(filePath) ? LogFilePath : filePath;
            if (!IsFullPath(fullFilePath))
            {
                var fixedRelativeFilePath = fullFilePath.StartsWith("\\") || fullFilePath.StartsWith("/") ? fullFilePath.Remove(0, 1) : fullFilePath;
                var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                fullFilePath = Path.Combine(exePath, fixedRelativeFilePath);
            }

            if (string.IsNullOrEmpty(Path.GetFileName(fullFilePath)))
            {
                return Path.Combine(fullFilePath, LogFilePath);
            }
            else
            {
                return fullFilePath;
            }
        }

        private static bool IsFullPath(string path)
        {
            return !String.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }

        private static string FixDirectoryPath(string dirPath)
        {
            return dirPath.EndsWith("\\") || dirPath.EndsWith("/") ? Path.GetDirectoryName(dirPath) : dirPath;
        }

        private static bool TryCreateDirectory(string filePath)
        {
            try
            {
                var dirName = FixDirectoryPath(filePath);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
