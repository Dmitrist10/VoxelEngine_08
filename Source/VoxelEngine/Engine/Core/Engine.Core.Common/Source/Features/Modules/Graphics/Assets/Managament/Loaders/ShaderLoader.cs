using System.IO;
using System.Text;

namespace VoxelEngine.Core;

public class ShaderLoader : IAssetLoader
{

    public IAssetData Load(string filePath)
    {
        string source = File.ReadAllText(filePath);
        string[] splitSource = source.Split("#type ", StringSplitOptions.RemoveEmptyEntries);

        string vertexSource = "";
        string fragmentSource = "";

        foreach (var part in splitSource)
        {
            var eol = part.IndexOfAny('\r', '\n');
            if (eol == -1) continue;

            string type = part.Substring(0, eol).Trim();
            string content = part.Substring(eol).Trim();

            if (type == "vertex")
            {
                vertexSource = content;
            }
            else if (type == "fragment")
            {
                fragmentSource = content;
            }
        }

        return new ShaderData(vertexSource, fragmentSource);
    }

}
