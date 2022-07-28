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

            var writer = new StandardImageWriter(outputStream, ImageType.Png) {
                PixelFormat = PixelFormat.RG_NORM,
                PixelType = PixelType.BYTE,
            };

            var processor = new LuaScriptProcessor {
                Width = 32,
                Height = 32,
            };

            var luaScript = await Assembly.GetExecutingAssembly()
                .ReadTextAsync("LutBaker.Tests.TestScripts.test1.lua");

            writer.Initialize(32, 32);
            await processor.BuildAsync(writer, luaScript);
            await outputStream.FlushAsync();
            await writer.CompleteAsync();

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

            writer.Initialize(32, 32);

            var processor = new LuaScriptProcessor {
                Width = 32,
                Height = 32,
            };

            var luaScript = await Assembly.GetExecutingAssembly()
                .ReadTextAsync("LutBaker.Tests.TestScripts.test1.lua");

            await processor.BuildAsync(writer, luaScript);
            await outputStream.FlushAsync();
            await writer.CompleteAsync();

            // TODO: now what?
        }
    }
}
