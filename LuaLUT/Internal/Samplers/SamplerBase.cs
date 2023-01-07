using NLua;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LuaLUT.Internal.Samplers;

internal interface ISampler
{
    Task LoadImageAsync(CancellationToken token = default);
    int TexelFetch(object texcoordData, out double pixelR, out double pixelG, out double pixelB, out double pixelA);
    int Texture(object texcoordData, out double pixelR, out double pixelG, out double pixelB, out double pixelA);
}

internal abstract class SamplerBase<TPixel> : ISampler
    where TPixel : struct, IRawPixel
{
    protected SamplerDescription Description {get;}


    protected SamplerBase(SamplerDescription description)
    {
        Description = description;
    }

    public async Task LoadImageAsync(CancellationToken token = default)
    {
        await using var stream = File.Open(Description.Filename, FileMode.Open, FileAccess.Read, FileShare.Read);

        LoadImageData(stream);
    }

    protected abstract void LoadImageData(Stream stream);

    public int TexelFetch(object texcoordData, out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        if (texcoordData is not LuaTable texcoordTable)
            throw new ApplicationException("Texture coordinate is undefined!");

        var texcoord = ParseTexcoord<int>(texcoordTable);
        GetPixel(texcoord, out var pixel);
        return pixel.Apply(out pixelR, out pixelG, out pixelB, out pixelA);
    }

    public int Texture(object texcoordData, out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        if (texcoordData is not LuaTable texcoordTable)
            throw new ApplicationException("Texture coordinate is undefined!");

        var texcoord = ParseTexcoord<double>(texcoordTable);
        GetPixelInterpolated(texcoord, out var pixel);
        return pixel.Apply(out pixelR, out pixelG, out pixelB, out pixelA);
    }

    private static T[] ParseTexcoord<T>(LuaTable texcoordTable)
    {
        if (texcoordTable == null)
            throw new ArgumentNullException(nameof(texcoordTable));

        return texcoordTable.Keys.Count switch {
            1 => new[] { (T)texcoordTable[1] },
            2 => new[] { (T)texcoordTable[1], (T)texcoordTable[2] },
            3 => new[] { (T)texcoordTable[1], (T)texcoordTable[2], (T)texcoordTable[3] },
            _ => throw new ApplicationException("texcoord is empty!")
        };
    }

    protected abstract void GetPixel(in int[] texcoord, out TPixel pixel);

    protected abstract void GetPixelInterpolated(in double[] texcoord, out TPixel pixel);
}
