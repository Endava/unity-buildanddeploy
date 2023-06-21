# Version 1.0.0
Refactored the whole Unity "Build And Deploy" objects and added modular approach within them. This allows you to create additional custom modules within your project, which can easily enhance the current build process object.

- **BREAKING CHANGES**
  - The BuildProcess scriptable object has been refactored in its structure **and is not compatible with previous versions**. You have to recreate your existing objects and copy their configuration. This is a bad side effect of the refactoring, which we thought it is worth, since the BuildProcess is now much more flexibel in its design and usage:
    - Build Modules have been added, which the BuildProcess now detects automatically and can be extended easily
    - Cleanup was done in the inspector to simplify/straighten the usage
    - Using the new UI Toolkit for improved module design
    - Removed "Build Asset" or "Build Script" category, which you can now use within the PreBuild/PostBuild step.
    - More error handling per module and reasonable explanations.
    - Flexibel "Build Target Build Settings Options", which can be extended/modified per platform
    - Provided modules are:
      - MainModule: For general settings and executed at the start of the process (path, platform target, logging, ...)
      - Build Settings Module: Platform specific settings applied to the Unity Build environment
      - Pre|Post Build steps Module: execute various code before|after the actual build
      - Unity Build Module: executes the Unity Build pipeline
  - BuildCollection were removed, since there usage scenario was causing more issues. (too time intense, platform switch etc...)
  - Removed any IMGUI rendering "style"
  - OutputPath is now separated into Path, Filename and extension.
  - Changing the command line parameters:
    - You cannot edit the "BuildMode". The Build is now what it is, no changes possible
    - Only overwriteable elements are "Log Targets", "Log Level" & "Full Output Path/BuildPath"

- **Changes**
  - added a refactored Build Process, which contains a better, modular and cleaned up building pipeline
    - modular approach (you can extend them within your project)
    - straighten build steps and create testable functionality for each step (Apply = DO IT, Test = DO IT and revert)
  - refactored samples to use NEW build process
  - update documentation to use NEW build process
  - Created a new "Build Process Overview" Editor window which you can open within (Window/Build and Deploy/Build Process Overview)

# Version 0.0.11
Update documentation and git links after moving the project from exozet to unitylibs group

- **Changes**
  - none - only CI and documentation changes

# Version 0.0.10
Updated BuildStep CreateWebGLConfigFile

- **Changes**
  - CreateWebGLConfigFile now using the BuildProcessSettings.DeploymentPath as base for the outputpath

# Version 0.0.9
Added extra TryBuildFromCloudBuild function for cloud build.

- **Changes**
  - TryBuildFromCloudBuild is almost TryBuildFromCommandLine, difference is "Applying Buildsetting" was removed. Cloud Build defines the build setting already.

# Version 0.0.8
Added functionality to override output path of the post build step for unity cloud build

- **Changes**
  - Add CloudBuildOutPath parameter in BuildProcess. OutPath of BuildProcess gets overriden in a CommandLineBuild if CloudBuildOutPath is not null. 

# Version 0.0.7
Improved WebGL json config file creation

- **Changes**
  - Checking unity version and set file postfix depending player setting

# Version 0.0.6
Updated support for newer Unity versions > 2021.2.x 

- **Changes**
  - added pre-compiler flag conditions for iOS build to seperate between old "iOSBuildType" and new "XcodeBuildConfig" enum

# Version 0.0.5
Added WebGL Build step to generate json setting file for older Unity WebGL WebApp setups

- **Changes**
  - added CreateWebGLConfigFile build step

# Version 0.0.4

Rewriting the Helpers class and exclude File related methods to FileUtilities. Create some new file&folder related steps + tests.
Some ui improvements.

- **Changes**
  - reworked build collection and build process to handle Unities assetbundle throwaway issue, where after building the assetbundles the loaded process reference will be null
  - minor ui fixes
  - exception handling for build process itself
  - separate the editor from commandline builds
  - changed logger handling for builds, since logging in editor and cmdline is very different (editor loggs on ui build|cmd a bit earlier)
  - fixing various issues in IOS build steps (compiler and import issues)
  - fixing issue in BuildSettings, which could happen if standalone builds like MacOSX are not installed in your unity installation
  
- **Breaking Changes**
  - buildCollections commandline builds might not make sense and will be removed in short future
  - removed not used "ConsoleLogger" and enumeration

# Version 0.0.3

Rewriting the Helpers class and exclude File related methods to FileUtilities. Create some new file&folder related steps + tests.
Some ui improvements.

- **Features**
  - new steps created: file&folder handling (MoveFile, DeleteFile, CopyFile, DeleteDirectory, MoveDirectory, CopyDirectory)
  - add FileUtilities with a lot of File|Folder|Path related utility methods
  - adding file/path normalization to all path related code
  - some ui fixes
  
- **Breaking Changes**
  - Helpers.GetProjectRelativePath -> FileUtilities.MakeRelativeToProjectPath
  - Helpers.GetAbsoluteProjectPath -> FileUtilities.AbsoluteProjectPath
  - Helpers.GetAbsolutePathRelativeToProject -> FileUtilities.ConvertRelativeProjectPathToAbsolutePath
  - removed some unneeded Helpers methods
  - IBuildProcessSection.Execute -> IBuildProcessSection.Execute(BuildProcess): changed IBuildProcessSection.Execute to contain the BuildProcess as parameter, since OnEnable is not called in CmdLine-Builds

# Version 0.0.2

minor fixes in buildprocess + append helper to be able to use path replacements 

- **Features**
  - Helpers.ReplacePossibleTemplatesInString() method can be used to replace string templates ($PROJECTDIR, $TARGETDIR, $ASSETSDIR) with their corresponding path string.

# Version 0.0.1

Initial commit

- **Features**
  - Build Process and Collection assets
  - BuildSteps: PreScript, PreSteps, Building, Build Assets, PostSteps, PostScripts
  - custom Build Settings for Standalone, iOS and Android (rest used readonly default view)
  - Command Line options
  - Logging file (Unity, File)
  - Error|Issue Details within Inspector
  - english texts
