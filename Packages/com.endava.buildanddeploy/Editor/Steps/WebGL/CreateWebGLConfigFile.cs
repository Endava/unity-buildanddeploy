using Endava.BuildAndDeploy.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Endava.BuildAndDeploy.BuildSteps
{
    [Serializable, BuildStep("WebGL/CreateWebGLConfigFile")]
    public class CreateWebGLConfigFile : NewBuildStep
    {
        public override string Description => "Creates a WebGL json config file";

        [SerializeField]
        private string writeFileName = string.Empty;
        [SerializeField]
        private string buildPath = string.Empty;

        public override string FoldedParameterPreviewText => $"{writeFileName}";

        public override BuildValidation Validate()
        {
            if (string.IsNullOrEmpty(writeFileName))
                return BuildValidation.CreateInvalid("Source cannot be null or empty!");

#if UNITY_2020_OR_NEWER
            return return BuildValidation.CreateInvalid("This is not intended for older Unity versions (<2020) which creates a config json.");
#endif

            return BuildValidation.Valid;
        }

        protected override Task<BuildStepResult> ExecuteStep()
        {
            var outFilePath = Process.Main.DeploymentPath + Path.DirectorySeparatorChar + writeFileName;
            BuildLogger.LogDebug($"Creating file {outFilePath}");

            var success = false;
            using (var streamWriter = new StreamWriter(outFilePath, false))
            {
                try
                {
                    WebGLConfig webGLConfig = new WebGLConfig(buildPath);
                    string jsonString = webGLConfig.AsJsonString();
                    streamWriter.Write(jsonString);
                    success = true;
                }
                catch (Exception e)
                {
                    BuildLogger.LogDebug($"Error in write {outFilePath}. Reason: {e.Message}");
                }
            }

            if (!success)
                return Task.FromResult(BuildStepResult.CreateError($"Could not create file {outFilePath}"));

            BuildLogger.LogInformation($"File creation succeeded! {outFilePath}");
            return Task.FromResult(BuildStepResult.Successfull);
        }

        protected override void CreateBuildStepContentUi(VisualElement stepContentContainer, SerializedProperty serializedProperty)
        {
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(writeFileName), "JSON Out File");
            AddPropertyFieldWithLabel(stepContentContainer, serializedProperty, nameof(buildPath), "Server Build Path");
        }
    }

    class WebGLConfig
    {
        public string dataUrl;
        public string frameworkUrl;
        public string codeUrl;
        public string streamingAssetsUrl;
        public string companyName;
        public string productName;
        public string productVersion;

        public WebGLConfig(string buildUrl)
        {
            string dataUrlPostfix = "";
            string frameworkUrlPostfix = "";
            string codeUrlPostfix = "";
#if UNITY_2020_1_OR_NEWER
            if (!PlayerSettings.WebGL.decompressionFallback)
            {
                switch (PlayerSettings.WebGL.compressionFormat)
                {
                    case WebGLCompressionFormat.Brotli:
                        dataUrlPostfix = ".br";
                        frameworkUrlPostfix = ".br";
                        codeUrlPostfix = ".br";
                        break;
                    case WebGLCompressionFormat.Gzip:
                        dataUrlPostfix = ".gz";
                        frameworkUrlPostfix = ".gz";
                        codeUrlPostfix = ".gz";
                        break;
                    case WebGLCompressionFormat.Disabled:
                    default:
                        break;
                }
            }
            else
            {
                dataUrlPostfix = ".unityweb";
                frameworkUrlPostfix = ".unityweb";
                codeUrlPostfix = ".unityweb";
            }
#endif

            dataUrl = buildUrl + "/WebGL.data" + dataUrlPostfix;
            frameworkUrl = buildUrl + "/WebGL.framework.js" + frameworkUrlPostfix;
            codeUrl = buildUrl + "/WebGL.wasm" + codeUrlPostfix;
            streamingAssetsUrl = "StreamingAssets";
            companyName = Application.companyName;
            productName = Application.productName;
            productVersion = Application.version;
        }

        public string AsJsonString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}