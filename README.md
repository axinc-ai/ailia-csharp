# ailia-csharp

This is a sample to execute ailia's Predict API using C#.

## Requirements

- Windows 10
- VisualStudio 2019

## Architecture

Unity's cs file can be used from Visual Studio just by commenting out import UnityEngine.
Debug.Log and Color32 are not defined, so define them in AiriaUnityMigrate.cs.

## Build

- Open `ailia-csharp.sln`.
- Change active platform `Any CPU` to `x64`.
- Build.

## Run

- Manually place `ailia.dll` to `/ailia-csharp/ailia-csharp/bin/x64/Debug`.
- `yolox_tiny.opt.onnx` and `input.jpg` are automatically place to `/ailia-csharp/ailia-csharp/bin/x64/Debug` on build process.
- Run.

## Result

