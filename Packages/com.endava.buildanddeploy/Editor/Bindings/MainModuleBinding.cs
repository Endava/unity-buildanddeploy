//<auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//</auto-generated>

using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.UxmlBindings
{
	public partial struct MainModuleBinding
	{
		private VisualElement _____root_____; // avoid name collisions!
		public VisualElement GetRootElement() => _____root_____;

		public readonly Label preferencesLabel;
		public readonly VisualElement preferencesHelpIcon;
		public readonly EnumField buildTargetField;
		public readonly Toggle cleanBuildToggle;
		public readonly VisualElement cleanBuildHelpIcon;
		public readonly TextField outputPathField;
		public readonly Toggle showWarningsToggle;
		public readonly Button chooseDirectoryBtn;
		public readonly TextField exportNameField;
		public readonly TextField filenameExtensionField;
		public readonly TextField fullPathField;
		public readonly ToolbarButton pathCopyBtn;
		public readonly VisualElement pathCopyIcon;
		public readonly VisualElement fullPathHelpIcon;
		public readonly Label loggingLabel;
		public readonly VisualElement loggingHelpIcon;
		public readonly EnumField logLevelField;
		public readonly EnumFlagsField logTargetsField;
		public readonly HelpBox logFileHint;

		public MainModuleBinding(VisualElement parent)
		{
			_____root_____ = parent;
			preferencesLabel = parent.Q<Label>("preferencesLabel");
			preferencesHelpIcon = parent.Q<VisualElement>("preferencesHelpIcon");
			buildTargetField = parent.Q<EnumField>("buildTargetField");
			cleanBuildToggle = parent.Q<Toggle>("cleanBuildToggle");
			cleanBuildHelpIcon = parent.Q<VisualElement>("cleanBuildHelpIcon");
			outputPathField = parent.Q<TextField>("outputPathField");
			showWarningsToggle = parent.Q<Toggle>("showWarningsToggle");
			chooseDirectoryBtn = parent.Q<Button>("chooseDirectoryBtn");
			exportNameField = parent.Q<TextField>("exportNameField");
			filenameExtensionField = parent.Q<TextField>("filenameExtensionField");
			fullPathField = parent.Q<TextField>("fullPathField");
			pathCopyBtn = parent.Q<ToolbarButton>("pathCopyBtn");
			pathCopyIcon = parent.Q<VisualElement>("pathCopyIcon");
			fullPathHelpIcon = parent.Q<VisualElement>("fullPathHelpIcon");
			loggingLabel = parent.Q<Label>("loggingLabel");
			loggingHelpIcon = parent.Q<VisualElement>("loggingHelpIcon");
			logLevelField = parent.Q<EnumField>("logLevelField");
			logTargetsField = parent.Q<EnumFlagsField>("logTargetsField");
			logFileHint = parent.Q<HelpBox>("logFileHint");
		}
	}
}
