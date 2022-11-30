using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LuaLUT.Internal;

internal class LuaScriptProcessor : IDisposable
{
    private readonly Lua context;
    private LuaFunction processPixelFunc;
    private LuaFunction processTexelFunc;

    public string Script {get; set;}
    public int Width {get; set;}
    public int Height {get; set;}
    public int Depth {get; set;}
    public int Dimensions {get; set;}
    public List<string> IncludedFiles {get; set;}
    public Dictionary<string, object> CustomVariables {get; set;}


    public LuaScriptProcessor()
    {
        IncludedFiles = new List<string>();
        CustomVariables = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        context = new Lua();
        context.UseTraceback = true;

        Dimensions = 0;
        Width = Height = Depth = 1;
    }

    public void Dispose()
    {
        context?.Dispose();
    }

    public void Initialize()
    {
        if (Dimensions <= 0 || Dimensions > 3)
            throw new ArgumentOutOfRangeException(nameof(Dimensions), "Image must have between 1 and 3 dimensions!");

        context["imageWidth"] = Width;
        if (Dimensions > 1) context["imageHeight"] = Height;
        if (Dimensions > 2) context["imageDepth"] = Depth;
        context["PI"] = MathF.PI;

        LoadScript("GLSL_ops.lua");
        LoadScript("GLSL_vec.lua");
        LoadScript("GLSL_mat.lua");
        LoadScript("GLSL.lua");

        if (CustomVariables != null)
            foreach (var (key, value) in CustomVariables)
                context[key] = value;

        if (IncludedFiles != null)
            foreach (var file in IncludedFiles)
                context.DoFile(file);

        context.DoString(Script);

        processPixelFunc = context["processPixel"] as LuaFunction;
        processTexelFunc = context["processTexel"] as LuaFunction;

        //if (processPixelFunc == null && processTexelFunc == null)
        //    throw new ApplicationException("Failed to load script! No 'processPixel()' or 'processTexel()' function found!");
    }

    public object[] ProcessPixel(in int px)
    {
        object[] result;
        if (processTexelFunc != null) {
            var tx = (px + 0.5f) / Width;
            result = processTexelFunc.Call(tx);
        }
        else if (processPixelFunc != null) {
            result = processPixelFunc.Call(px);
        }
        else {
            throw new ApplicationException("Failed to load script! No 'processPixel()' or 'processTexel()' function found!");
        }

        if (result.Length == 1 && result[0] is LuaTable resultTable)
            return resultTable.Values.OfType<object>().ToArray();

        return result;
    }

    public object[] ProcessPixel(in int px, in int py)
    {
        object[] result;
        if (processTexelFunc != null) {
            var tx = (px + 0.5f) / Width;
            var ty = (py + 0.5f) / Height;
            result = processTexelFunc.Call(tx, ty);
        }
        else if (processPixelFunc != null) {
            result = processPixelFunc.Call(px, py);
        }
        else {
            throw new ApplicationException("Failed to load script! No 'processPixel()' or 'processTexel()' function found!");
        }

        if (result.Length == 1 && result[0] is LuaTable resultTable)
            return resultTable.Values.OfType<object>().ToArray();

        return result;
    }

    public object[] ProcessPixel(in int px, in int py, in int pz)
    {
        object[] result;
        if (processTexelFunc != null) {
            var tx = (px + 0.5f) / Width;
            var ty = (py + 0.5f) / Height;
            var tz = (pz + 0.5f) / Depth;
            result = processTexelFunc.Call(tx, ty, tz);
        }
        else if (processPixelFunc != null) {
            result = processPixelFunc.Call(px, py, pz);
        }
        else {
            throw new ApplicationException("Failed to load script! No 'processPixel()' or 'processTexel()' function found!");
        }

        if (result.Length == 1 && result[0] is LuaTable resultTable)
            return resultTable.Values.OfType<object>().ToArray();

        return result;
    }

    private void LoadScript(string localPath)
    {
        var script = Assembly.GetExecutingAssembly()
            .ReadText($"LuaLUT.LuaScripts.{localPath}");

        context.DoString(script);
    }

    //private async Task LoadScriptAsync(string localPath)
    //{
    //    var script = await Assembly.GetExecutingAssembly()
    //        .ReadTextAsync($"LuaLUT.LuaScripts.{localPath}");

    //    context.DoString(script);
    //}
}