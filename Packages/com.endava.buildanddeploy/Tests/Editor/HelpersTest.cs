using Endava.BuildAndDeploy;
using Endava.BuildAndDeploy.Logging;
using NUnit.Framework;
using System;
using System.IO;
using UnityEngine;

namespace Tests
{
    public class HelpersTest
    {
        [Test]
        public void DirectoryTemplatesMatch()
        {
            Assert.AreEqual("$ASSETSDIR", Helpers.AssetsDirTemplate);
            Assert.AreEqual("$TARGETDIR", Helpers.TargetDirTemplate);
            Assert.AreEqual("$PROJECTDIR", Helpers.ProjectDirTemplate);
        }

        [Test]
        public void ReplacePossibleTemplatesInStringTests()
        {
            var someFilePath = "some/folder/file.exe";
            var deploymentTargetPath = "Deployment/Path/file.exe";

            var result = Helpers.ReplacePossibleTemplatesInString(someFilePath, deploymentTargetPath);
            Assert.NotNull(result);
            Assert.AreEqual(someFilePath, result);

            var path = $"{Helpers.AssetsDirTemplate}/{someFilePath}";
            Assert.AreEqual($"{Application.dataPath}/{someFilePath}", Helpers.ReplacePossibleTemplatesInString(path, deploymentTargetPath));

            path = $"{Helpers.TargetDirTemplate}/{someFilePath}";
            Assert.AreEqual($"{Path.Combine(FileUtilities.AbsoluteProjectPath(), Path.GetDirectoryName(deploymentTargetPath))}/{someFilePath}", Helpers.ReplacePossibleTemplatesInString(path, deploymentTargetPath));

            path = $"{Helpers.ProjectDirTemplate}/{someFilePath}";
            Assert.AreEqual($"{FileUtilities.AbsoluteProjectPath()}/{someFilePath}", Helpers.ReplacePossibleTemplatesInString(path, deploymentTargetPath));
        }
    }
}
