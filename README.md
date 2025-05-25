# poy-mod-menu
![Code size](https://img.shields.io/github/languages/code-size/Kaden5480/poy-mod-menu?color=5c85d6)
![Open issues](https://img.shields.io/github/issues/Kaden5480/poy-mod-menu?color=d65c5c)
![License](https://img.shields.io/github/license/Kaden5480/poy-mod-menu?color=a35cd6)

> [!IMPORTANT]
> This is mod is incomplete and won't have an available release yet.
> If you know what you're doing and want to try it out, you can follow the [build instructions](#building-from-source).

An in game mod menu for
[Peaks of Yore](https://store.steampowered.com/app/2236070/).

# Overview
- [Building from source](#building-from-source)
    - [Dotnet](#dotnet-build)
    - [Visual Studio](#visual-studio-build)
    - [Custom game locations](#custom-game-locations)

# Building from source
Whichever approach you use for building from source, the resulting
plugin/mod can be found in `bin/`.

The following configurations are supported:
- Debug-BepInEx
- Release-BepInEx
- Debug-MelonLoader
- Release-MelonLoader

## Dotnet build
To build with dotnet, run the following command, replacing
<configuration> with the desired value:
```sh
dotnet build -c <configuration>
```

## Visual Studio build
To build with Visual Studio, open ModMenu.sln and build by pressing ctrl + shift + b,
or by selecting Build -> Build Solution.

## Custom game locations
If you installed Peaks of Yore in a custom game location, you may require
an extra file to configure the build so it knows where to find the Peaks of Yore game
libraries.

The file must be in the root of this repository and must be called "GamePath.props".

Below gives an example where Peaks of Yore is installed on the F drive:
```xml
<Project>
  <PropertyGroup>
    <GamePath>F:\Games\Peaks of Yore</GamePath>
  </PropertyGroup>
</Project>
```
