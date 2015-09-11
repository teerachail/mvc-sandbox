
namespace Microsoft.Framework.MemoryPool
{
    public interface IArraySegmentPool<T>
    {
        LeasedArraySegment<T> Lease(int size);

        void Return(LeasedArraySegment<T> buffer);
    }
}
