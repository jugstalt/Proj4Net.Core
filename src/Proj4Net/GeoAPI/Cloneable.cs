#if !HAS_SYSTEM_ICLONEABLE
namespace GeoAPI
{
    public interface ICloneable
    {
        object Clone();
    }
}
#endif
