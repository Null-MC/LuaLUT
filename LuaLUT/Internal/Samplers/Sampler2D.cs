using LuaLUT.Internal.PixelWriter;
using System;
using System.IO;

namespace LuaLUT.Internal.Samplers;

internal class Sampler2D<TPixel> : SamplerBase<TPixel>
    where TPixel : struct, IRawPixel
{
    private TPixel[,] data;


    public Sampler2D(SamplerDescription description) : base(description) {}

    protected override void LoadImageData(Stream stream)
    {
        if (!Description.Height.HasValue) throw new ApplicationException("Image height is undefined!");

        data = new TPixel[Description.Width, Description.Height.Value];

        var reader = new PixelReader(stream);

        for (var y = 0; y < Description.Height.Value; y++) {
            for (var x = 0; x < Description.Width; x++) {
                data[x, y].Read(reader);
            }
        }
    }

    protected override void GetPixel(in int[] texcoord, out TPixel pixel)
    {
        if (texcoord == null) throw new ArgumentNullException(nameof(texcoord));
        pixel = data[texcoord[0], texcoord[1]];
    }

    protected override void GetPixelInterpolated(in double[] texcoord, out TPixel pixel)
    {
        if (texcoord == null) throw new ArgumentNullException(nameof(texcoord));
        if (texcoord.Length != 2) throw new ApplicationException("Expected 2 components in texcoord!");
        if (!Description.Height.HasValue) throw new ApplicationException("Image height is undefined!");

        var x = texcoord[0] * Description.Width + 0.5;
        var px1 = (int)Math.Floor(x);
        var px2 = (int)Math.Ceiling(x);

        var y = texcoord[1] * Description.Height.Value + 0.5;
        var py1 = (int)Math.Floor(y);
        var py2 = (int)Math.Ceiling(y);

        // TODO: Add clamp/wrap support
        px1 = Math.Clamp(px1, 0, Description.Width-1);
        px2 = Math.Clamp(px2, 0, Description.Width-1);
        py1 = Math.Clamp(py1, 0, Description.Height.Value-1);
        py2 = Math.Clamp(py2, 0, Description.Height.Value-1);

        var pixel_x1_y1 = data[px1, py1];
        var pixel_x2_y1 = data[px2, py1];
        var pixel_x1_y2 = data[px1, py2];
        var pixel_x2_y2 = data[px2, py2];

        var pixel_y1 = new TPixel();
        pixel_y1.Interpolate(pixel_x1_y1, pixel_x2_y1, (float)(x % 1d));

        var pixel_y2 = new TPixel();
        pixel_y2.Interpolate(pixel_x1_y2, pixel_x2_y2, (float)(x % 1d));

        pixel = new TPixel();
        pixel.Interpolate(pixel_y1, pixel_y2, (float)(y % 1d));
    }
}
