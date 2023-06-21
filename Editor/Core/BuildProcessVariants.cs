
//<auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//</auto-generated>

using System.IO;
using UnityEditor;

namespace Endava.BuildAndDeploy
{
    public static class BuildProcessVariants
    {
        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/StandaloneOSX")]
        private static void CreateBuildProcessStandaloneOSX()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-StandaloneOSX.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.StandaloneOSX;
            process.Main.DeploymentPath = "Builds/StandaloneOSX/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/StandaloneWindows")]
        private static void CreateBuildProcessStandaloneWindows()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-StandaloneWindows.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.StandaloneWindows;
            process.Main.DeploymentPath = "Builds/StandaloneWindows/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/iOS")]
        private static void CreateBuildProcessiOS()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-iOS.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.iOS;
            process.Main.DeploymentPath = "Builds/iOS/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/Android")]
        private static void CreateBuildProcessAndroid()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-Android.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.Android;
            process.Main.DeploymentPath = "Builds/Android/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/StandaloneWindows64")]
        private static void CreateBuildProcessStandaloneWindows64()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-StandaloneWindows64.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.StandaloneWindows64;
            process.Main.DeploymentPath = "Builds/StandaloneWindows64/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/WebGL")]
        private static void CreateBuildProcessWebGL()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-WebGL.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.WebGL;
            process.Main.DeploymentPath = "Builds/WebGL/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/WSAPlayer")]
        private static void CreateBuildProcessWSAPlayer()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-WSAPlayer.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.WSAPlayer;
            process.Main.DeploymentPath = "Builds/WSAPlayer/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/StandaloneLinux64")]
        private static void CreateBuildProcessStandaloneLinux64()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-StandaloneLinux64.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.StandaloneLinux64;
            process.Main.DeploymentPath = "Builds/StandaloneLinux64/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/PS4")]
        private static void CreateBuildProcessPS4()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-PS4.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.PS4;
            process.Main.DeploymentPath = "Builds/PS4/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/XboxOne")]
        private static void CreateBuildProcessXboxOne()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-XboxOne.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.XboxOne;
            process.Main.DeploymentPath = "Builds/XboxOne/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/tvOS")]
        private static void CreateBuildProcesstvOS()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-tvOS.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.tvOS;
            process.Main.DeploymentPath = "Builds/tvOS/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/Switch")]
        private static void CreateBuildProcessSwitch()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-Switch.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.Switch;
            process.Main.DeploymentPath = "Builds/Switch/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }

#if !UNITY_2022_2_OR_NEWER
        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/Lumin")]
        private static void CreateBuildProcessLumin()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-Lumin.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.Lumin;
            process.Main.DeploymentPath = "Builds/Lumin/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }
#endif

        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/Stadia")]
        private static void CreateBuildProcessStadia()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-Stadia.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.Stadia;
            process.Main.DeploymentPath = "Builds/Stadia/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/LinuxHeadlessSimulation")]
        private static void CreateBuildProcessLinuxHeadlessSimulation()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-LinuxHeadlessSimulation.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.LinuxHeadlessSimulation;
            process.Main.DeploymentPath = "Builds/LinuxHeadlessSimulation/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/GameCoreXboxSeries")]
        private static void CreateBuildProcessGameCoreXboxSeries()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-GameCoreXboxSeries.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.GameCoreXboxSeries;
            process.Main.DeploymentPath = "Builds/GameCoreXboxSeries/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/GameCoreXboxOne")]
        private static void CreateBuildProcessGameCoreXboxOne()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-GameCoreXboxOne.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.GameCoreXboxOne;
            process.Main.DeploymentPath = "Builds/GameCoreXboxOne/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/PS5")]
        private static void CreateBuildProcessPS5()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-PS5.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.PS5;
            process.Main.DeploymentPath = "Builds/PS5/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


        [MenuItem("Assets/Create/Build and Deploy/Build Process (Variant)/EmbeddedLinux")]
        private static void CreateBuildProcessEmbeddedLinux()
        {
            var process = ObjectFactory.CreateInstance<BuildProcess>();
            
            string directory = "Assets";
            string assetName = "Process-EmbeddedLinux.asset";
            process.EnableFromInspector();
            process.Main.Target = BuildTarget.EmbeddedLinux;
            process.Main.DeploymentPath = "Builds/EmbeddedLinux/";
            string path = Path.Combine(directory, assetName);
            AssetDatabase.CreateAsset(process, path);
        }


    }
}
