using LuaLUT.Internal.ImageWriter;
using LuaLUT.Internal.PixelWriter;
using LuaLUT.Tests.Internal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace LuaLUT.Tests;

public class CustomVariablesTests : TestBase
{
    [InlineData(0.0f, 0)]
    [InlineData(0.5f, 128)]
    [InlineData(1.0f, 255)]
    [Theory] public async Task CanWritePngNormalizedOutput_UBYTE(float actualValue, byte expectedValue)
    {
        using var outputStream = new MemoryStream();

        var writer = new StandardImageWriter(outputStream, ImageType.Png) {
            PixelFormat = PixelFormat.R_NORM,
            PixelType = PixelType.BYTE,
            CustomVariables = {
                ["value"] = actualValue,
            },
        };

        var luaScript = await LoadScriptAsync("custom-variables.lua");
        await writer.ProcessAsync(luaScript, 1, 1);

        outputStream.Seek(0, SeekOrigin.Begin);
        var image = await Image.LoadAsync<L8>(outputStream);
        image.RedEquals(0, 0, expectedValue);
    }

    [InlineData(0.0f,   0)]
    [InlineData(0.5f, 127)]
    [InlineData(1.0f, 255)]
    [Theory] public async Task CanWriteRawNormalizedOutput_UBYTE(float actualValue, byte expectedValue)
    {
        var outputBuffer = await WriteRawDataAsync(PixelFormat.R_NORM, PixelType.UNSIGNED_BYTE, actualValue);
        ByteAssert.UByteEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(0L,   0)]
    [InlineData(127L, 127)]
    [InlineData(255L, 255)]
    [Theory] public async Task CanWriteRawIntegerOutput_UBYTE(long actualValue, byte expectedValue)
    {
        var outputBuffer = await WriteRawDataAsync(PixelFormat.R_INT, PixelType.UNSIGNED_BYTE, actualValue);
        ByteAssert.UByteEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(0.0f,     0)]
    [InlineData(0.5f, 32767)]
    [InlineData(1.0f, 65535)]
    [Theory] public async Task CanWriteRawNormalizedOutput_USHORT(float actualValue, ushort expectedValue)
    {
        var outputBuffer = await WriteRawDataAsync(PixelFormat.R_NORM, PixelType.UNSIGNED_SHORT, actualValue);
        ByteAssert.UShortEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(    0L,     0)]
    [InlineData(32767L, 32767)]
    [InlineData(65535L, 65535)]
    [Theory] public async Task CanWriteRawIntegerOutput_USHORT(long actualValue, ushort expectedValue)
    {
        var outputBuffer = await WriteRawDataAsync(PixelFormat.R_INT, PixelType.UNSIGNED_SHORT, actualValue);
        ByteAssert.UShortEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(0.0f,          0)]
    [InlineData(0.5f, 2147483647)]
    [InlineData(1.0f, 4294967295)]
    [Theory] public async Task CanWriteRawNormalizedOutput_UINT(float actualValue, uint expectedValue)
    {
        var outputBuffer = await WriteRawDataAsync(PixelFormat.R_NORM, PixelType.UNSIGNED_INT, actualValue);
        ByteAssert.UIntEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(         0L,          0)]
    [InlineData(2147483647L, 2147483647)]
    [InlineData(4294967295L, 4294967295)]
    [Theory] public async Task CanWriteRawIntegerOutput_UINT(long actualValue, uint expectedValue)
    {
        var outputBuffer = await WriteRawDataAsync(PixelFormat.R_INT, PixelType.UNSIGNED_INT, actualValue);
        ByteAssert.UIntEquals(outputBuffer, 0, expectedValue);
    }
}
