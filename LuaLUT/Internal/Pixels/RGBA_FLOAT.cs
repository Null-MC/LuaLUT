using LuaLUT.Internal.PixelWriter;
using LuaLUT.Internal.Samplers;

namespace LuaLUT.Internal.Pixels;

internal struct RGBA_FLOAT : IRawPixel
{
    public float R;
    public float G;
    public float B;
    public float A;


    public void Read(PixelReader reader)
    {
        reader.ReadFloat(out R);
        reader.ReadFloat(out G);
        reader.ReadFloat(out B);
        reader.ReadFloat(out A);
    }

    public int Apply(out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        pixelR = R;
        pixelG = G;
        pixelB = B;
        pixelA = A;
        return 4;
    }

    public void Interpolate(in RGBA_FLOAT pixelA, in RGBA_FLOAT pixelB, in float mix)
    {
        R = pixelA.R * (1f - mix) + pixelB.R * mix;
        G = pixelA.G * (1f - mix) + pixelB.G * mix;
        B = pixelA.B * (1f - mix) + pixelB.B * mix;
        A = pixelA.A * (1f - mix) + pixelB.A * mix;
    }

    void IRawPixel.Interpolate(in IRawPixel pixelA, in IRawPixel pixelB, in float mix) => Interpolate((RGBA_FLOAT)pixelA, (RGBA_FLOAT)pixelB, in mix);
}
