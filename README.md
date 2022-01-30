# Proj4Net.Core

This project is a port of Proj4.Net for .NET Standard for use in .NET Core applications. 
origin: http://proj4net.codeplex.com/ (not available anymore!)

## Project Description

Proj4Net is a C#/.Net library to transform point coordinates from one geographic coordinate system to another, 
including datum transformation. The core of this library is a port of the Proj4 library.

## Proj4Net targets

* .NET Core
* .NET Framework 4.x

## Home of Proj4

Description of projections and parameters: https://proj.org/index.html

Online implementation: https://mygeodata.cloud/cs2cs/

## Still to do

* Implement remaining Projections
* Check CoordianteReferenceSystem resource files (epsg/esri/nad/world) for actuality (with proj4)

[Modification to the original project](./doc/modifications.md)

## Usage of the library

The is an implementation for a simple console application ``cs2cs.core`` in the project solution.

```csharp
   
   CoordinateReferenceSystemFactory crsFactory = new CoordinateReferenceSystemFactory();
   CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();

   var sourceCRS = crsFactory.CreateFromParameters("Anon", "+proj=longlat +datum=WGS84 +no_defs");
   var targetCRS = crsFactory.CreateFromName("EPSG:31256");

   var transform = ctFactory.CreateTransform(sourceCRS, targetCRS);

   var sourceCoord = new ProjCoordinate(15, 47);
   var targetCoord = new ProjCoordinate();

   transform.Transform(sourceCoord, targetCoord);

   Console.WriteLine(targetCoord);  // => ProjCoordinate[-101323.59 207623.96 NaN]

```

## Other libraries

Proj4Net is neither the only open source projection library for .Net available nor the one with the most active 
community. You may want to take a look at

* DotSpatial.Projections
* Proj.Net
* C# Wrapper for native proj4