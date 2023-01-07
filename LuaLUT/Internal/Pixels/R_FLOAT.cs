using LuaLUT.Internal.PixelWriter;
using LuaLUT.Internal.Samplers;

namespace LuaLUT.Internal.Pixels;

internal struct R_FLOAT : IRawPixel
{
    public float R;


    public void Read(PixelReader reader)
    {
        reader.ReadFloat(out R);
    }

    public int Apply(out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        pixelR = R;
        pixelG = 0d;
        pixelB = 0d;
        pixelA = 0d;
        return 1;
    }

    public void Interpolate(in R_FLOAT pixelA, in R_FLOAT pixelB, in float mix)
    {
        R = pixelA.R * (1f - mix) + pixelB.R * mix;
    }

    void IRawPixel.Interpolate(in IRawPixel pixelA, in IRawPixel pixelB, in float mix) => Interpolate((R_FLOAT)pixelA, (R_FLOAT)pixelB, in mix);
}
