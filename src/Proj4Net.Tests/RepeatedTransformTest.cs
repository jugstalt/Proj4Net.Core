using System;
using NUnit.Framework;

namespace Proj4Net.Tests
{
public class RepeatedTransformTest 
{
  [Test]
    public void testRepeatedTransform()
  {
    var crsFactory = new CoordinateReferenceSystemFactory();

    var src = crsFactory.CreateFromName("epsg:4326");
    var dest = crsFactory.CreateFromName("epsg:27700");

    var ctf = new CoordinateTransformFactory();
    var transform = ctf.CreateTransform(src, dest);
    
    var srcPt = new ProjCoordinate(0.899167, 51.357216);
    var destPt = new ProjCoordinate();
   
    transform.Transform(srcPt, destPt);
    Console.WriteLine(srcPt + " ==> " + destPt);
    
    // do it again
    var destPt2 = new ProjCoordinate();
    transform.Transform(srcPt, destPt2);
    Console.WriteLine(srcPt + " ==> " + destPt2);

    Assert.IsTrue(destPt.Equals(destPt2));
  }
}}