using LuaLUT.Internal.ImageWriter;
using LuaLUT.Internal.PixelWriter;
using LuaLUT.Tests.Internal;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace LuaLUT.Tests;

public class WriterTests : TestBase
{
    [Fact]
    public async Task CanWriteStandard()
    {
        using var outputStream = new MemoryStream();

        var writer = new StandardImageWriter(outputStream, ImageType.Png) {
            PixelFormat = PixelFormat.RG_NORM,
            PixelType = PixelType.BYTE,
        };

        var luaScript = await LoadScriptAsync("test1.lua");
        await writer.ProcessAsync(luaScript, 32, 32);

        // TODO: now what?
        // could reset the stream, reload as new PNG, and very left-right is X:red, and up-down is Y:green
    }

    [Fact]
    public async Task CanWriteRawNorm()
    {
        using var outputStream = new MemoryStream();

        var pixelWriter = new PixelWriterNorm(outputStream);
        var imageWriter = new RawImageWriter<double>(pixelWriter) {
            PixelFormat = PixelFormat.RG_NORM,
            PixelType = PixelType.UNSIGNED_BYTE,
        };

        var luaScript = await LoadScriptAsync("test1.lua");
        await imageWriter.ProcessAsync(luaScript, 32, 32);

        // TODO: now what?
    }

    [Fact]
    public async Task CanWriteRawInt()
    {
        using var outputStream = new MemoryStream();

        var pixelWriter = new PixelWriterInt(outputStream);
        var imageWriter = new RawImageWriter<long>(pixelWriter) {
            PixelFormat = PixelFormat.RG_INT,
            PixelType = PixelType.UNSIGNED_BYTE,
        };

        var luaScript = await LoadScriptAsync("test1.lua");
        await imageWriter.ProcessAsync(luaScript, 32, 32);

        // TODO: now what?
    }
}