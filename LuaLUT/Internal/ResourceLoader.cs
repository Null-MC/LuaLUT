using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace LuaLUT.Internal;

internal static class ResourceLoader
{
    public static string ReadText(this Assembly assembly, string resourcePath)
    {
        using var stream = assembly.GetManifestResourceStream(resourcePath);
        if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourcePath}' could not be found!");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static async Task<string> ReadTextAsync(this Assembly assembly, string resourcePath)
    {
        await using var stream = assembly.GetManifestResourceStream(resourcePath);
        if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourcePath}' could not be found!");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}