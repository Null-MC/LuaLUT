using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LutBaker.Internal
{
    internal class LuaScriptProcessor : IDisposable
    {
        private readonly Lua context;
        private LuaFunction processPixelFunc;

        public string Script {get; set;}
        public int Width {get; set;}
        public int Height {get; set;}
        public Dictionary<string, object> CustomVariables {get; set;}


        public LuaScriptProcessor()
        {
            context = new Lua();
            CustomVariables = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void Dispose()
        {
            context?.Dispose();
        }

        public async Task InitializeAsync()
        {
            context["width"] = Width;
            context["height"] = Height;

            foreach (var (key, value) in CustomVariables)
                context[key] = value;

            await LoadScriptAsync("GLSL_ops.lua");
            await LoadScriptAsync("GLSL_vec.lua");
            await LoadScriptAsync("GLSL.lua");

            context.DoString(Script);

            processPixelFunc = context["processPixel"] as LuaFunction;
            if (processPixelFunc == null) throw new ApplicationException("Failed to load script! No 'processPixel(x, y)' function found!");
        }

        public double[] ProcessPixel(int x, int y)
        {
            var result = processPixelFunc.Call(x, y);

            if (result.Length == 1 && result[0] is LuaTable resultTable) {
                return resultTable.Values.OfType<object>()
                    .Select(Convert.ToDouble).ToArray();
            }

            return result.Select(Convert.ToDouble).ToArray();
        }

        private async Task LoadScriptAsync(string localPath)
        {
            var script = await Assembly.GetExecutingAssembly()
                .ReadTextAsync($"LutBaker.LuaScripts.{localPath}");

            context.DoString(script);
        }
    }
}
