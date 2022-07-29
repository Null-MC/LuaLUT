using LuaLUT.Internal.Writing;
using LuaLUT.Tests.Internal;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace LuaLUT.Tests
{
    public class BrdfTests : TestBase
    {
        [Fact]
        //[Fact(Skip = "For manual use only")]
        public async Task CanWriteStandard()
        {
            await using var outputStream = File.Open("BRDF.png", FileMode.Create, FileAccess.Write);

            var writer = new StandardImageWriter(outputStream, ImageType.Png) {
                PixelFormat = PixelFormat.RG_NORM,
                PixelType = PixelType.HALF_FLOAT,
                CustomVariables = {
                    ["SAMPLE_COUNT"] = 32,
                },
            };

            var luaScript = await LoadScriptAsync("BRDF.lua");
            await writer.ProcessAsync(luaScript, 256, 256);
        }
    }
}
