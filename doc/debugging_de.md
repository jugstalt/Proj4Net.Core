# Debugging

Um nachvollziehen zu können, welche Schritt der einer Transformation durchlaufen werden, dann das
Programm ``Cs2Cs.Core-debug.exe`` verwendet werden.

Beispiel Aufruf:

```bash
 .\Cs2Cs.Core-debug.exe --from EPSG:31256 --to EPSG:3857

From: +proj=tmerc +lat_0=0 +lon_0=16.33333333333333 +k=1 +x_0=0 +y_0=-5000000 +ellps=bessel +towgs84=577.326,90.129,463.919,5.137,1.474,5.297,2.4232 +units=m +no_defs
To: +proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +wktext +no_defs
input coordinates:
examples:
>> 15 47
>> 15.12 48.1
>> 15.11,47.3
```

Gibt man jetzt einen Koordinate ein werden die einzelnen Schritte der Transformation aufgelistet:

```bash
>> -99411.687 318802.404

Debug: Start Transformation: -99411.687,318802.404

Debug: Projection: TransverseMercatorProjection
Debug:   (x,y) => (lon,lat): (15.001001939162,48.000522906924)

Debug: #### Begin datums transformations ####

Debug: Converted geodetic to geocentric
Debug: with Ellipsoid: a=6377397,155, b=6356078,963, 1/f=299,1528128
Debug:   (lon,lat) => [X,Y,Z] [4129463.201,1106563.727,4716436.932]

Debug: Transformed to WGS84 (Helmert)
Debug: with Datum: [Datum-User-defined 577,326,90,129,463,919,2,490487879859686E-05,7,14615365955456E-06,2,568058068837212E-05,1,0000024232]
Debug:   [X,Y,Z] => [X,Y,Z]: 4130055.821,1106645.123,4716910.329

Debug: Converted geocentric to geodetic
Debug: with Ellipsoid: a=6378137, b=6356752,314, 1/f=298,25722356
Debug:   [X,Y,Z] => (lon,lat): (15.000000008066,48.000000007697)

Debug: --- Begin grid shifts ---

Debug: Ignored optional grid shift: @null => (15.000000008066,48.000000007697)

Debug: --- End grid shifts -----

Debug: Inverse grid shift result
Debug:   (lon,lat) => (lon,lat) (15.000000008066,48.000000007697)

Debug: #### End datums transformations ######

Debug: Projection: MercatorProjection
Debug:   (lon,lat) => (x,y): (1669792.3627970004,6106854.836165595)
1669792.3627970004,6106854.836165595
```

Die Inverse Transformation wird ausgeführt, wenn man den Koordinaten ein ``!`` voran stellt:

```bash
>> !1669792.3627970004,6106854.836165595

Debug: Start Transformation: 1669792.3627970004,6106854.836165595

Debug: Projection: MercatorProjection
Debug:   (x,y) => (lon,lat): (15.000000008066,48.000000007697)

Debug: #### Begin datums transformations ####

Debug: --- Begin grid shifts ---

Debug: Ignored optional grid shift: @null => (15.000000008066,48.000000007697)

Debug: --- End grid shifts -----

Debug: Grid shift result
Debug:   (lon,lat) => (lon,lat) (15.000000008066,48.000000007697)

Debug: Converted geodetic to geocentric
Debug: with Ellipsoid: a=6378137, b=6356752,314, 1/f=298,25722356
Debug:   (lon,lat) => [X,Y,Z] [4130026.252,1106637.2,4716876.331]

Debug: Transformed from WGS84 (Helmert)
Debug: with Datum: [Datum-User-defined 577,326,90,129,463,919,2,490487879859686E-05,7,14615365955456E-06,2,568058068837212E-05,1,0000024232]
Debug:   [X,Y,Z] => [X,Y,Z]: [4129433.632,1106555.804,4716402.934]

Debug: Converted geocentric to geodetic
Debug: with Ellipsoid: a=6377397,155, b=6356078,963, 1/f=299,1528128
Debug:   [X,Y,Z] => (lon,lat): (15.00100194365,48.000522914552)

Debug: #### End datums transformations ######

Debug: Projection: TransverseMercatorProjection
Debug:   (lon,lat) => (x,y): (-99411.68665049569,318802.404842156)
-99411.68665049569,318802.404842156
```

> **_NOTE:_** Für das Debugging immer die exe mit Ending ``-debug.exe`` verwendet. In Release Versionen gibt es die entsprechenden Code Blöcke für das Debugging aus Performance Gründen nicht!
