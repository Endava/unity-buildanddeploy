using Endava.BuildAndDeploy;
using NUnit.Framework;
using System.IO;
using UnityEngine;

namespace Tests
{
    [Category("FileUtilities")]
    public class FileUtilitiesTests
    {
        [Test]
        public void NormalizeTest()
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                Assert.AreEqual("Some\\Cool\\Path\\file.exe", FileUtilities.Normalize("Some/Cool/Path/file.exe"));
                Assert.AreEqual("Some\\Cool\\Path\\file.exe", FileUtilities.Normalize("Some\\Cool\\Path\\file.exe"));
                Assert.AreEqual("Some\\Cool\\Path\\file.exe", FileUtilities.Normalize("Some\\Cool/Path\\file.exe"));
            }

            if (Path.DirectorySeparatorChar == '/')
            {
                Assert.AreEqual("Some/Cool/Path/file.exe", FileUtilities.Normalize("Some/Cool/Path/file.exe"));
                Assert.AreEqual("Some/Cool/Path/file.exe", FileUtilities.Normalize("Some\\Cool\\Path\\file.exe"));
                Assert.AreEqual("Some/Cool/Path/file.exe", FileUtilities.Normalize("Some\\Cool/Path\\file.exe"));
            }
        }

        [Test]
        public void NormalizeWindowsToUnixTest()
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                Assert.AreEqual("Some/Cool/Path/file.exe", FileUtilities.NormalizeWindowsToUnix("Some/Cool/Path/file.exe"));
                Assert.AreEqual("Some/Cool/Path/file.exe", FileUtilities.NormalizeWindowsToUnix("Some\\Cool\\Path\\file.exe"));
                Assert.AreEqual("Some/Cool/Path/file.exe", FileUtilities.NormalizeWindowsToUnix("Some\\Cool/Path\\file.exe"));
            }
        }

        [Test]
        public void GetAbsoluteProjectPathTest()
        {
            var projectPath = Path.GetFullPath(Application.dataPath.Replace("/Assets", ""));
            var result = FileUtilities.AbsoluteProjectPath();
            Assert.NotNull(result);
            Assert.AreEqual(FileUtilities.Normalize(projectPath), result);

            projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            result = FileUtilities.AbsoluteProjectPath();
            Assert.NotNull(result);
            Assert.AreEqual(FileUtilities.Normalize(projectPath), result);
        }


        [Test]
        public void GetAbsolutePathRelativeToProjectWithFileTest()
        {
            var projectPath = FileUtilities.Normalize(Path.GetFullPath(Application.dataPath.Replace("/Assets", "")));
            var relativePath = FileUtilities.Normalize("Folder/Folder2/file.txt");
            var result = FileUtilities.ConvertRelativeProjectPathToAbsolutePath(relativePath);
            Assert.NotNull(result);
            Assert.AreEqual(Path.Combine(projectPath, relativePath), result);
        }

        [Test]
        public void GetAbsolutePathRelativeToProjectWithDirectoryTest()
        {
            var projectPath = FileUtilities.Normalize(Path.GetFullPath(Application.dataPath.Replace("/Assets", "")));
            var relativePath = FileUtilities.Normalize("Folder/Folder2");
            var result = FileUtilities.ConvertRelativeProjectPathToAbsolutePath(relativePath);
            Assert.NotNull(result);
            Assert.AreEqual(Path.Combine(projectPath, relativePath), result);
        }

        [Test]
        public void GetAbsolutePathRelativeEdgeCaseTest()
        {
            var result = FileUtilities.ConvertRelativeProjectPathToAbsolutePath(null);
            Assert.IsNull(result);

            result = FileUtilities.ConvertRelativeProjectPathToAbsolutePath(string.Empty);
            Assert.AreEqual(string.Empty, result);

            // is already absolute!
            result = FileUtilities.ConvertRelativeProjectPathToAbsolutePath(@"C:\someFolder\somefile.txt");
            Assert.IsNull(result);
        }

        [Test]
        public void GetProjectPathRelativeTest()
        {
            var appended = FileUtilities.Normalize("Folder/Folder2/file.txt");
            var testPath = Path.Combine(FileUtilities.AbsoluteProjectPath(), appended);

            var result = FileUtilities.MakeRelativeToProjectPath(testPath);
            Assert.NotNull(result);
            Assert.AreEqual(appended, result);
        }

        [Test]
        public void GetProjectPathRelativeWithDirectoryTest()
        {
            var appended = FileUtilities.Normalize("Folder/Folder2");
            var testPath = Path.Combine(FileUtilities.AbsoluteProjectPath(), appended);

            var result = FileUtilities.MakeRelativeToProjectPath(testPath);
            Assert.NotNull(result);
            Assert.AreEqual(appended, result);
        }

        [Test]
        public void GetProjectPathRelativeWithSpacesTest()
        {
            var appended = FileUtilities.Normalize("New Folder/New Folder2/file.txt");
            var testPath = Path.Combine(FileUtilities.AbsoluteProjectPath(), appended);

            var result = FileUtilities.MakeRelativeToProjectPath(testPath);
            Assert.NotNull(result);
            Assert.AreEqual(appended, result);
        }

        [Test]
        public void HelperPathCombinationTest()
        {
            var projectPath = FileUtilities.AbsoluteProjectPath();
            var relativePath = FileUtilities.Normalize("Folder/Folder2/file.txt");
            var result = FileUtilities.MakeRelativeToProjectPath(Path.Combine(projectPath, relativePath));

            Assert.NotNull(result);
            Assert.AreEqual(relativePath, result);
            Assert.AreEqual(Path.Combine(projectPath, relativePath), FileUtilities.ConvertRelativeProjectPathToAbsolutePath(result));
        }

        [Test]
        public void DirectoryCreateCheckDeleteTest()
        {
            var folder = Path.Combine(FileUtilities.AbsoluteProjectPath(), "TestFolder");

            Assert.IsTrue(FileUtilities.CreateDirectory(folder));
            Assert.IsTrue(FileUtilities.DirectoryExists(folder));
            Assert.IsTrue(FileUtilities.DeleteDirectory(folder));
        }

        [Test]
        public void NonRootedDirectoryCreateCheckDeleteTest()
        {
            var folder = "TestFolder";

            Assert.IsTrue(FileUtilities.CreateDirectory(folder));
            Assert.IsTrue(FileUtilities.DirectoryExists(folder));
            Assert.IsTrue(FileUtilities.DeleteDirectory(folder));
        }

        [Test]
        public void FileCheckCopyMoveDeleteTest()
        {
            var file = Path.Combine(FileUtilities.AbsoluteProjectPath(), "README.md");

            Assert.IsTrue(FileUtilities.FileExists(file));
            Assert.IsTrue(FileUtilities.CopyFile(file, "README111.md"));
            Assert.IsTrue(FileUtilities.MoveFile("README111.md", "README222.md"));
            Assert.IsTrue(FileUtilities.DeleteFile("README222.md"));
        }

        [Test]
        public void NonRootedFileCheckCopyMoveDeleteTest()
        {
            var file =  "README.md";

            Assert.IsTrue(FileUtilities.FileExists(file));
            Assert.IsTrue(FileUtilities.CopyFile(file, "README111.md"));
            Assert.IsTrue(FileUtilities.MoveFile("README111.md", "README222.md"));
            Assert.IsTrue(FileUtilities.DeleteFile("README222.md"));
        }

        [Test]
        public void IsPathTest()
        {
            Assert.IsTrue(FileUtilities.IsPathDirectory("./"));
            Assert.IsTrue(FileUtilities.IsPathDirectory(""));
            Assert.IsTrue(FileUtilities.IsPathDirectory("TestFolder/Test"));
            Assert.IsTrue(FileUtilities.IsPathDirectory("TestFolder/Test/"));

            Assert.IsFalse(FileUtilities.IsPathDirectory("TestFolder/.git"));
            Assert.IsFalse(FileUtilities.IsPathDirectory("TestFolder/file.ext"));
            Assert.IsFalse(FileUtilities.IsPathDirectory("Test Folder/file.ext"));

            Assert.IsTrue(FileUtilities.IsPathDirectory("C:/TestFolder/Test"));
            Assert.IsTrue(FileUtilities.IsPathDirectory("C:\\TestFolder\\Test"));
            Assert.IsTrue(FileUtilities.IsPathDirectory("C:/TestFolder/Test/"));
            Assert.IsTrue(FileUtilities.IsPathDirectory("C:\\TestFolder\\Test\\"));
            Assert.IsFalse(FileUtilities.IsPathDirectory("C:/TestFolder/Test/test.file"));
            Assert.IsFalse(FileUtilities.IsPathDirectory("C:\\TestFolder\\Test\\test.file"));
        }

        [Test]
        public void ConvertPathRelativeToProjectWithNullTest()
        {
            // check edge case null parameters
            Assert.IsNull(FileUtilities.ConvertPathRelative(null, "C:\\SomePath\\AntoherPath"));
            Assert.IsNull(FileUtilities.ConvertPathRelative("C:\\SomePath\\AntoherPath", null));
            Assert.IsNull(FileUtilities.ConvertPathRelative(null, null));
        }

        [Test]
        public void ConvertPathRelativeToProjectWithSamePathTest()
        {
            string projectPath = "C:\\develop\\unitybuildanddeploy";

            // check same path
            Assert.AreEqual(".", FileUtilities.ConvertPathRelative(projectPath, projectPath));
            Assert.AreEqual(".", FileUtilities.ConvertPathRelative("C:\\develop\\unitybuildanddeploy\\", projectPath));
            Assert.AreEqual(".", FileUtilities.ConvertPathRelative(projectPath, "C:\\develop\\unitybuildanddeploy\\"));
            Assert.AreEqual(".", FileUtilities.ConvertPathRelative("C:/develop/unitybuildanddeploy", projectPath));
            Assert.AreEqual(".", FileUtilities.ConvertPathRelative(projectPath, "C:/develop/unitybuildanddeploy/"));
            Assert.AreEqual(".", FileUtilities.ConvertPathRelative("C:\\develop\\unitybuildanddeploy\\SomeFolder\\..\\", projectPath));
        }

        [Test]
        public void ConvertPathRelativeToProjectWithRelativeResultTest()
        {
            string projectPath = "C:\\develop\\unitybuildanddeploy";

            var pathRoot = $"{projectPath}\\Some Path";
            var output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("Some Path", output);

            pathRoot = $"Some Path";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("Some Path", output);

            pathRoot = $"Some Path\\AnotherPath";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("Some Path\\AnotherPath", output);

            pathRoot = $"{projectPath}\\Some Path\\AnotherPath\\..";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("Some Path", output);

            pathRoot = $"Some Path\\AnotherPath\\..";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("Some Path", output);

            pathRoot = $"C:\\develop\\Some Path\\AnotherPath";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("..\\Some Path\\AnotherPath", output);
        }

        [Test]
        public void ConvertPathRelativeToProjectWithOutOfFolderResultTest()
        {
            string projectPath = "C:\\Data\\develop\\Projects\\unitybuildanddeploy";

            var pathRoot = $"C:\\Data\\develop\\Projects";
            var output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("..", output);

            pathRoot = $"C:\\Data\\develop";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("..\\..", output);

            pathRoot = $"C:\\Data";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("..\\..\\..", output);

            pathRoot = $"C:";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("..\\..\\..\\..", output);
        }

        [Test]
        public void ConvertPathRelativeToProjectWithDifferentDriveResultTest()
        {
            string projectPath = "C:\\Data\\develop\\Projects\\unitybuildanddeploy";

            var pathRoot = $"D:\\Data\\develop\\Projects";
            var output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual(pathRoot, output);

            pathRoot = $"E:\\Data";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual(pathRoot, output);
        }

        [Test]
        public void ConvertPathRelativeToProjectWithFilesResultTest()
        {
            string projectPath = "C:\\develop\\unitybuildanddeploy";

            var pathRoot = $"{projectPath}\\build.zip";
            var output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("build.zip", output);

            pathRoot = $"{projectPath}\\Some Path\\build.zip";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("Some Path\\build.zip", output);

            pathRoot = $"C:\\develop\\build.zip";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("..\\build.zip", output);

            pathRoot = $"D:\\develop\\build.zip";
            output = FileUtilities.ConvertPathRelative(projectPath, pathRoot);
            Assert.AreEqual("D:\\develop\\build.zip", output);
        }
    }
}
