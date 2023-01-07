using LuaLUT.Internal.PixelWriter;

namespace LuaLUT.Internal.Samplers;

internal interface IRawPixel
{
    void Read(PixelReader reader);
    int Apply(out double pixelR, out double pixelG, out double pixelB, out double pixelA);
    void Interpolate(in IRawPixel pixelA, in IRawPixel pixelB, in float mix);
}
