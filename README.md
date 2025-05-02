<p align="center">
  <img src="https://img.shields.io/badge/License-Apache%202.0-blue.svg">
  <img src="https://img.shields.io/badge/Version-1.25.1501-lightgrey">
  <img src="https://img.shields.io/badge/Tests-Passed-brightgreen">
</p><br>


# Proj4Net.Core

This project is a port of Proj4.Net for .NET Standard for use in .NET Core applications. 
origin: http://proj4net.codeplex.com/ (not available anymore!)

## Project Description

Proj4Net.Core is a C#/.Net library to transform point coordinates from one geographic coordinate system to another, 
including datum transformation. The core of this library is a port of the Proj4J library.

## Proj4Net targets

* .NET 9

## Home of Proj4J/Proj4

Proj4J: https://trac.osgeo.org/proj4j/

Description of projections and parameters: https://proj.org/index.html

Online implementation: https://epsg.io/transform

## Improvements

I made some improvements, now all tests run successful 

Modification to the original project 
[English](./doc/modifications_en.md)
[Germain](./doc/modifications_de.md)

## Usage of the library

Install nuget package: https://www.nuget.org/packages/Proj4Net.Core/

```
Install-Package Proj4Net.Core 
```

There is an implementation for a simple console application ``cs2cs.core`` in the project solution.

```csharp
   
   CoordinateReferenceSystemFactory crsFactory = new CoordinateReferenceSystemFactory();
   CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();

   var sourceCRS = crsFactory.CreateFromParameters("Anon", "+proj=longlat +datum=WGS84 +no_defs");
   var targetCRS = crsFactory.CreateFromName("EPSG:31256");

   var transform = ctFactory.CreateTransform(sourceCRS, targetCRS);

   var sourceCoord = new Coordinate(15, 47);
   var targetCoord = new Coordinate();

   transform.Transform(sourceCoord, targetCoord);

   Console.WriteLine(targetCoord);  // => ProjCoordinate[-101323.59 207623.96 NaN]

```

The following overload for the ``Transform()`` are available (added) with 2.x of the library:

```csharp

  Coordinate Transform(Coordinate src, Coordinate tgt);

  Coordinate Transform(Coordinate src);

  void Transform(int count, Action<int, Coordinate> setAction, Action<int, Coordinate> getAction, bool parallelize = false);

```

So you can also write:

``` csharp

  var targetCoord = transform.Transform(new Coordinate(15, 47));

  // mass transormation

  double[][] coordsArray = [
    [ 15.0, 47.0 ],
    [ 16.1, 42.1 ]
    //...
  ];

  trans.Transform(coordsArray.Length,
    (i, from) => {
      from.X = coordsArray[i] [0];
      from.Y = coordsArray[i][1];
    },
    (i, to) => {
      coordsArray[i][0] = to.X;
      coordsArray[i][1] = to.Y;
    }, 
    true); // run parallel, eg if coordsArray.Length > 100 

```

For mass transformation the transformation of the points can run parallel. Parallelize for example 
if there are more than 100 points.

## Other libraries

Proj4Net is neither the only open source projection library for .Net available nor the one with the most active 
community. You may want to take a look at

* DotSpatial.Projections
* Proj.Net
* C# Wrapper for native proj4