using System.Runtime.CompilerServices;

namespace VoxelEngine.Core;

public class ResourcePool<T>
{
    private struct Slot
    {
        public T Item;
        public uint Generation;
    }

    private Slot[] _slots;
    private Queue<uint> _freeIndices;
    private uint _capacity;

    public ResourcePool(uint initialCapacity = 1024)
    {
        _capacity = initialCapacity;
        _slots = new Slot[_capacity];
        _freeIndices = new Queue<uint>((int)_capacity);

        // Fill free list (Start at 1, so Index 0 is strictly reserved as "Null")
        for (uint i = 1; i < _capacity; i++)
        {
            _freeIndices.Enqueue(i);
        }
    }

    public ResourceHandle Add(T item)
    {
        if (_freeIndices.Count == 0)
            Resize(); // Expand array if full

        uint index = _freeIndices.Dequeue();

        // We do NOT reset the generation. It keeps incrementing forever for this slot.
        _slots[index].Item = item;
        uint gen = _slots[index].Generation;

        return new ResourceHandle(index, gen);
    }

    public void Remove(ResourceHandle handle)
    {
        if (!IsValid(handle)) return;

        // Clear the item to allow Garbage Collection if T is a managed type
        _slots[handle.Index].Item = default!;
        // Increment the generation! Any old handles pointing here are now permanently invalid!
        _slots[handle.Index].Generation++;

        _freeIndices.Enqueue(handle.Index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid(ResourceHandle handle)
    {
        if (handle.Index == 0 || handle.Index >= _capacity) return false;
        return _slots[handle.Index].Generation == handle.Generation;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get(ResourceHandle handle)
    {
        if (!IsValid(handle))
            throw new Exception($"Invalid or expired ResourceHandle: {handle}");
        return _slots[handle.Index].Item;
    }

    private void Resize()
    {
        uint newCapacity = _capacity * 2;
        Array.Resize(ref _slots, (int)newCapacity);
        for (uint i = _capacity; i < newCapacity; i++)
            _freeIndices.Enqueue(i);
        _capacity = newCapacity;
    }
}