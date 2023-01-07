using LuaLUT.Internal.PixelWriter;
using LuaLUT.Internal.Samplers;
using System;

namespace LuaLUT.Internal.Pixels;

internal struct RGB_HALF_FLOAT : IRawPixel
{
    public Half R;
    public Half G;
    public Half B;
    

    public void Read(PixelReader reader)
    {
        reader.ReadHalf(out R);
        reader.ReadHalf(out G);
        reader.ReadHalf(out B);
    }

    public int Apply(out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        pixelR = (double)R;
        pixelG = (double)G;
        pixelB = (double)B;
        pixelA = 0d;
        return 3;
    }

    public void Interpolate(in RGB_HALF_FLOAT pixelA, in RGB_HALF_FLOAT pixelB, in float mix)
    {
        R = (Half)((float)pixelA.R * (1f - mix) + (float)pixelB.R * mix);
        G = (Half)((float)pixelA.G * (1f - mix) + (float)pixelB.G * mix);
        B = (Half)((float)pixelA.B * (1f - mix) + (float)pixelB.B * mix);
    }

    void IRawPixel.Interpolate(in IRawPixel pixelA, in IRawPixel pixelB, in float mix) => Interpolate((RGB_HALF_FLOAT)pixelA, (RGB_HALF_FLOAT)pixelB, in mix);
}
