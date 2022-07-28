using LutBaker.Internal.Writing;
using NLua;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LutBaker.Internal
{
    internal class LuaScriptProcessor
    {
        public int Width {get; set;}
        public int Height {get; set;}


        public async Task BuildAsync(IImageWriter imageWriter, string luaScript)
        {
            using var luaContext = new Lua();
            luaContext["width"] = Width;
            luaContext["height"] = Height;

            var glslScript = await Assembly.GetExecutingAssembly()
                .ReadTextAsync("LutBaker.LuaScripts.GLSL.lua");

            luaContext.DoString(glslScript);

            for (var y = 0; y < Height; y++) {
                luaContext["y"] = y;

                for (var x = 0; x < Width; x++) {
                    luaContext["x"] = x;

                    var pixel = luaContext.DoString(luaScript)
                        .Cast<double>().ToArray();

                    imageWriter.AppendPixel(x, y, pixel);
                }
            }
        }
    }
}
