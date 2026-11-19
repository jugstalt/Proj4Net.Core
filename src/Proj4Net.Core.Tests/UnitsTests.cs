using NUnit.Framework;
using System;
using Proj4Net.Core.Units;

namespace Proj4Net.Core.Tests;

public class UnitsTests
{
    [TestCase("m", 1.0)]
    [TestCase("ft", 0.3048)]
    [TestCase("us-ft", 0.304800609601219)]
    [TestCase("link", 0.201168)]
    [TestCase("ch", 20.1168)]
    [TestCase("us-ch", 20.11684023368047)]
    [TestCase("fath", 1.8288)]
    public void FindUnit_ByAbbreviation_ReturnsExpectedValue(string name, double metres)
    {
        Assert.That(Units.Units.FindUnit(name).Value, Is.EqualTo(metres).Within(1e-12));
    }

    [Test]
    public void TryFindUnit_UnknownName_ReturnsFalse()
    {
        Assert.That(Units.Units.TryFindUnit("not-a-unit", out _), Is.False);
    }

    [Test]
    public void Parser_UnknownUnit_Throws()
    {
        var factory = new CoordinateReferenceSystemFactory();
        Assert.Throws<ArgumentException>(() =>
            factory.CreateFromParameters("t", "+proj=tmerc +lat_0=0 +lon_0=9 +k=0.9996 +ellps=GRS80 +units=not-a-unit".Split(' ')));
    }

    [Test]
    public void Parser_LinkUnit_IsAccepted()
    {
        var factory = new CoordinateReferenceSystemFactory();
        Assert.DoesNotThrow(() =>
            factory.CreateFromParameters("t", "+proj=tmerc +lat_0=0 +lon_0=9 +k=0.9996 +ellps=GRS80 +units=link".Split(' ')));
    }
}
