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
	public partial struct ProcessMainBinding
	{
		private VisualElement _____root_____; // avoid name collisions!
		public VisualElement GetRootElement() => _____root_____;

		public readonly Label headline;
		public readonly VisualElement icon;
		public readonly VisualElement ctrmodules;
		public readonly Button buildBtn;
		public readonly Button buildAndRunBtn;
		public readonly Button cmdlineBtn;
		public readonly HelpBox validationInfo;

		public ProcessMainBinding(VisualElement parent)
		{
			_____root_____ = parent;
			headline = parent.Q<Label>("headline");
			icon = parent.Q<VisualElement>("icon");
			ctrmodules = parent.Q<VisualElement>("ctr-modules");
			buildBtn = parent.Q<Button>("buildBtn");
			buildAndRunBtn = parent.Q<Button>("buildAndRunBtn");
			cmdlineBtn = parent.Q<Button>("cmdlineBtn");
			validationInfo = parent.Q<HelpBox>("validationInfo");
		}
	}
}
