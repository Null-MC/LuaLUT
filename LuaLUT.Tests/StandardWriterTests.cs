using LuaLUT.Internal.ImageWriter;
using LuaLUT.Internal.PixelWriter;
using LuaLUT.Tests.Internal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace LuaLUT.Tests;

public class StandardWriterTests : TestBase
{
    [InlineData(0.0f,   0)]
    [InlineData(0.5f, 128)]
    [InlineData(1.0f, 255)]
    [Theory] public async Task CanWriteNormalized_UBYTE(float actualValue, byte expectedValue)
    {
        using var image = await WriteDataAsync<L8>(PixelFormat.R_NORM, PixelType.UNSIGNED_BYTE, actualValue);
        image.ValueEquals(0, 0, expectedValue);
    }

    [InlineData(0.0f,     0)]
    [InlineData(0.5f, 32768)]
    [InlineData(1.0f, 65535)]
    [Theory] public async Task CanWriteNormalized_HALF_FLOAT(float actualValue, ushort expectedValue)
    {
        using var image = await WriteDataAsync<L16>(PixelFormat.R_NORM, PixelType.HALF_FLOAT, actualValue);
        image.ValueEquals(0, 0, expectedValue);
    }

    protected static async Task<Image<T>> WriteDataAsync<T>(PixelFormat pixelFormat, PixelType pixelType, float actualValue)
        where T : unmanaged, IPixel<T>
    {
        using var outputStream = new MemoryStream();

        var writer = new StandardImageWriter(outputStream, ImageType.Png) {
            ImageWidth = 1,
            ImageHeight = 1,
            ImageDimensions = 2,
            PixelFormat = pixelFormat,
            PixelType = pixelType,
            CustomVariables = {
                ["value"] = actualValue,
            },
        };

        var luaScript = await LoadScriptAsync("variable.lua");
        await writer.ProcessAsync(luaScript);

        outputStream.Seek(0, SeekOrigin.Begin);
        return await Image.LoadAsync<T>(outputStream);
    }
}
