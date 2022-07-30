using System;
using System.Collections.Generic;
using System.Linq;

namespace LuaLUT.Internal.PixelWriter;

public enum PixelFormat
{
    UNDEFINED,
    R_NORM,
    RG_NORM,
    RGB_NORM,
    RGBA_NORM,
    BGR_NORM,
    BGRA_NORM,
    R_INT,
    RG_INT,
    RGB_INT,
    RGBA_INT,
    BGR_INT,
    BGRA_INT,
}

internal static class PixelFormats
{
    private static readonly Dictionary<string, PixelFormat> parseMap = new(StringComparer.InvariantCultureIgnoreCase) {
        ["R"] = PixelFormat.R_NORM,
        ["R_NORM"] = PixelFormat.R_NORM,
        ["RED"] = PixelFormat.R_NORM,
        ["RED_NORM"] = PixelFormat.R_NORM,
        ["RG"] = PixelFormat.RG_NORM,
        ["RG_NORM"] = PixelFormat.RG_NORM,
        ["RGB"] = PixelFormat.RGB_NORM,
        ["RGB_NORM"] = PixelFormat.RGB_NORM,
        ["RGBA"] = PixelFormat.RGBA_NORM,
        ["RGBA_NORM"] = PixelFormat.RGBA_NORM,
        ["BGR"] = PixelFormat.BGR_NORM,
        ["BGR_NORM"] = PixelFormat.BGR_NORM,
        ["BGRA"] = PixelFormat.BGRA_NORM,
        ["BGRA_NORM"] = PixelFormat.BGRA_NORM,
        ["R_INT"] = PixelFormat.R_INT,
        ["R_INTEGER"] = PixelFormat.R_INT,
        ["RED_INT"] = PixelFormat.R_INT,
        ["RED_INTEGER"] = PixelFormat.R_INT,
        ["RG_INT"] = PixelFormat.RG_INT,
        ["RG_INTEGER"] = PixelFormat.RG_INT,
        ["RGB_INT"] = PixelFormat.RGB_INT,
        ["RGB_INTEGER"] = PixelFormat.RGB_INT,
        ["RGBA_INT"] = PixelFormat.RGBA_INT,
        ["RGBA_INTEGER"] = PixelFormat.RGBA_INT,
        ["BGR_INT"] = PixelFormat.BGR_INT,
        ["BGR_INTEGER"] = PixelFormat.BGR_INT,
        ["BGRA_INT"] = PixelFormat.BGRA_INT,
        ["BGRA_INTEGER"] = PixelFormat.BGRA_INT,
    };


    public static PixelFormat Parse(in string name)
    {
        if (parseMap.TryGetValue(name, out var pixelFormat)) return pixelFormat;
        throw new ApplicationException($"Unknown pixel format '{name}'!");
    }

    public static int GetStride(in PixelFormat pixelFormat)
    {
        switch (pixelFormat) {
            case PixelFormat.R_NORM:
            case PixelFormat.R_INT:
                return 1;
            case PixelFormat.RG_NORM:
            case PixelFormat.RG_INT:
                return 2;
            case PixelFormat.RGB_NORM:
            case PixelFormat.RGB_INT:
            case PixelFormat.BGR_NORM:
            case PixelFormat.BGR_INT:
                return 3;
            case PixelFormat.RGBA_NORM:
            case PixelFormat.RGBA_INT:
            case PixelFormat.BGRA_NORM:
            case PixelFormat.BGRA_INT:
                return 4;
            default:
                throw new ApplicationException("Unsupported");
        }
    }

    public static bool IsNormalized(in PixelFormat pixelFormat)
    {
        return normalizedFormats.Contains(pixelFormat);
    }

    private static readonly PixelFormat[] normalizedFormats = {
        PixelFormat.R_NORM,
        PixelFormat.RG_NORM,
        PixelFormat.RGB_NORM,
        PixelFormat.RGBA_NORM,
        PixelFormat.BGR_NORM,
        PixelFormat.BGRA_NORM,
    };
}
