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

        public void Initialize()
        {
            context["width"] = Width;
            context["height"] = Height;
            context["PI"] = MathF.PI;

            foreach (var (key, value) in CustomVariables)
                context[key] = value;

            LoadScript("GLSL_ops.lua");
            LoadScript("GLSL_vec.lua");
            LoadScript("GLSL.lua");

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

        private void LoadScript(string localPath)
        {
            var script = Assembly.GetExecutingAssembly()
                .ReadText($"LutBaker.LuaScripts.{localPath}");

            context.DoString(script);
        }

        private async Task LoadScriptAsync(string localPath)
        {
            var script = await Assembly.GetExecutingAssembly()
                .ReadTextAsync($"LutBaker.LuaScripts.{localPath}");

            context.DoString(script);
        }
    }
}
