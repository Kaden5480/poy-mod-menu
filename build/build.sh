#!/usr/bin/env bash

set -xe

cd ../

MOD_NAME="ModMenu"
VERSION="$(git describe --abbrev=0 | tr -d  "v")"

BP_NAME="$MOD_NAME-$VERSION-BepInEx"
ML_NAME="$MOD_NAME-$VERSION-MelonLoader"
BP_DIR="build/$BP_NAME"
ML_DIR="build/$ML_NAME"


dotnet build -c Release-BepInEx
dotnet build -c Release-MelonLoader

mkdir -p "$BP_DIR"/plugins
mkdir -p "$ML_DIR"/Mods

# BepInEx
cp bin/release-bepinex/net472/"$MOD_NAME.dll" \
    "$BP_DIR/plugins/"
cp build/README-BepInEx.txt "$BP_DIR/README.txt"

# MelonLoader
cp bin/release-melonloader/net472/"$MOD_NAME.dll" \
    "$ML_DIR/Mods/"
cp build/README-MelonLoader.txt "$ML_DIR/README.txt"

# Zip everything
pushd "$BP_DIR"
zip -r ../"$BP_NAME.zip" .
popd

pushd "$ML_DIR"
zip -r ../"$ML_NAME.zip" .
popd

# Remove directories
rm -rf "$BP_DIR" "$ML_DIR"
