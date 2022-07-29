using System;
using System.Collections.Generic;

namespace LuaLUT.Internal.Writing
{
    internal enum ImageType
    {
        Raw,
        Png,
        Bmp,
    }

    internal static class ImageTypes
    {
        private static readonly Dictionary<string, ImageType> parseMap = new(StringComparer.InvariantCultureIgnoreCase) {
            ["RAW"] = ImageType.Raw,
            ["PNG"] = ImageType.Png,
            ["BMP"] = ImageType.Bmp,
        };


        public static ImageType Parse(string name)
        {
            if (parseMap.TryGetValue(name, out var imageType)) return imageType;
            throw new ApplicationException($"Unknown image type '{name}'!");
        }
    }
}
