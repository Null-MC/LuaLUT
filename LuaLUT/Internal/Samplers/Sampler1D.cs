using LuaLUT.Internal.PixelWriter;
using System;
using System.IO;

namespace LuaLUT.Internal.Samplers;

internal class Sampler1D<TPixel> : SamplerBase<TPixel>
    where TPixel : struct, IRawPixel
{
    private TPixel[] data;


    public Sampler1D(SamplerDescription description) : base(description) {}

    protected override void LoadImageData(Stream stream)
    {
        data = new TPixel[Description.Width];

        var reader = new PixelReader(stream);

        for (var x = 0; x < Description.Width; x++) {
            data[x].Read(reader);
        }
    }

    protected override void GetPixel(in int[] texcoord, out TPixel pixel)
    {
        if (texcoord == null) throw new ArgumentNullException(nameof(texcoord));
        pixel = data[texcoord[0]];
    }

    protected override void GetPixelInterpolated(in double[] texcoord, out TPixel pixel)
    {
        if (texcoord == null) throw new ArgumentNullException(nameof(texcoord));
        if (texcoord.Length != 1) throw new ApplicationException("Expected 1 component in texcoord!");
        if (!Description.Height.HasValue) throw new ApplicationException("Image height is undefined!");

        var x = texcoord[0] * Description.Width + 0.5;
        var px1 = (int)Math.Floor(x);
        var px2 = (int)Math.Ceiling(x);

        // TODO: Add clamp/wrap support
        px1 = Math.Clamp(px1, 0, Description.Width-1);
        px2 = Math.Clamp(px2, 0, Description.Width-1);

        var pixel1 = data[px1];
        var pixel2 = data[px2];

        pixel = new TPixel();
        pixel.Interpolate(pixel1, pixel2, (float)(x % 1d));
    }
}
