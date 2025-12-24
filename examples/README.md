# Mod Menu Examples
Examples demonstrating how you can interact with Mod Menu.

# Overview
- [Building](#building)
    - [Dotnet](#dotnet-build)
    - [Visual Studio](#visual-studio-build)
    - [Build configuration](#build-configuration)

# Installing
# Building
Whichever approach you use for building from source, the resulting
plugin/mod can be found in `bin/`.

The following configurations are supported:
- Debug
- Release

## Dotnet build
To build with dotnet, run the following command, replacing
<configuration> with the desired value:
```sh
dotnet build -c <configuration>
```

## Visual Studio build
To build with Visual Studio, open `ModMenuExamples.sln` and build by pressing ctrl + shift + b,
or by selecting Build -> Build Solution.

## Build configuration
The following can be configured:
- The path Peaks of Yore is installed at
- Whether the mod should automatically install on build

Note that both of these properties are optional.

The configuration file must be in the root of this repository and must be called `Config.props`.
```xml
<Project>
  <PropertyGroup>
    <!-- For example, if peaks is installed under F: -->
    <GamePath>F:\Games\Peaks of Yore</GamePath>

    <!-- Add this option if you want to install after building -->
    <InstallAfterBuild>true</InstallAfterBuild>
  </PropertyGroup>
</Project>
```
