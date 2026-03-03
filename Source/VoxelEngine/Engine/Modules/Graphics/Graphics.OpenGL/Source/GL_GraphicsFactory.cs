using Silk.NET.OpenGL;

namespace VoxelEngine.Graphics.OpenGL;

internal class GL_GraphicsFactory : IGraphicsFactory
{
    private GL _GL;

    public GL_GraphicsFactory(GL gL)
    {
        _GL = gL;
    }

    public IGraphicsCommandsList CreateCommandsList()
    {
        return new GL_GraphicsCommandsList(_GL);
    }

    public void Dispose()
    {
    }
}
