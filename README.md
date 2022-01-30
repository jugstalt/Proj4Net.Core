# Proj4Net.Core

This project is a port of Proj4.Net for .NET Standard for use in .NET Core applications. 
origin: http://proj4net.codeplex.com/

## Project Description

Proj4Net is a C#/.Net library to transform point coordinates from one geographic coordinate system to another, 
including datum transformation. The core of this library is a port of the Proj4J library.

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

## Other libraries

Proj4Net is neither the only open source projection library for .Net available nor the one with the most active 
community. You may want to take a look at

* DotSpatial.Projections
* Proj.Net
* C# Wrapper for native proj4