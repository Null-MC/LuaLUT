using LuaLUT.Internal.PixelWriter;
using LuaLUT.Internal.Samplers;

namespace LuaLUT.Internal.Pixels;

internal struct RGB_FLOAT : IRawPixel
{
    public float R;
    public float G;
    public float B;
    

    public void Read(PixelReader reader)
    {
        reader.ReadFloat(out R);
        reader.ReadFloat(out G);
        reader.ReadFloat(out B);
    }

    public int Apply(out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        pixelR = R;
        pixelG = G;
        pixelB = B;
        pixelA = 0d;
        return 3;
    }

    public void Interpolate(in RGB_FLOAT pixelA, in RGB_FLOAT pixelB, in float mix)
    {
        R = pixelA.R * (1f - mix) + pixelB.R * mix;
        G = pixelA.G * (1f - mix) + pixelB.G * mix;
        B = pixelA.B * (1f - mix) + pixelB.B * mix;
    }

    void IRawPixel.Interpolate(in IRawPixel pixelA, in IRawPixel pixelB, in float mix) => Interpolate((RGB_FLOAT)pixelA, (RGB_FLOAT)pixelB, in mix);
}
