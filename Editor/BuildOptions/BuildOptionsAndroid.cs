using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
	[System.Serializable]
	public class BuildOptionsAndroid : BuildOptionsBase
	{
		[SerializeField] protected AndroidETC2Fallback androidETC2Fallback = AndroidETC2Fallback.Quality32Bit;
		[SerializeField] protected AndroidCreateSymbols androidCreateSymbols = AndroidCreateSymbols.Disabled;
		[SerializeField] protected bool exportAsGoogleAndroidProject = false;
		[SerializeField] protected bool symlinkSources = false;

		public override void Execute()
		{
			base.Execute();

			EditorUserBuildSettings.androidETC2Fallback = androidETC2Fallback;
			EditorUserBuildSettings.exportAsGoogleAndroidProject = exportAsGoogleAndroidProject;
			EditorUserBuildSettings.symlinkSources = symlinkSources;
			EditorUserBuildSettings.androidCreateSymbols = androidCreateSymbols;
		}

		public override BuildOptions GetBuildOptions(BuildOptions options)
		{
			options = Helpers.SetOptionsValue(options, BuildOptions.SymlinkSources, symlinkSources);
			return base.GetBuildOptions(options);
		}

		public override void CreateUI(SerializedProperty prop, VisualElement root)
		{
			root.Add(CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(androidETC2Fallback))));
			root.Add(CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(androidCreateSymbols))));
			root.Add(CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(exportAsGoogleAndroidProject))));
			root.Add(CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(symlinkSources))));
			base.CreateUI(prop, root);
		}
	}
}