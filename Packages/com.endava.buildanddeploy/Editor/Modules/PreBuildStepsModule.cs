namespace Endava.BuildAndDeploy
{
    [BuildModule(BuildExecutionMode.BeforeBuild, 10)]
    public class PreBuildStepsModule : BaseBuildStepsModule
    {
        public override string SectionName => "Pre Build Steps";
        public override string HelpIconTooltip => "Enable the Pre Build steps to execute tasks BEFORE the actual Unity Build starts.";
    }
}