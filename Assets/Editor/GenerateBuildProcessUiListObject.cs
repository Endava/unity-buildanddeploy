using Endava.BuildAndDeploy;
using System.IO;
using UnityEditor;

namespace Editor.BuildAndDeploy
{
    internal static class GenerateBuildProcessUiListObject
    {
        [MenuItem("Window/Build and Deploy/Generate build ui object")]
        private static void GenerateBuildProcessVariants()
        {
            var uiList = ObjectFactory.CreateInstance<BuildProcessUiList>();
            string directory = "Assets";
            string assetName = "uiList.asset";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(uiList, path);
        }
    }
}