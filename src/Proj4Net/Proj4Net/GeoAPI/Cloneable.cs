#if !HAS_SYSTEM_ICLONEABLE
namespace Proj4Net.GeoAPI
{
    public interface ICloneable
    {
        object Clone();
    }
}
#endif
