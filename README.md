# GS.Gltf.Collisions ![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat) ![HitCount](http://hits.dwyl.com/griffelstudio/gltf-collision-detection.svg)

.NET Core 3.1 library to find intersecting meshes in 3D glTF files.

Not the fastest yet, but stable and 100% pure C#, ready to be built and utilized in any .NET codebase.

![image](https://user-images.githubusercontent.com/8260211/210110809-7816b2b9-863a-4964-8088-60830dbc1012.png)

# Quick Start ![Package Deploy](https://github.com/griffelstudio/gltf-collision-detection/actions/workflows/nuget.yml/badge.svg?branch=development)

Get the package from [GitHub Packages](https://github.com/griffelstudio/gltf-collision-detection/pkgs/nuget/GS.Gltf.Collisions) or [NuGet.org](https://www.nuget.org/packages/GS.Gltf.Collisions#supportedframeworks-body-tab)

```
dotnet add package GS.Gltf.Collisions --version 1.0.0
```

Add ```using GS.Gltf.Collisions;```

Create settings according to which collisions will be detected, supplying paths to one or many glTF files:

```c#
var inputFiles = new List<string> { "Just.gltf", "An.glb", "Example.glb" };
var settings = new CollisionSettings(inputFiles);
```

Create detector object and run detection method

```c#
var detector = new CollisionDetector(settings);
var result = detector.Detect();
```

# Help

Result of collision detection is the list of `CollisionResult` objects, each containing pair of intersecting elements and `BoundingBox`s.

Each of both elements is represented by `KeyValuePair` where key is *model* index, value is glTF *node* index.
All bounding boxes are axis aligned. There are minimum one - around points of intersection, and maximum - around intersecting elements.

`CollisionSettings.OutputMode` enumeration has 3 options:
* `InMemory` - just calculates intersection result without writing any files to disk
* `SeparateFile` - creates glTF file containing red axis aligned bounding boxes for each intersected pair of elements
* `MergeAll` - creates one glTF file with all input models merged and adds red bounding boxes nodes in it

## Resources
https://github.com/jslee02/awesome-collision-detection#other-awesome-lists
