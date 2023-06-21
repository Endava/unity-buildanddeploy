namespace Endava.BuildAndDeploy
{
    public static class ConstValidations
    {
        public static string InvalidSceneModuleEmptyBuildSettings = "Scenes in BuildSettings is used and found empty!";
        public static string InvalidSceneModuleBuildSettingsNull = "Scenes in BuildSettings is used contains null items!";
        public static string InvalidSceneModuleScenesNull = "Scenes cannot be null or empty!";
        public static string InvalidSceneModuleScenesNotExisiting = "Not existing or incorrect scene entry found!";
    }

    public static class ConstLogs
    {
        public static string ErrorSceneAssets = "SceneAssets found empty!";
        public static string ErrorSceneAsset = "Current scene cannot be added or might already been added!";
        public static string ErrorSceneAssetCannotBeAdded = "Current scene cannot be added!";

        public static string DebugSceneAssets = "Scene setup succeeded.";
    }
}