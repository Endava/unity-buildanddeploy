using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Endava.BuildAndDeploy.Logging;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable]
    public abstract class BasePlist<T> : NewBuildStep
    {
        [SerializeField]
        protected string Key = string.Empty;

        [SerializeField]
        protected T Value = default(T);

        public override string FoldedParameterPreviewText => $"{(string.IsNullOrEmpty(Key) ? "<MissingKey!>" : Key)} : {Value}";

        public override string Description => $"Set a {typeof(T).Name} value into plist";


        public override BuildValidation Validate()
        {
            return string.IsNullOrEmpty(Key) ? BuildValidation.CreateInvalid("Key of plist cannot be null or empty!") : BuildValidation.Valid;
        }

#if UNITY_IOS
        protected abstract PlistElement GetPlistElement(T value);
#endif
        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(Key), "Key");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(Value), "Value");
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
#if UNITY_IOS
            AddPlistElement(GetPlistElement(Value));

            BuildLogger.LogDebug($"Set plist {Value.GetType().Name} for key \"{Key}\" set to {Value} ");

            return Task.FromResult(BuildStepResult.Successfull);
#else
            return Task.FromResult(BuildStepResult.CreateError($"Plist {Value.GetType().Name} is only valid in IOS builds!"));
#endif
        }

#if UNITY_IOS

        protected void AddPlistElement(PlistElement element)
        {
            var outputpath = Process.Main.DeploymentPath;

            var plist = new PlistDocument();
            var plistFilePath = outputpath + "/Info.plist";

            plist.ReadFromFile(plistFilePath);

            var plistRoot = plist.root;
            var plistValues = plistRoot.values;

            if (plistValues.ContainsKey(Key))
                plistValues[Key] = element;
            else
                plistValues.Add(Key, element);

            plist.WriteToFile(plistFilePath);
    }
#endif
    }
}
