# Modifications

I made some modifications to the original project to ensure all the test run successful.
Here is a list of my edits:

## Removed Dependencies

I removed the dependencies to **GeoAPI**. In the original project only uses ``Coordinates`` class 
from the **GeoAPI** package.
A ``Coordinates`` class with the ``X``, ``Z``, ``Z`` properties is now included directly in
this project. So, there are no dependencies to other projects. 

## +nadgrids=@null

The WebMercator projection contains the parameter ``+nadgrids=@null``. This suppresses a datum shift:

``+proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +wktext +no_defs``

WebMercator calculates the geographic coordinates on the WGS84 ellipsoid.

``[X,Y,Z <==> Longidute, Latitude]``

However, the actual Mercator projection is calculated on another ellispoid (sphere)

``[Longidute, Latitude <==> x,y]``

By ``+nadgrids=@null`` the WGS84 ellispoid is be used in the datum transformation and not the WebMercator sphere.

First an new ``DatumTransformType`` ``NoDatum`` is introduced:

```csharp
        // Datum.cs

        public enum DatumTransformType
        {
            Unknown = 0,
            WGS84 = 1,
            ThreeParameters = 2,
            SevenParameters = 3,
            GridShift = 4,
            NoDatum = 5   // jugstalt
        }

        ...

        public Datum(String code,
                     String transformSpec,
                     Ellipsoid ellipsoid,
                     String name)
                     : this(code, (double[])null, ellipsoid, name)
        {
            if (transformSpec == "@null")   // jugstalt
            {
                _grids = null;
            }
            else
            {
                _grids = transformSpec.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        ...

        public DatumTransformType TransformType
        {
            get
            {
                if (_transform == null)
                {
                    return _grids != null
                        ? DatumTransformType.GridShift
                        : DatumTransformType.NoDatum;   // jugstalt
                        //: DatumTransformType.WGS84;
                }

                if (IsIdentity(_transform)) return DatumTransformType.WGS84;

                if (_transform.Length == 3) return DatumTransformType.ThreeParameters;
                if (_transform.Length == 7) return DatumTransformType.SevenParameters;
                
                return DatumTransformType.WGS84;
            }
        }
```

If the ``DatumTransformType`` is ``NoDatum`` then the WGS84 Ellipsoid is used as ``GeocentricConverter``:

```csharp

    // BasicCoordinateTransform.cs

    // constructor:

    // jugstalt
    _sourceGeoConv = sourceCRS.Datum.TransformType == Datum.Datum.DatumTransformType.NoDatum ?
                new GeocentricConverter(Datum.Datum.WGS84.Ellipsoid) :
                new GeocentricConverter(sourceCRS.Datum.Ellipsoid);

    _targetGeoConv = targetCRS.Datum.TransformType == Datum.Datum.DatumTransformType.NoDatum ?
                new GeocentricConverter(Datum.Datum.WGS84.Ellipsoid) :
                new GeocentricConverter(targetCRS.Datum.Ellipsoid);
```

In the Test project is a method to check WebMercator projections: ``Proj4Net.Tests.CoordinateTransformTest.TestNadGrids_atNULL()``

...

## +lat_0, +lat_1, +lat_2 for lcc (Lambert Conformal Conic) projection

The **ED50 / France EuroLambert (EPSG:2192)** for example has the following definition:

``+proj=lcc +lat_1=46.8 +lat_0=46.8 +lon_0=2.337229166666667 +k_0=0.99987742 +x_0=600000 +y_0=2200000 +ellps=intl +towgs84=-87,-98,-121,0,0,0,0 +units=m +no_defs`` 

Here, the parameters ``lat_1`` and ``lat_0`` are defined but not ``lat_2``. This caused an error because a default value 
for ``lat_2`` is used during the projection. To correct that error, ``lat_2`` must set to the same value as ``lat_1``.

```csharp
            // Proj4Parser.cs

            // old 
            //if (parameters.TryGetValue(Proj4Keyword.lat_1, out s))
            //    projection.ProjectionLat.itude1Degrees = ParseAngle(s);

            //if (parameters.TryGetValue(Proj4Keyword.lat_2, out s))
            //    projection.ProjectionLatitude2Degrees = ParseAngle(s);

            // jugstalt
            bool lat_0_hasValue = false;
            if (parameters.TryGetValue(Proj4Keyword.lat_0, out s))
            {
                projection.ProjectionLatitudeDegrees = ParseAngle(s);
                lat_0_hasValue = true;
            }

            if (parameters.TryGetValue(Proj4Keyword.lat_1, out s))
            {
                projection.ProjectionLatitude1Degrees = ParseAngle(s);

                if (parameters.TryGetValue(Proj4Keyword.lat_2, out s))
                {
                    projection.ProjectionLatitude2Degrees = ParseAngle(s);
                }
                else if (projection.Name == "lcc")
                {
                    projection.ProjectionLatitude2Degrees = projection.ProjectionLatitude1Degrees;
                }

                if(!lat_0_hasValue && projection.Name=="lcc")
                {
                    projection.ProjectionLatitudeDegrees = projection.ProjectionLatitude1Degrees;
                }
            }

```

## gamma-Parameter

The ``+gamma`` parameter was not implemented:

```csharp
        // Proj4Parser.cs

        if (parameters.TryGetValue(Proj4Keyword.gamma, out s))
            projection.GammaDegrees = Double.Parse(s, CultureInfo.InvariantCulture);

        // Projections.cs
        public double Gamma
        {
            get { return _gamma; }
            set { _gamma = value; }
        }

        public double GammaDegrees
        {
            get { return _gamma * RTD; }
            set { _gamma = DTR * value; }
        }    

```

## Modifications on the Testing CSV Files

The tests defined in the ``*.csv`` files produced errors. 
Some have been fixed by the changes made above.
A few still delivered errors. However, the results were checked via the page https://mygeodata.cloud/cs2cs/.
I trust that the site delivers correct results and have adjusted the CSV files. 

* PROJ4_SPCS_EPSG_nad83.csv => PROJ4_SPCS_EPSG_nad83_modified.csv
* PROJ4_SPCS_ESRI_nad83.csv => PROJ4_SPCS_ESRI_nad83_modified.csv  
* PROJ4_SPCS_nad27.csv => PROJ4_SPCS_nad27_modified.csv 

My aim is that this library produces the same results as https://mygeodata.cloud/cs2cs/.