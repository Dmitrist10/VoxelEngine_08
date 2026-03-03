using System.Diagnostics.CodeAnalysis;

namespace VoxelEngine.Core;

public sealed class LayerManager
{
    private ILayer? currentLayer;

    public void SetLayer(ILayer layer)
    {
        currentLayer?.Shutdown();
        currentLayer = layer;
        currentLayer.Initialize();
    }

    public ILayer? GetLayer()
    {
        return currentLayer;
    }

    public bool TryGetLayer<T>([NotNullWhen(true)] out T layer)
    {
        if (currentLayer is T typedLayer)
        {
            layer = typedLayer;
            return true;
        }

        layer = default!;
        return false;
    }
}