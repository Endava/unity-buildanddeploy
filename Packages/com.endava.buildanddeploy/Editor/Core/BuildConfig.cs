using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Endava.BuildAndDeploy
{
    public class BuildConfig : ScriptableObject
    {
        [Serializable]
        internal class BuildSettingEntry
        {
            public BuildTarget target;
            public List<string> settingReferences = new();
        }

        [SerializeField]
        internal List<BuildSettingEntry> settingsMap = new();


        //internal static readonly string BuildConfigDefaultPath = @"Assets\Editor\BuildConfig.asset";
        //internal static BuildConfig GetOrCreateConfig()
        //{
        //    var settings = AssetDatabase.LoadAssetAtPath<BuildConfig>(BuildConfigDefaultPath);
        //    var targetPath = Path.GetDirectoryName(BuildConfigDefaultPath);

        //    if (settings != null)
        //        return settings;

        //    if (!Directory.Exists(targetPath))
        //        Directory.CreateDirectory(targetPath); // make sure we create the folder

        //    settings = ScriptableObject.CreateInstance<BuildConfig>();
        //    AssetDatabase.CreateAsset(settings, BuildConfigDefaultPath);
        //    AssetDatabase.SaveAssets();
        //    return settings;
        //}
    }
}
