using LuaLUT.Internal.PixelWriter;
using System;
using System.IO;

namespace LuaLUT.Internal.Samplers;

internal class Sampler3D<TPixel> : SamplerBase<TPixel>
    where TPixel : struct, IRawPixel
{
    private TPixel[,,] data;


    public Sampler3D(SamplerDescription description) : base(description) {}

    protected override void LoadImageData(Stream stream)
    {
        if (!Description.Height.HasValue) throw new ApplicationException("Image height is undefined!");
        if (!Description.Depth.HasValue) throw new ApplicationException("Image depth is undefined!");

        data = new TPixel[Description.Width, Description.Height.Value, Description.Depth.Value];

        var reader = new PixelReader(stream);

        for (var z = 0; z < Description.Depth.Value; z++) {
            for (var y = 0; y < Description.Height.Value; y++) {
                for (var x = 0; x < Description.Width; x++) {
                    data[x, y, z].Read(reader);
                }
            }
        }
    }

    protected override void GetPixel(in int[] texcoord, out TPixel pixel)
    {
        if (texcoord == null) throw new ArgumentNullException(nameof(texcoord));
        pixel = data[texcoord[0], texcoord[1], texcoord[2]];
    }

    protected override void GetPixelInterpolated(in double[] texcoord, out TPixel pixel)
    {
        if (texcoord == null) throw new ArgumentNullException(nameof(texcoord));
        if (texcoord.Length != 3) throw new ApplicationException("Expected 3 components in texcoord!");
        if (!Description.Height.HasValue) throw new ApplicationException("Image height is undefined!");
        if (!Description.Depth.HasValue) throw new ApplicationException("Image depth is undefined!");

        var x = texcoord[0] * Description.Width + 0.5;
        var px1 = (int)Math.Floor(x);
        var px2 = (int)Math.Ceiling(x);

        var y = texcoord[1] * Description.Height.Value + 0.5;
        var py1 = (int)Math.Floor(y);
        var py2 = (int)Math.Ceiling(y);

        var z = texcoord[2] * Description.Depth.Value + 0.5;
        var pz1 = (int)Math.Floor(z);
        var pz2 = (int)Math.Ceiling(z);

        // TODO: Add clamp/wrap support
        px1 = Math.Clamp(px1, 0, Description.Width-1);
        px2 = Math.Clamp(px2, 0, Description.Width-1);
        py1 = Math.Clamp(py1, 0, Description.Height.Value-1);
        py2 = Math.Clamp(py2, 0, Description.Height.Value-1);
        pz1 = Math.Clamp(pz1, 0, Description.Depth.Value-1);
        pz2 = Math.Clamp(pz2, 0, Description.Depth.Value-1);

        var pixel_x1_y1_z1 = data[px1, py1, pz1];
        var pixel_x2_y1_z1 = data[px2, py1, pz1];
        var pixel_x1_y2_z1 = data[px1, py2, pz1];
        var pixel_x2_y2_z1 = data[px2, py2, pz1];
        var pixel_x1_y1_z2 = data[px1, py1, pz2];
        var pixel_x2_y1_z2 = data[px2, py1, pz2];
        var pixel_x1_y2_z2 = data[px1, py2, pz2];
        var pixel_x2_y2_z2 = data[px2, py2, pz2];

        var pixel_y1_z1 = new TPixel();
        pixel_y1_z1.Interpolate(pixel_x1_y1_z1, pixel_x2_y1_z1, (float)(x % 1d));

        var pixel_y2_z1 = new TPixel();
        pixel_y2_z1.Interpolate(pixel_x1_y2_z1, pixel_x2_y2_z1, (float)(x % 1d));

        var pixel_z1 = new TPixel();
        pixel_z1.Interpolate(pixel_y1_z1, pixel_y2_z1, (float)(y % 1d));

        var pixel_y1_z2 = new TPixel();
        pixel_y1_z2.Interpolate(pixel_x1_y1_z2, pixel_x2_y1_z2, (float)(x % 1d));

        var pixel_y2_z2 = new TPixel();
        pixel_y2_z2.Interpolate(pixel_x1_y2_z2, pixel_x2_y2_z2, (float)(x % 1d));

        var pixel_z2 = new TPixel();
        pixel_z2.Interpolate(pixel_y1_z2, pixel_y2_z2, (float)(y % 1d));

        pixel = new TPixel();
        pixel.Interpolate(pixel_z1, pixel_z2, (float)(z % 1d));
    }
}
