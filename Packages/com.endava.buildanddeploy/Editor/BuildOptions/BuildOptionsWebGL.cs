using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy
{
	[Serializable]
	public class BuildOptionsWebGL : BuildOptionsBase
	{
#if UNITY_WEBGL
		[SerializeField] protected UnityEditor.WebGL.CodeOptimization codeOptimization = UnityEditor.WebGL.CodeOptimization.Speed;
#endif
        [SerializeField] protected int codeOptimization = 0;

        public override void Execute()
		{
			base.Execute();

#if UNITY_WEBGL
			UnityEditor.WebGL.UserBuildSettings.codeOptimization = codeOptimization;
#endif
		}

		public override void CreateUI(SerializedProperty prop, VisualElement root)
		{
			root.Add(CreatePropertyFromSerializedObject(prop.FindPropertyRelative(nameof(codeOptimization))));
			base.CreateUI(prop, root);
		}
	}
}