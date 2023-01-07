using LuaLUT.Internal.PixelWriter;
using System;

namespace LuaLUT.Internal.Samplers;

internal class SamplerDescription
{
    public string Filename {get; set;}
    public PixelFormat PixelFormat {get; set;}
    public PixelType PixelType {get; set;}
    public int Dimensions {get; set;}
    public int Width {get; set;}
    public int? Height {get; set;}
    public int? Depth {get; set;}


    public void Parse(in string spec)
    {
        var parts = spec.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Dimensions = parts.Length switch {
            6 => 3,
            5 => 2,
            4 => 1,
            _ => throw new ApplicationException("Invalid sampler definition!"),
        };

        Filename = parts[0];

        Width = int.Parse(parts[1]);

        if (Dimensions >= 2)
            Height = int.Parse(parts[2]);

        if (Dimensions >= 3)
            Depth = int.Parse(parts[3]);

        PixelFormat = PixelFormats.Parse(parts[^2]);
        PixelType = PixelTypes.Parse(parts[^1]);
    }
}
