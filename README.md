# ailia-csharp

This is a sample to execute ailia's Inference API using C#.

## Requirements

- Windows 10
- VisualStudio 2019

## Architecture

Unity's cs file can be used from Visual Studio just by commenting out import UnityEngine.
Debug.Log, Color32, Vactor2, Mathf are not defined, so define them in [AiriaMigration.cs](/ailia-csharp/ailia-csharp/ailia/AiliaMigration.cs).

## Build

- Open `ailia-csharp.sln`.
- Change active platform `Any CPU` to `x64`.
- Build.

## Run

- Manually place `ailia.dll` to `/ailia-csharp/ailia-csharp/bin/x64/Debug`.
- `*.onnx` and `*.jpg` are automatically place to `/ailia-csharp/ailia-csharp/bin/x64/Debug` on build process.
- Run.

## Result

### yolox

Read yolox.jpg and display the inference result of yolox.

![yolox.png](yolox.png)

### facemesh

Read facemesh.jpg and display the inference result of facemesh.

![facemesh.png](facemesh.png)

## Architecture

The inference code is below. Usage is the same as the Unity version.

- [Form1.cs](/ailia-csharp/ailia-csharp/Form1.cs)
- [AiliaYoloxSample.cs](/ailia-csharp/ailia-csharp/yolox/AiliaYoloxSample.cs)
- [AiliaFaceMeshSample.cs](/ailia-csharp/ailia-csharp/facemesh/AiliaFaceMeshSample.cs)
