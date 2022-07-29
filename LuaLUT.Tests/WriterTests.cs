using LuaLUT.Internal.Writing;
using LuaLUT.Tests.Internal;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace LuaLUT.Tests
{
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
        public async Task CanWriteRaw()
        {
            using var outputStream = new MemoryStream();

            var writer = new RawImageWriter(outputStream) {
                PixelFormat = PixelFormat.RG_NORM,
                PixelType = PixelType.BYTE,
            };

            var luaScript = await LoadScriptAsync("test1.lua");
            await writer.ProcessAsync(luaScript, 32, 32);

            // TODO: now what?
        }
    }
}
