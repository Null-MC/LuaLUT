using LuaLUT.Internal.PixelWriter;
using LuaLUT.Internal.Samplers;
using System;

namespace LuaLUT.Internal.Pixels;

internal struct RGB_BYTE : IRawPixel
{
    public byte R;
    public byte G;
    public byte B;
    

    public void Read(PixelReader reader)
    {
        reader.ReadUByte(out R);
        reader.ReadUByte(out G);
        reader.ReadUByte(out B);
    }

    public int Apply(out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        pixelR = (double)R / byte.MaxValue;
        pixelG = (double)G / byte.MaxValue;
        pixelB = (double)B / byte.MaxValue;
        pixelA = 0d;
        return 3;
    }

    public void Interpolate(in RGB_BYTE pixelA, in RGB_BYTE pixelB, in float mix)
    {
        R = (byte)Math.Clamp(pixelA.R * (1f - mix) + pixelB.R * mix, byte.MinValue, byte.MaxValue);
        G = (byte)Math.Clamp(pixelA.G * (1f - mix) + pixelB.G * mix, byte.MinValue, byte.MaxValue);
        B = (byte)Math.Clamp(pixelA.B * (1f - mix) + pixelB.B * mix, byte.MinValue, byte.MaxValue);
    }

    void IRawPixel.Interpolate(in IRawPixel pixelA, in IRawPixel pixelB, in float mix) => Interpolate((RGB_BYTE)pixelA, (RGB_BYTE)pixelB, in mix);
}
