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
	public partial struct ModuleFrameBinding
	{
		private VisualElement _____root_____; // avoid name collisions!
		public VisualElement GetRootElement() => _____root_____;

		public readonly VisualElement ctr_root;
		public readonly Foldout foldout_module;
		public readonly VisualElement moduleContent;
		public readonly VisualElement ctr_overlayItems;
		public readonly Toggle enabled;
		public readonly VisualElement moduleIcon;
		public readonly Label headlineLabel;
		public readonly VisualElement validationIcon;
		public readonly VisualElement helpIcon;

		public ModuleFrameBinding(VisualElement parent)
		{
			_____root_____ = parent;
			ctr_root = parent.Q<VisualElement>("ctr_root");
			foldout_module = parent.Q<Foldout>("foldout_module");
			moduleContent = parent.Q<VisualElement>("moduleContent");
			ctr_overlayItems = parent.Q<VisualElement>("ctr_overlayItems");
			enabled = parent.Q<Toggle>("enabled");
			moduleIcon = parent.Q<VisualElement>("moduleIcon");
			headlineLabel = parent.Q<Label>("headlineLabel");
			validationIcon = parent.Q<VisualElement>("validationIcon");
			helpIcon = parent.Q<VisualElement>("helpIcon");
		}
	}
}
