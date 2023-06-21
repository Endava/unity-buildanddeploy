using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Endava.BuildAndDeploy
{
    public static class FileUtilities
    {
        public const char WinSeparator = '\\';
        public const char UnixSeparator = '/';

        /// <summary>
        /// Converts a relative path of a unity project asset into an absolute path.
        /// Like: "Assets\Folder1\file.txt" to "C:\MyUnityProject\Assets\Folder1\file.txt"
        /// </summary>
        /// <param name="relativePath">The relative path of your file/directory.</param>
        /// <returns>Returns an absolute normalized path of the given relativePath, or null if path might already be rooted.</returns>
        public static string ConvertRelativeProjectPathToAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return relativePath;

            if (Path.IsPathRooted(relativePath))
                return null;

            var result = Path.Combine(AbsoluteProjectPath(), relativePath);
            result = Normalize(result);
            return result;
        }

        /// <summary>
        /// Returns the project path of the current unity project. (The path where the Assets, Temps, Library, etc. is listed)
        /// </summary>
        /// <returns>A unity applications current "project path".</returns>
        public static string AbsoluteProjectPath()
        {
            var result = Path.GetFullPath(Application.dataPath.Replace("/Assets", ""));
            result = Normalize(result);
            return result;
        }

        /// <summary>
        /// Normalize a path by using os related separators.
        /// </summary>
        /// <param name="path">The path you want to normalize.</param>
        /// <returns>Returns a normalized path string, depending on the actual OS unity is running on.</returns>
        public static string Normalize(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            if (Path.DirectorySeparatorChar == WinSeparator)
                path = path.Replace(UnixSeparator, WinSeparator);
            if (Path.DirectorySeparatorChar == UnixSeparator)
                path = path.Replace(WinSeparator, UnixSeparator);

            return path.Replace(string.Concat(WinSeparator, WinSeparator), WinSeparator.ToString());
        }

        /// <summary>
        /// Normalize path string by replace all Windows separators with the unix one.
        /// </summary>
        /// <param name="path">The path you want to normalize.</param>
        /// <returns>Returns a unix normalized path string.</returns>
        public static string NormalizeWindowsToUnix(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            return path.Replace(WinSeparator, UnixSeparator);
        }

        /// <summary>
        /// Normalize path string by replace all unix separators with the windows one.
        /// </summary>
        /// <param name="path">The path you want to normalize.</param>
        /// <returns>Returns a windows normalized path string.</returns>
        public static string NormalizeUnixToWindows(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            return path.Replace(UnixSeparator, WinSeparator);
        }

        public static bool IsPathDirectory(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            path = path.Trim();

            if (Directory.Exists(path))
                return true;

            if (File.Exists(path))
                return false;

            // neither file nor directory exists. guess intention

            // if has trailing slash then it's a directory
            if (new[] { "\\", "/" }.Any(x => path.EndsWith(x)))
                return true; // ends with slash

            // if has extension then it is most propably a file; directory otherwise
            return string.IsNullOrWhiteSpace(Path.GetExtension(path));
        }

        /// <summary>
        /// Checks if a given filename is based to the unity project path.
        /// </summary>
        /// <param name="fileName">The fileName/folder you want to check.</param>
        /// <returns>Returns true, if the fileName is based to the unity project path, otherwise false.</returns>
        public static bool IsFileInProjectRootDirectory(string fileName)
        {
            var relative = MakeRelativeToProjectPath(fileName);
            if (string.IsNullOrEmpty(relative))
                return false;

            return relative == Path.GetFileName(relative);
        }

        /// <summary>
        /// Converts a fileName path relative to the current used Unity project path.
        /// </summary>
        /// <param name="fileName">The fileName/folder you want to check.</param>
        /// <returns>Returns the fileName relative from the Unity Project folder. Returns null, if the project is outside of the Unity folder.</returns>
        public static string MakeRelativeToProjectPath(string fileName)
        {
            var basePath = AbsoluteProjectPath();
            fileName = Normalize(fileName);

            if (!Path.IsPathRooted(fileName))
                fileName = Path.Combine(basePath, fileName);

            if (!fileName.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                return null;

            return fileName
                .Substring(basePath.Length)
                .Trim(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Converts the given path relative to the unity project path.
        /// It will allow folder navigation like "..\\.." and can contain absolute and relative path.
        /// </summary>
        /// <param name="path">The path you want to have relative to the unity project folder.</param>
        /// <returns>Returns null on error or parameter incorrectness, "." on path similarity, relative path (if possible and on same drive), or absolute if on different drive.</returns>
        public static string ConvertPathRelativeToProject(string path)
            => ConvertPathRelative(AbsoluteProjectPath(), path);

        /// <summary>
        /// Converts the given path relative to the relativeTo parameter
        /// It will allow folder navigation like "..\\.." and can contain absolute and relative path.
        /// </summary>
        /// <param name="relativeTo">The path you want to be the origin of the path</param>
        /// <param name="path">The path you want to have relative to the relativeTo parameter.</param>
        /// <returns>Returns null on error or parameter incorrectness, "." on path similarity, relative path (if possible and on same drive), or absolute if on different drive.</returns>
        public static string ConvertPathRelative(string relativeTo, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(relativeTo)) return null;
                if (string.IsNullOrEmpty(path)) return null;
                if (relativeTo == path) return ".";

                // if there is no file separator at the end, the next checking method will not detect the last folder properly - therefore we add a separator if needed
                var fileSeparators = new char[] { '\\', '/' };
                if (IsPathDirectory(relativeTo) && fileSeparators.Any(x => path.EndsWith(x)) == false) relativeTo += Path.DirectorySeparatorChar;
                if (IsPathDirectory(path) && fileSeparators.Any(x => path.EndsWith(x)) == false) path += Path.DirectorySeparatorChar;

                // normalize for possible comparisons
                var normalizedRelativeTo = Normalize(relativeTo);
                var normalizedPath = Normalize(path);

                if (normalizedRelativeTo == normalizedPath) return ".";

                if (!Path.IsPathRooted(normalizedPath))
                {
                    normalizedPath = Path.Combine(normalizedRelativeTo, normalizedPath);
                }

                string result = Path.GetRelativePath(normalizedRelativeTo, normalizedPath);
                result = result.TrimEnd(fileSeparators);

                return result;
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Returns a relative path from one path to another.
        /// </summary>
        /// <param name="relativeTo">The source path the result should be relative to. This path is always considered to be a directory.</param>
        /// <param name="path">The destination path.</param>
        /// <returns>The relative path, or path if the paths don't share the same root.</returns>
        public static string MakeRelativePath(string relativeTo, string path)
        {
            try
            {
                return Path.GetRelativePath(relativeTo, path);
            }
            catch
            {
                return path;
            }
        }

        /// <summary>
        /// Creates a given directory. If the path is not rooted, the unity project path gets added.
        /// </summary>
        /// <param name="path">The absolute or relative path you want to create.</param>
        /// <returns>Returns true, if the directory could be created with no issues, otherwise false.</returns>
        public static bool CreateDirectory(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return false;

                var directoryPath = Normalize(path);

                if (!Path.IsPathRooted(directoryPath))
                    directoryPath = Path.Combine(AbsoluteProjectPath(), directoryPath);

                var result = Directory.CreateDirectory(directoryPath);
                return result.Exists;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a directory exists. If the path is not rooted, the unity project path gets added.
        /// </summary>
        /// <param name="path">The absolute or relative path you want to check.</param>
        /// <returns>Returns true, if the directory exists, otherwise false.</returns>
        public static bool DirectoryExists(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return false;

                var directoryPath = Normalize(path);

                if (!Path.IsPathRooted(directoryPath))
                    directoryPath = Path.Combine(AbsoluteProjectPath(), directoryPath);

                return Directory.Exists(directoryPath);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes an existing directory. If the path is not rooted, the unity project path gets added.
        /// </summary>
        /// <param name="path">The absolute or relative path you want to delete.</param>
        /// <returns>Returns true, if the directory could be deleted, otherwise false.</returns>
        public static bool DeleteDirectory(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return false;

                var directoryPath = Normalize(path);

                if (!Path.IsPathRooted(directoryPath))
                    directoryPath = Path.Combine(AbsoluteProjectPath(), directoryPath);

                if (!DirectoryExists(directoryPath))
                    return false;

                Directory.Delete(directoryPath, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Moves a directory from its source to the destination folder. If the path is not rooted, the unity project path gets added.
        /// </summary>
        /// <param name="source">The absolute or relative source path.</param>
        /// <param name="destination">The absolute or relative destination path.</param>
        /// <returns>Returns true, if the directory could be moved, otherwise false.</returns>
        public static bool MoveDirectory(string source, string destination)
        {
            try
            {
                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination))
                    return false;

                var sourcePath = Normalize(source);
                var destinationPath = Normalize(destination);

                if (!Path.IsPathRooted(sourcePath))
                    sourcePath = Path.Combine(AbsoluteProjectPath(), sourcePath);

                if (!Path.IsPathRooted(destinationPath))
                    destinationPath = Path.Combine(AbsoluteProjectPath(), destinationPath);

                if (!DirectoryExists(sourcePath))
                    return false;

                Directory.Move(sourcePath, destinationPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Copies a directory from its source to the destination folder. If the path is not rooted, the unity project path gets added.
        /// </summary>
        /// <param name="sourceDirName">The absolute or relative source path.</param>
        /// <param name="destDirName">The absolute or relative destination path.</param>
        /// <param name="copySubDirs">Allows the copy process to be recursive.</param>
        /// <returns>Returns true, if the directory could be copied, otherwise false.</returns>
        public static bool CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceDirName) || string.IsNullOrEmpty(destDirName))
                    return false;

                var sourcePath = Normalize(sourceDirName);
                var destinationPath = Normalize(destDirName);

                if (!Path.IsPathRooted(sourcePath))
                    sourcePath = Path.Combine(AbsoluteProjectPath(), sourcePath);

                if (!Path.IsPathRooted(destinationPath))
                    destinationPath = Path.Combine(AbsoluteProjectPath(), destinationPath);

                if (!DirectoryExists(sourcePath))
                    return false;

                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                DirectoryInfo[] dirs = dir.GetDirectories();

                // If the destination directory doesn't exist, create it.
                Directory.CreateDirectory(destDirName);

                // Get the files in the directory and copy them to the new location.
                foreach (FileInfo file in dir.GetFiles())
                {
                    string tempPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(tempPath, false);
                }

                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string tempPath = Path.Combine(destDirName, subdir.Name);
                        CopyDirectory(subdir.FullName, tempPath, copySubDirs);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a file exists. If the path is not rooted, the unity project path gets added.
        /// </summary>
        /// <param name="path">The absolute or relative path you want to check.</param>
        /// <returns>Returns true, if the file exists, otherwise false.</returns>
        public static bool FileExists(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return false;

                var filePath = Normalize(path);

                if (!Path.IsPathRooted(filePath))
                    filePath = Path.Combine(AbsoluteProjectPath(), filePath);

                return File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes an existing file. If the path is not rooted, the unity project path gets added.
        /// </summary>
        /// <param name="path">The absolute or relative path you want to delete.</param>
        /// <returns>Returns true, if the file could be deleted, otherwise false.</returns>
        public static bool DeleteFile(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return false;

                var filePath = Normalize(path);

                if (!Path.IsPathRooted(filePath))
                    filePath = Path.Combine(AbsoluteProjectPath(), filePath);

                if (!FileExists(filePath))
                    return false;

                File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Moves a file from its source to the destination. If the path is not rooted, the unity project path gets added.
        /// </summary>
        /// <param name="source">The absolute or relative source path.</param>
        /// <param name="destination">The absolute or relative destination path.</param>
        /// <returns>Returns true, if the file could be moved, otherwise false.</returns>
        public static bool MoveFile(string source, string destination)
        {
            try
            {
                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination))
                    return false;

                var sourcePath = Normalize(source);
                var destinationPath = Normalize(destination);

                if (!Path.IsPathRooted(sourcePath))
                    sourcePath = Path.Combine(AbsoluteProjectPath(), sourcePath);

                if (!Path.IsPathRooted(destinationPath))
                    destinationPath = Path.Combine(AbsoluteProjectPath(), destinationPath);

                if (!FileExists(sourcePath))
                    return false;

                File.Move(sourcePath, destinationPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Copies a file from its source to the destination. If the path is not rooted, the unity project path gets added.
        /// </summary>
        /// <param name="source">The absolute or relative source path.</param>
        /// <param name="destination">The absolute or relative destination path.</param>
        /// <returns>Returns true, if the file could be copied, otherwise false.</returns>
        public static bool CopyFile(string source, string destination)
        {
            try
            {
                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination))
                    return false;

                var sourcePath = Normalize(source);
                var destinationPath = Normalize(destination);

                if (!Path.IsPathRooted(sourcePath))
                    sourcePath = Path.Combine(AbsoluteProjectPath(), sourcePath);

                if (!Path.IsPathRooted(destinationPath))
                    destinationPath = Path.Combine(AbsoluteProjectPath(), destinationPath);

                if (!FileExists(sourcePath))
                    return false;

                File.Copy(sourcePath, destinationPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
