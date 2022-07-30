using LuaLUT.Internal;
using LuaLUT.Internal.ImageWriter;
using LuaLUT.Internal.PixelWriter;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace LuaLUT.Tests.Internal;

public abstract class TestBase
{
    protected static Task<string> LoadScriptAsync(string localPath)
    {
        return Assembly.GetExecutingAssembly()
            .ReadTextAsync($"LuaLUT.Tests.TestScripts.{localPath}");
    }

    protected static async Task<byte[]> WriteRawDataAsync(PixelFormat pixelFormat, PixelType pixelType, float actualValue)
    {
        using var outputStream = new MemoryStream();

        if (PixelFormats.IsNormalized(pixelFormat)) {
            var pixelWriter = new PixelWriterNorm(outputStream) {
                PixelFormat = pixelFormat,
                PixelType = pixelType,
            };

            await WriteRawDataAsync(pixelWriter, pixelFormat, pixelType, actualValue);
        }
        else {
            var pixelWriter = new PixelWriterInt(outputStream) {
                PixelFormat = pixelFormat,
                PixelType = pixelType,
            };

            await WriteRawDataAsync(pixelWriter, pixelFormat, pixelType, actualValue);
        }

        return outputStream.GetBuffer();
    }

    private static async Task WriteRawDataAsync<T>(PixelWriterBase<T> pixelWriter, PixelFormat pixelFormat, PixelType pixelType, float actualValue)
    {
        var imageWriter = new RawImageWriter<T>(pixelWriter) {
            PixelFormat = pixelFormat,
            PixelType = pixelType,
            CustomVariables = {
                ["value"] = actualValue,
            },
        };

        var luaScript = await LoadScriptAsync("custom-variables.lua");
        await imageWriter.ProcessAsync(luaScript, 1, 1);
    }
}