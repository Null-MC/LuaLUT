using LuaLUT.Internal.PixelWriter;
using LuaLUT.Internal.Samplers;

namespace LuaLUT.Internal.Pixels;

internal struct RG_FLOAT : IRawPixel
{
    public float R;
    public float G;


    public void Read(PixelReader reader)
    {
        reader.ReadFloat(out R);
        reader.ReadFloat(out G);
    }

    public int Apply(out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        pixelR = R;
        pixelG = G;
        pixelB = 0d;
        pixelA = 0d;
        return 2;
    }

    public void Interpolate(in RG_FLOAT pixelA, in RG_FLOAT pixelB, in float mix)
    {
        R = pixelA.R * (1f - mix) + pixelB.R * mix;
        G = pixelA.G * (1f - mix) + pixelB.G * mix;
    }

    void IRawPixel.Interpolate(in IRawPixel pixelA, in IRawPixel pixelB, in float mix) => Interpolate((RG_FLOAT)pixelA, (RG_FLOAT)pixelB, in mix);
}
