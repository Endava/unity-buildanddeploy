using Endava.BuildAndDeploy;
using Endava.BuildAndDeploy.Logging;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;

namespace Tests
{
    public class CommandLineParserTest
    {
        [Test]
        public void ParserWithNullString()
        {
            Assert.IsTrue(CommandLineParser.TryParseArguments(null, out CommandLineParser result));
            Assert.NotNull(result);
        }

        [Test]
        public void ParserWithEmptyArguments()
        {
            Assert.IsTrue(CommandLineParser.TryParseArguments(Array.Empty<string>(), out CommandLineParser result));
            Assert.NotNull(result);
        }

        [Test]
        public void ParserWithManyArguments()
        {
            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-quit", "-batchmode", "-projectPath", "C:\\Users\\UserName\\Documents\\MyProject", "-executeMethod", "MyEditorScript.PerformBuild" }, out CommandLineParser result));
            Assert.NotNull(result);
        }

        [Test]
        public void ParserWithBuildTargetSet()
        {
            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-batchmode" }, out CommandLineParser result));
            Assert.NotNull(result);
            Assert.IsFalse(result.HasBuildTargetSet);

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-buildTarget iOS" }, out result));
            Assert.NotNull(result);
            Assert.IsTrue(result.HasBuildTargetSet);
        }

        [Test]
        public void ParserWithSomeOtherArguments()
        {
            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-lisemiff -buildTargets=iOS helloWorld" }, out CommandLineParser result));
            Assert.NotNull(result);
            Assert.IsNull(result.TargetProcessPath);
            Assert.IsNull(result.AndroidSdkPath);
            Assert.IsNull(result.AndroidNdkPath);
            Assert.IsNull(result.OverrideLogTargets);
            Assert.IsNull(result.OverrideLogLevel);
            Assert.IsNull(result.OverrideFullOutputPath);
        }

        [Test]
        public void ParserBuildProcessArgument()
        {
            var testPath = "Assets/Path/SomeMore";
            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { $"-buildProcess={testPath}" }, out CommandLineParser result));
            Assert.NotNull(result);
            Assert.AreEqual(result.TargetProcessPath, testPath, "BuildProcess Argument incorrect!");

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { $"-buildProcess= {testPath} " }, out result));
            Assert.NotNull(result);
            Assert.AreEqual(result.TargetProcessPath, testPath, "BuildProcess Argument incorrect! Should be trimmed!");

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { $"-BUildPROCess={testPath}" }, out result));
            Assert.NotNull(result);
            Assert.AreEqual(result.TargetProcessPath, testPath, "BuildProcess Argument should handle invariation of key!");
        }

        [Test]
        public void ParserAndroidSDKArgument()
        {
            var testPath = "someCoolAndroidSDKPath";
            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { $"-android-sdk={testPath}" }, out CommandLineParser result));
            Assert.NotNull(result);
            Assert.AreEqual(result.AndroidSdkPath, testPath, "AndroidSDK Argument incorrect!");

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { $"-android-sdk= {testPath} " }, out result));
            Assert.NotNull(result);
            Assert.AreEqual(result.AndroidSdkPath, testPath, "AndroidSDK Argument incorrect! Should be trimmed!");

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { $"-ANDROID-sDK={testPath}" }, out result));
            Assert.NotNull(result);
            Assert.AreEqual(result.AndroidSdkPath, testPath, "AndroidSDK Argument should handle invariation of key!");
        }

        [Test]
        public void ParserAndroidNDKArgument()
        {
            var testPath = "someCoolAndroidNDKPath";
            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { $"-android-ndk={testPath}" }, out CommandLineParser result));
            Assert.NotNull(result);
            Assert.AreEqual(result.AndroidNdkPath, testPath, "AndroidNDK Argument incorrect!");

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { $"-android-ndk= {testPath} " }, out result));
            Assert.NotNull(result);
            Assert.AreEqual(result.AndroidNdkPath, testPath, "AndroidNDK Argument incorrect! Should be trimmed!");

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { $"-anDROid-Ndk={testPath}" }, out result));
            Assert.NotNull(result);
            Assert.AreEqual(result.AndroidNdkPath, testPath, "AndroidNDK Argument should handle invariation of key!");
        }

        [Test]
        public void ParserLogLevelArgument()
        {
            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-overrideLogLevel=" }, out CommandLineParser result));
            Assert.NotNull(result);
            Assert.IsNull(result.OverrideLogLevel);

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-overrideLogLevel=warning" }, out result));
            Assert.NotNull(result);
            Assert.AreEqual(result.OverrideLogLevel, LogLevel.Warning);

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-overrideLogLevel=Error" }, out result));
            Assert.NotNull(result);
            Assert.AreEqual(result.OverrideLogLevel, LogLevel.Error);
        }

        [Test]
        public void ParserLogTargetsArgument()
        {
            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-overrideLogTargets=" }, out CommandLineParser result));
            Assert.NotNull(result);
            Assert.IsNull(result.OverrideLogTargets);
            Assert.IsFalse(result.OverrideLogTargets.HasValue);

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-overrideLogTargets=Unity" }, out result));
            Assert.NotNull(result);
            Assert.IsTrue(result.OverrideLogTargets.HasValue);
            Assert.IsTrue(result.OverrideLogTargets.Value.HasFlag(LogTargets.Unity));
            Assert.AreEqual(result.OverrideLogTargets.Value, LogTargets.Unity);

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-overrideLogTargets=unity,FilE" }, out result));
            Assert.NotNull(result);
            Assert.IsTrue(result.OverrideLogTargets.HasValue);
            Assert.IsTrue(result.OverrideLogTargets.Value.HasFlag(LogTargets.Unity));
            Assert.IsTrue(result.OverrideLogTargets.Value.HasFlag(LogTargets.File));
            Assert.AreEqual(result.OverrideLogTargets.Value, LogTargets.Unity | LogTargets.File);
        }

        [Test]
        public void ParserFullOutputPathArgument()
        {
            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-overrideFullOutputPath=C:\\Test\\Output\bin.exe" }, out var result));
            Assert.NotNull(result);
            Assert.AreEqual(result.OverrideFullOutputPath, "C:\\Test\\Output\bin.exe");

            Assert.IsTrue(CommandLineParser.TryParseArguments(new string[] { "-overrideFullOutputPath=Folder\\file.exe" }, out result));
            Assert.NotNull(result);
            Assert.AreEqual(result.OverrideFullOutputPath, "Folder\\file.exe");
        }
    }
}
