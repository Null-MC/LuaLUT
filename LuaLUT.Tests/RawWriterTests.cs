using LuaLUT.Internal.ImageWriter;
using LuaLUT.Internal.PixelWriter;
using LuaLUT.Tests.Internal;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace LuaLUT.Tests;

public class RawWriterTests : TestBase
{
    [InlineData(0.0f,   0)]
    [InlineData(0.5f, 127)]
    [InlineData(1.0f, 255)]
    [Theory] public async Task CanWriteNormalized_UBYTE(float actualValue, byte expectedValue)
    {
        var outputBuffer = await WriteDataAsync(PixelFormat.R_NORM, PixelType.UNSIGNED_BYTE, actualValue);
        ByteAssert.UByteEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(0L,     0)]
    [InlineData(127L, 127)]
    [InlineData(255L, 255)]
    [Theory] public async Task CanWriteInteger_UBYTE(long actualValue, byte expectedValue)
    {
        var outputBuffer = await WriteDataAsync(PixelFormat.R_INT, PixelType.UNSIGNED_BYTE, actualValue);
        ByteAssert.UByteEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(0.0f,     0)]
    [InlineData(0.5f, 32767)]
    [InlineData(1.0f, 65535)]
    [Theory] public async Task CanWriteNormalized_USHORT(float actualValue, ushort expectedValue)
    {
        var outputBuffer = await WriteDataAsync(PixelFormat.R_NORM, PixelType.UNSIGNED_SHORT, actualValue);
        ByteAssert.UShortEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(    0L,     0)]
    [InlineData(32767L, 32767)]
    [InlineData(65535L, 65535)]
    [Theory] public async Task CanWriteInteger_USHORT(long actualValue, ushort expectedValue)
    {
        var outputBuffer = await WriteDataAsync(PixelFormat.R_INT, PixelType.UNSIGNED_SHORT, actualValue);
        ByteAssert.UShortEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(0.0f,          0)]
    [InlineData(0.5f, 2147483647)]
    [InlineData(1.0f, 4294967295)]
    [Theory] public async Task CanWriteNormalized_UINT(float actualValue, uint expectedValue)
    {
        var outputBuffer = await WriteDataAsync(PixelFormat.R_NORM, PixelType.UNSIGNED_INT, actualValue);
        ByteAssert.UIntEquals(outputBuffer, 0, expectedValue);
    }

    [InlineData(         0L,          0)]
    [InlineData(2147483648L, 2147483648)]
    [InlineData(4294967295L, 4294967295)]
    [Theory] public async Task CanWriteInteger_UINT(long actualValue, uint expectedValue)
    {
        var outputBuffer = await WriteDataAsync(PixelFormat.R_INT, PixelType.UNSIGNED_INT, actualValue);
        ByteAssert.UIntEquals(outputBuffer, 0, expectedValue);
    }

    protected static async Task<byte[]> WriteDataAsync(PixelFormat pixelFormat, PixelType pixelType, float actualValue)
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

        var luaScript = await LoadScriptAsync("variable.lua");
        await imageWriter.ProcessAsync(luaScript, 1, 1);
    }
}
