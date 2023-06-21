using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Endava.BuildAndDeploy
{
    [BuildModule(BuildExecutionMode.AfterBuild)]
    public class PostBuildStepsModule : BaseBuildStepsModule
    {
        public override string SectionName => "Post Build Steps";
        public override string HelpIconTooltip => "Enable the Post Build steps to add additional tasks AFTER the actual Unity Build has completed.";
    }
}