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
	public partial struct ErrorBinding
	{
		private VisualElement _____root_____; // avoid name collisions!
		public VisualElement GetRootElement() => _____root_____;

		public readonly HelpBox errorLabel;

		public ErrorBinding(VisualElement parent)
		{
			_____root_____ = parent;
			errorLabel = parent.Q<HelpBox>("errorLabel");
		}
	}
}
