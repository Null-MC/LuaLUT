using LutBaker.Internal;
using LutBaker.Internal.Writing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace LutBaker.Tests
{
    public class Tests
    {
        [Fact]
        public async Task CanWriteStandard()
        {
            using var outputStream = new MemoryStream();

            var luaScript = await Assembly.GetExecutingAssembly()
                .ReadTextAsync("LutBaker.Tests.TestScripts.test1.lua");

            var processor = new LuaScriptProcessor {
                Script = luaScript,
                Width = 32,
                Height = 32,
            };

            var writer = new StandardImageWriter(outputStream, ImageType.Png) {
                PixelFormat = PixelFormat.RG_NORM,
                PixelType = PixelType.BYTE,
            };

            await processor.InitializeAsync();

            await writer.ProcessAsync(processor);

            // TODO: now what?
            // could reset the stream, reload as new PNG, and very left-right is X:red, and up-down is Y:green
        }

        [Fact]
        public async Task CanWriteRaw()
        {
            using var outputStream = new MemoryStream();

            var luaScript = await Assembly.GetExecutingAssembly()
                .ReadTextAsync("LutBaker.Tests.TestScripts.test1.lua");

            var processor = new LuaScriptProcessor {
                Script = luaScript,
                Width = 32,
                Height = 32,
            };

            var writer = new RawImageWriter(outputStream) {
                PixelFormat = PixelFormat.RG_NORM,
                PixelType = PixelType.BYTE,
            };

            await processor.InitializeAsync();

            await writer.ProcessAsync(processor);

            // TODO: now what?
        }
    }
}
