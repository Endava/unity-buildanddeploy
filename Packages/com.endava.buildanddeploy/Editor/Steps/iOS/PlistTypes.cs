using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("iOS/PlistBool")]
    public class PlistBool : BasePlist<bool>
    {
#if UNITY_IOS
        protected override PlistElement GetPlistElement(bool value) => new PlistElementBoolean(value);
#endif
    }

    [Serializable, BuildStep("iOS/PlistInt")]
    public class PlistInt : BasePlist<int>
    {
#if UNITY_IOS
        protected override PlistElement GetPlistElement(int value) => new PlistElementInteger(value);
#endif
    }

    [Serializable, BuildStep("iOS/PlistFloat")]
    public class PlistFloat : BasePlist<float>
    {
#if UNITY_IOS
        protected override PlistElement GetPlistElement(float value) => new PlistElementReal(value);
#endif
    }

    [Serializable, BuildStep("iOS/PlistString")]
    public class PlistString : BasePlist<string>
    {
#if UNITY_IOS
        protected override PlistElement GetPlistElement(string value) => new PlistElementString(value);
#endif
    }

}
