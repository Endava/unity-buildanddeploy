using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
	[System.Serializable]
	public class BuildOptionsStandalone : BuildOptionsBase
	{
		[SerializeField] protected bool copyPDBFiles = false;
		[SerializeField] protected bool createSolution = false;

		public override void Execute()
		{
			base.Execute();

#if UNITY_STANDALONE
			UnityEditor.WindowsStandalone.UserBuildSettings.copyPDBFiles = copyPDBFiles;
			UnityEditor.WindowsStandalone.UserBuildSettings.createSolution = createSolution;
#endif
		}

		public override void CreateUI(SerializedProperty prop, VisualElement root)
		{
			root.Add(CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(copyPDBFiles))));
			root.Add(CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(createSolution))));
			base.CreateUI(prop, root);
		}
	}
}
