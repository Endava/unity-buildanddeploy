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
	public partial struct BuildStepEntryFrameBinding
	{
		private VisualElement _____root_____; // avoid name collisions!
		public VisualElement GetRootElement() => _____root_____;

		public readonly VisualElement buildStepRootCtr;
		public readonly VisualElement stepCtr;
		public readonly VisualElement headerCtr;
		public readonly Button foldoutBtn;
		public readonly VisualElement foldoutIcon;
		public readonly Toggle enableStepToggle;
		public readonly Label readonlyStepTitleLabel;
		public readonly TextField editableStepTitleField;
		public readonly TextField parameterPreviewLabel;
		public readonly VisualElement validationIcon;
		public readonly VisualElement foldoutContent;
		public readonly Label buildStepDescription;
		public readonly Button openCodeBtn;
		public readonly VisualElement buildStepContent;
		public readonly Label emptyPropertiesLabel;
		public readonly Button applyStepBtn;
		public readonly Button testStepBtn;
		public readonly VisualElement errorCtr;
		public readonly HelpBox errorLabel;

		public BuildStepEntryFrameBinding(VisualElement parent)
		{
			_____root_____ = parent;
			buildStepRootCtr = parent.Q<VisualElement>("buildStepRootCtr");
			stepCtr = parent.Q<VisualElement>("stepCtr");
			headerCtr = parent.Q<VisualElement>("headerCtr");
			foldoutBtn = parent.Q<Button>("foldoutBtn");
			foldoutIcon = parent.Q<VisualElement>("foldoutIcon");
			enableStepToggle = parent.Q<Toggle>("enableStepToggle");
			readonlyStepTitleLabel = parent.Q<Label>("readonlyStepTitleLabel");
			editableStepTitleField = parent.Q<TextField>("editableStepTitleField");
			parameterPreviewLabel = parent.Q<TextField>("parameterPreviewLabel");
			validationIcon = parent.Q<VisualElement>("validationIcon");
			foldoutContent = parent.Q<VisualElement>("foldoutContent");
			buildStepDescription = parent.Q<Label>("buildStepDescription");
			openCodeBtn = parent.Q<Button>("openCodeBtn");
			buildStepContent = parent.Q<VisualElement>("buildStepContent");
			emptyPropertiesLabel = parent.Q<Label>("emptyPropertiesLabel");
			applyStepBtn = parent.Q<Button>("applyStepBtn");
			testStepBtn = parent.Q<Button>("testStepBtn");
			errorCtr = parent.Q<VisualElement>("errorCtr");
			errorLabel = parent.Q<HelpBox>("errorLabel");
		}
	}
}