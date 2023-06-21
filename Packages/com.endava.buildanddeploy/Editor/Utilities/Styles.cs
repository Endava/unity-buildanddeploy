using UnityEditor;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
    internal static class Styles
    {
        public const string Hidden = "hidden";

        public const string kImagePath = "Packages/com.endava.buildanddeploy/Editor/Resources/";

        public static string GetPlatformIconName(BuildTarget mPlatform) => mPlatform switch
        {
            // For a list of editor icon names see:
            // https://gist.github.com/MattRix/c1f7840ae2419d8eb2ec0695448d4321
            BuildTarget.WSAPlayer => "BuildSettings.Metro",
            BuildTarget.StandaloneOSX => "BuildSettings.Standalone",
            BuildTarget.StandaloneWindows => "BuildSettings.Standalone",
            BuildTarget.iOS => "BuildSettings.iPhone",
            BuildTarget.Android => "BuildSettings.Android",
            BuildTarget.StandaloneWindows64 => "BuildSettings.Standalone",
            BuildTarget.WebGL => "BuildSettings.WebGL",
            BuildTarget.StandaloneLinux64 => "BuildSettings.Standalone",
            BuildTarget.PS4 => "BuildSettings.PS4",
            BuildTarget.XboxOne => "BuildSettings.XboxOne",
            BuildTarget.tvOS => "BuildSettings.tvOS",
            BuildTarget.Switch => "BuildSettings.Switch",
#if !UNITY_2022_2_OR_NEWER
            BuildTarget.Lumin => "BuildSettings.Lumin",
#endif
            BuildTarget.Stadia => "BuildSettings.Stadia",
            BuildTarget.GameCoreXboxOne => "BuildSettings.XboxOne",
            BuildTarget.PS5 => "BuildSettings.PS5",
            _ => null
        };

        public static void AssignValidationStateToTargetIcon(VisualElement target, BuildValidation state, bool emptyIfSuccess = true) =>
            AssignValidationStateToTargetIcon(target, state.Status, emptyIfSuccess);

        public static void AssignValidationStateToTargetIcon(VisualElement target, ValidationStatus state, bool emptyIfSuccess = true)
        {
            if (target == null) return;

            if (state == ValidationStatus.Valid)
            {
                target.EnableInClassList("valid-icon", !emptyIfSuccess);
                if (emptyIfSuccess)
                {
                    target.style.backgroundImage = StyleKeyword.Null;
                }
            }
            else
            {
                target.EnableInClassList("valid-icon", false);
            }

            target.EnableInClassList("warning-icon", state == ValidationStatus.Issued);
            target.EnableInClassList("error-icon", state == ValidationStatus.Invalid);
        }
    }
}

