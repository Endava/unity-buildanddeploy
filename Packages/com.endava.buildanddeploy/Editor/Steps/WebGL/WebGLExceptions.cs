using Endava.BuildAndDeploy.Logging;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("WebGL/WebGLExceptions")]
    public class WebGLExceptions : TestableBuildStep<WebGLExceptionSupport>
    {
        public override string Description => "Setup the exception level support for WebGL";

        [SerializeField]
        protected WebGLExceptionSupport ExceptionsSupport = WebGLExceptionSupport.None;

        public override string FoldedParameterPreviewText => ExceptionsSupport.ToString();

        public override BuildValidation Validate()
        {
            if (!Enum.IsDefined(typeof(WebGLExceptionSupport), ExceptionsSupport))
                return BuildValidation.CreateInvalid("WebGLExceptionSupport is not supported!");

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            BuildLogger.LogDebug($"ExceptionSupport before {PlayerSettings.WebGL.exceptionSupport}");
            PlayerSettings.WebGL.exceptionSupport = ExceptionsSupport;
            BuildLogger.LogDebug($"WebGL Exception={PlayerSettings.WebGL.exceptionSupport}");
            BuildLogger.LogDebug($"ExceptionSupport after {PlayerSettings.WebGL.exceptionSupport}");

            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(ExceptionsSupport), "WebGlExceptionType");
        }

        protected override Task<WebGLExceptionSupport> BeforeTestExecution()
        {
            Debug.Log($"Before Test State: {PlayerSettings.WebGL.exceptionSupport}");
            return Task.FromResult(PlayerSettings.WebGL.exceptionSupport);
        }

        protected override Task AfterTestExecution(WebGLExceptionSupport beforeState)
        {
            PlayerSettings.WebGL.exceptionSupport = beforeState;
            Debug.Log($"After Test State: {PlayerSettings.WebGL.exceptionSupport}");
            return Task.CompletedTask;
        }
    }
}
