# Modifications

I made some modifications to the original project to ensure all the test run successful.
Here is a list of my edits:

## -nadgrids=@null

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

## lat_0, lat_1, lat_2 for lcc (Lambert Conformal Conic)

...

## gamma-Parameter

...

## Modifications on the Testing CSV Files

* PROJ4_SPCS_EPSG_nad83.csv => PROJ4_SPCS_EPSG_nad83_modified.csv
* PROJ4_SPCS_ESRI_nad83.csv => PROJ4_SPCS_ESRI_nad83_modified.csv  
* PROJ4_SPCS_nad27.csv => PROJ4_SPCS_nad27_modified.csv 

