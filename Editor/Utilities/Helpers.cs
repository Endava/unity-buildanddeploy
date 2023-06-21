using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    public static class Helpers
    {
        public const string ProjectDirTemplate = "$PROJECTDIR";
        public const string AssetsDirTemplate = "$ASSETSDIR";
        public const string TargetDirTemplate = "$TARGETDIR";

        /// <summary>
        /// Checks for possible templates within the input string and replaces possible templates, like $PROJECTDIR or $TARGETDIR, with corresponding path.
        /// </summary>
        /// <param name="value">The input string you want to check.</param>
        /// <param name="targetDeploymentPath">Required, since the TARGETDIR template needs the deployment path.</param>
        /// <returns>Returns the original string or, if templates exists, a replaced template string.</returns>
        public static string ReplacePossibleTemplatesInString(string value, string targetDeploymentPath)
        {
            if (value.Contains(ProjectDirTemplate))
            {
                return ReplaceTemplateWithPath(value, ProjectDirTemplate, FileUtilities.AbsoluteProjectPath());
            }
            else if (value.Contains(AssetsDirTemplate))
            {
                return ReplaceTemplateWithPath(value, AssetsDirTemplate, Application.dataPath);
            }
            else if (value.Contains(TargetDirTemplate))
            {
                return ReplaceTemplateWithPath(value, TargetDirTemplate, Path.Combine(FileUtilities.AbsoluteProjectPath(), Path.GetDirectoryName(targetDeploymentPath)));
            }

            string ReplaceTemplateWithPath(string input, string templateString, string replaceWith)
            {
                var result = input.Replace(templateString, "");
                if (result.StartsWith("\\"))
                {
                    result = result.Remove(0, 1);
                    return $"{replaceWith}\\{result}";
                }
                else if (result.StartsWith("/"))
                {
                    result = result.Remove(0, 1);
                    return $"{replaceWith}/{result}";
                }
                else
                {
                    return Path.Combine(replaceWith, result);
                }
            }

            return value;
        }

        /// <summary>
        /// Starts a shell/cmd line process by using the project path as origin.
        /// </summary>
        /// <param name="path">The script's path relative to the project path.</param>
        /// <returns>ReturnCode, Output tuple. Code != 0 is a possible error, with an error report as output string.</returns>
        public static (int, string) StartExternalProcess(string path)
        {
            return StartExternalProcess(FileUtilities.AbsoluteProjectPath(), path);
        }

        /// <summary>
        /// Starts a shell/cmd line process by using a working directory as origin and an executable script path.
        /// </summary>
        /// <param name="workingDir">The script's working directory origin.</param>
        /// <param name="path">The script's path relative to passed workingDir.</param>
        /// <returns>ReturnCode, Output tuple. Code != 0 is a possible error, with an error report as output string.</returns>
        public static (int, string) StartExternalProcess(string workingDir, string path)
        {
            if (string.IsNullOrEmpty(workingDir))
                return (-1, "WorkingDir cannot be empty!");

            if (string.IsNullOrEmpty(path))
                return (-1, "Path cannot be empty!");

            if (!File.Exists(Path.Combine(workingDir, path)))
                return (-1, $"File \"{Path.Combine(workingDir, path)}\" does not exists!");

            var fileEnding = Path.GetExtension(path);

            var processInfo = new ProcessStartInfo();

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardInput = false;
            processInfo.RedirectStandardError = true;
            processInfo.WorkingDirectory = workingDir;

#if UNITY_EDITOR_WIN
            processInfo.FileName = path;
#else
            processInfo.FileName = "sh";
            processInfo.Arguments = $"\"{Path.Combine(processInfo.WorkingDirectory, path)}\"";
#endif

            int exitCode = -1;
            string output = null;
            try
            {
                using (var p = new Process())
                {
                    p.StartInfo = processInfo;
                    p.Start();

                    // read the output stream first and then wait.
                    output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    exitCode = p.ExitCode;
                }
            }
            catch (Exception e)
            {
                output = $"Exception: {e.Message}";
            }

            return (exitCode, output);
        }

        public static BuildTargetGroup GroupFromBuildTarget(BuildTarget pTarget) => BuildPipeline.GetBuildTargetGroup(pTarget);
        public static string GroupFromBuildTargetWithoutObsolete(BuildTarget pTarget) 
        {
            var groupID = (int)BuildPipeline.GetBuildTargetGroup(pTarget);
            var names = Enum.GetNames(typeof(BuildTargetGroup));
            foreach (var name in names)
            {
                var field = typeof(BuildTargetGroup).GetField(name);
                
                if (field.IsDefined(typeof(ObsoleteAttribute), false))
                    continue;

                var id = Enum.Parse<BuildTargetGroup>(name);

                if ((int)id == groupID)
                    return name; 
            }

            return BuildTargetGroup.Unknown.ToString();
        }

        public static BuildOptions SetOptionsValue(BuildOptions options, BuildOptions option, bool val)
        {
            if (val)
                options |= option;
            else
                options &= ~option;

            return options;
        }

        /// <summary>
        /// Return a mapping of platform relevant information details of BuildTarget, Platform and platform target string.
        /// </summary>
        public static readonly List<Tuple<BuildTarget, string, string>> StandalonePlatformMapper = new List<Tuple<BuildTarget, string, string>>()
        {
#if !UNITY_2019_2_OR_NEWER
            Tuple.Create(BuildTarget.StandaloneLinux, "Linux", "x86"),
            Tuple.Create(BuildTarget.StandaloneLinuxUniversal", Linux", "x86_x64"),
#endif
            Tuple.Create(BuildTarget.StandaloneLinux64, "Linux", "x86_64"),
            Tuple.Create(BuildTarget.StandaloneWindows, "Windows", "x86"),
            Tuple.Create(BuildTarget.StandaloneWindows64, "Windows", "x86_64"),
            Tuple.Create(BuildTarget.StandaloneOSX, "Mac OS X", "x86_64"),
        };

        public static string BuildTargetPlatformExtension(BuildTarget target) => target switch
        {
            BuildTarget.Android => "apk", // apk, aab
            BuildTarget.iOS => "ipa",
            BuildTarget.WSAPlayer => "WSA",
            BuildTarget.StandaloneOSX => "app",
#if !UNITY_2019_2_OR_NEWER
            BuildTarget.StandaloneLinux or BuildTarget.StandaloneLinuxUniversal:
#endif
            BuildTarget.StandaloneLinux64 or BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64 => "exe",
            _ => "" // unknown or just a folder
        };

        /// <summary>
        /// Checks if a current build platform is supported/installed within your unity installation.
        /// </summary>
        /// <param name="target">The platform you want to check.</param>
        /// <returns>Returns true, if the target is properly installed in your unity environment, otherwise false.</returns>
        public static bool IsBuildTargetSupported(BuildTarget target)
        {
            var buildTargetGroup = Helpers.GroupFromBuildTarget(target);
            return BuildPipeline.IsBuildTargetSupported(buildTargetGroup, target);
        }

        public static Type[] GetAllSubTypes(Type baseType, bool include = false)
        {
            var result = new List<Type>();
            System.Reflection.Assembly[] AS = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var A in AS)
            {
                Type[] types = A.GetTypes();
                foreach (var T in types)
                {
                    if (T.IsSubclassOf(baseType))
                        result.Add(T);
                }
            }
            if (include)
                result.Add(baseType);

            return result.ToArray();
        }

        public static string[] CreateSceneListFromBuildSettings()
        {
            var result = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                if (scene == null) continue;
                if (string.IsNullOrEmpty(scene.path)) continue;

                result.Add(scene.path);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Retrieves a Unity in Build Editor texture
        /// </summary>
        /// <remarks>
        /// See: https://github.com/halak/unity-editor-icons
        /// </remarks>
        /// <param name="id">The id of the icon/texture - see repo for all possibilies.</param>
        /// <returns>Returns a texture matching the unity in build id, or null.</returns>
        public static Texture2D GetInBuildEditorUnityTexture(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return EditorGUIUtility.IconContent(id).image as Texture2D;
        }

        /// <summary>
        /// Retrieves a Unity in Build Editor texture and assigns
        /// </summary>
        /// <param name="target"></param>
        /// <param name="id"></param>
        public static void AssignInBuildEditorUnityTextureToVisualElement(VisualElement target, string id)
        {
            if (target == null) return;
            if (string.IsNullOrEmpty(id)) return;

            target.style.backgroundImage = new StyleBackground(EditorGUIUtility.IconContent(id).image as Texture2D);
        }
    }
}
