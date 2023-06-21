using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

namespace Endava.BuildAndDeploy
{
	[Serializable]
	public class BuildOptionsiOS : BuildOptionsBase
	{
		[SerializeField] protected bool buildScriptsOnly = false;
		[SerializeField] protected XcodeBuildConfig iOSXcodeBuildConfig = XcodeBuildConfig.Release;

		public override void Execute()
		{
			base.Execute();
			EditorUserBuildSettings.buildScriptsOnly = buildScriptsOnly;
			EditorUserBuildSettings.iOSXcodeBuildConfig = iOSXcodeBuildConfig;
		}

		public override void CreateUI(SerializedProperty prop, VisualElement root)
		{
			root.Add(CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(iOSXcodeBuildConfig)), "Run in Xcode as"));
			base.CreateUI(prop, root);
		}

		protected override void CreateDevelopmentDependingUI(SerializedProperty prop, VisualElement root, PropertyField developmentPropertyField)
		{
			base.CreateDevelopmentDependingUI(prop, root, developmentPropertyField);

			var buildScriptsOnlyField = CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(buildScriptsOnly)), "Scripts Only Build", development);
			root.Add(buildScriptsOnlyField);

			developmentPropertyField.RegisterValueChangeCallback(evt =>
			{
				var newValue = evt.changedProperty.boolValue;
				buildScriptsOnlyField.SetEnabled(newValue);
			});
		}
	}
}
