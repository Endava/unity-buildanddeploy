"Build and Deploy"
======================

A package which extends the Unity Build Feature by providing a Unity Object, which describes the actual build process, its settings, additional pre|post steps as well as an isolated logging.

The project was developed with **Unity 2021.1.17f1** but should be compatible with **Unity 2020** and above.

It is based upon the former UBS, even though that it does not share or extend any former features or datasets.

## Usage

Include the package as a custom unity package within the manifest or the Unity PackageManager window by using the **#upm** or **#\<tag\>**

    "com.endava.buildanddeploy": "https://github.com/Endava/unity-buildanddeploy.git#upm",
    "com.endava.buildanddeploy": "https://github.com/Endava/unity-buildanddeploy.git#1.0.0", // tag-based

Once added, create your **Build Process** by using the Unity Asset Menu **"Create/Build and Deploy/Build Process"** within the Project Window.  
Configure your build process object and customize the Unity Build with the given modules and features. For example by having various standalone "Build Process object" for Steam builds, Ubisoft builds or a standalone variant.

For further explanation read the [Getting Started](./Documentation~/getting-started.md) guide.

## Create package branch (upm)

The upm branch is generated within the CI system, which detects changes in 'main' branch and uses a CI-job to generate the upm branch automatically.
Therefore it is not required to create any package manually. **Just create a merge request and push your changes to the main branch.**

Please update the **[Changelog.md](./Changelog.md)** and **[package.json](./package.json)** whenever necessary. 

In addition, whenever a new upm branch has been created and you have updated the changelog and package.json version number, **create a git-tag of the created upm branch**.
