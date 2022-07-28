using System;
using System.Collections.Generic;

namespace LutBaker.Internal.Writing
{
    internal enum PixelFormat
    {
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
            ["R"] = PixelFormat.R_INT,
            ["R_INT"] = PixelFormat.R_INT,
            ["R_INTEGER"] = PixelFormat.R_INT,
            ["RED"] = PixelFormat.R_INT,
            ["RED_INT"] = PixelFormat.R_INT,
            ["RED_INTEGER"] = PixelFormat.R_INT,
            ["RG"] = PixelFormat.RG_INT,
            ["RG_INT"] = PixelFormat.RG_INT,
            ["RG_INTEGER"] = PixelFormat.RG_INT,
            ["RGB"] = PixelFormat.RGB_INT,
            ["RGB_INT"] = PixelFormat.RGB_INT,
            ["RGB_INTEGER"] = PixelFormat.RGB_INT,
            ["RGBA"] = PixelFormat.RGBA_INT,
            ["RGBA_INT"] = PixelFormat.RGBA_INT,
            ["RGBA_INTEGER"] = PixelFormat.RGBA_INT,
            ["BGR"] = PixelFormat.BGR_INT,
            ["BGR_INT"] = PixelFormat.BGR_INT,
            ["BGR_INTEGER"] = PixelFormat.BGR_INT,
            ["BGRA"] = PixelFormat.BGRA_INT,
            ["BGRA_INT"] = PixelFormat.BGRA_INT,
            ["BGRA_INTEGER"] = PixelFormat.BGRA_INT,
        };


        public static PixelFormat Parse(string name)
        {
            if (parseMap.TryGetValue(name, out var pixelFormat)) return pixelFormat;
            throw new ApplicationException($"Unknown pixel format '{name}'!");
        }
    }
}
