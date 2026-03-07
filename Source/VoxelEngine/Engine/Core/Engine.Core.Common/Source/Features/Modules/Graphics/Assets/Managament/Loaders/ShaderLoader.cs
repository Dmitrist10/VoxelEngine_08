using System.IO;
using System.Text;

namespace VoxelEngine.Core.Assets;

public class ShaderLoader : IAssetLoader<ShaderData>
{
    public ShaderData Load(Stream stream, AssetId id, string absolutePath)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
        string source = reader.ReadToEnd();

        if (absolutePath.EndsWith(".glsl", System.StringComparison.OrdinalIgnoreCase))
        {
            string[] splitSource = source.Split("#type ", System.StringSplitOptions.RemoveEmptyEntries);

            string vertexSource = "";
            string fragmentSource = "";

            foreach (var part in splitSource)
            {
                var eol = part.IndexOfAny(['\r', '\n']);
                if (eol == -1) continue;

                string type = part.Substring(0, eol).Trim();
                string content = part.Substring(eol).Trim();

                if (type == "vertex") vertexSource = content;
                else if (type == "fragment") fragmentSource = content;
            }

            return new ShaderData(vertexSource, fragmentSource);
        }
        else if (absolutePath.EndsWith(".vert", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShaderData(source, "");
        }
        else if (absolutePath.EndsWith(".frag", System.StringComparison.OrdinalIgnoreCase))
        {
            return new ShaderData("", source);
        }

        return new ShaderData(source, "");
    }
}
