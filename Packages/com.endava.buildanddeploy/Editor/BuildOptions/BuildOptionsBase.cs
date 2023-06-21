using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
	[Serializable]
	public class BuildOptionsBase
	{
		[SerializeField] protected bool development = false;
		[SerializeField] protected bool allowDebugging = false;
		[SerializeField] protected bool connectProfiler = false;
		[SerializeField] protected bool buildWithDeepProfilingSupport = false;
		[SerializeField] protected Compression compression = Compression.None;

		public enum Compression
		{
			None = 0,
			Lz4 = 2,
			Lz4HC = 3
		}

		public virtual void Execute()
		{
			EditorUserBuildSettings.development = development;
			EditorUserBuildSettings.allowDebugging = allowDebugging;
			EditorUserBuildSettings.connectProfiler = connectProfiler;
			EditorUserBuildSettings.buildWithDeepProfilingSupport = buildWithDeepProfilingSupport;
		}

		public virtual BuildOptions GetBuildOptions(BuildOptions options)
		{
			options = Helpers.SetOptionsValue(options, BuildOptions.Development, development);
			options = Helpers.SetOptionsValue(options, BuildOptions.AllowDebugging, allowDebugging);
			options = Helpers.SetOptionsValue(options, BuildOptions.ConnectWithProfiler, connectProfiler);
			options = Helpers.SetOptionsValue(options, BuildOptions.EnableDeepProfilingSupport, buildWithDeepProfilingSupport);

			options = Helpers.SetOptionsValue(options, BuildOptions.CompressWithLz4, compression == Compression.Lz4);
			options = Helpers.SetOptionsValue(options, BuildOptions.CompressWithLz4HC, compression == Compression.Lz4HC);

			return options;
		}

		public virtual void CreateUI(SerializedProperty prop, VisualElement root)
		{
			var developmentProperty = CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(development)), "Development Build");
			root.Add(developmentProperty);

			CreateDevelopmentDependingUI(prop, root, developmentProperty);

            root.Add(CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(compression)), "Compression Method"));
		}

		protected virtual void CreateDevelopmentDependingUI(SerializedProperty prop, VisualElement root, PropertyField developmentPropertyField)
		{
            var connectProfilerField = CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(connectProfiler)), "Autoconnect Profiler", development);
            root.Add(connectProfilerField);

            var buildWithDeepProfilingSupportField = CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(buildWithDeepProfilingSupport)), "Deep Profiling", development);
            root.Add(buildWithDeepProfilingSupportField);

            var allowDebuggingField = CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(allowDebugging)), "Script Debugging", development);
            root.Add(allowDebuggingField);

            developmentPropertyField.RegisterValueChangeCallback(evt =>
            {
                var newValue = evt.changedProperty.boolValue;
                connectProfilerField.SetEnabled(newValue);
                buildWithDeepProfilingSupportField.SetEnabled(newValue);
                allowDebuggingField.SetEnabled(newValue);
            });
        }

        protected PropertyField CreatePropertyFromSerializedObject(SerializedProperty property, bool enabled = true)
		{
			var result = new PropertyField(property);
			result.AddToClassList($"settings-property-field");
			result.SetEnabled(enabled);
			return result;
		}

		protected PropertyField CreatePropertyFromSerializedObject(SerializedProperty property, string label, bool enabled = true)
		{
			var result = new PropertyField(property, label);
            result.AddToClassList($"settings-property-field");
            result.SetEnabled(enabled);
			return result;
		}
	}
}