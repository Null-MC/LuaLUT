using System;
using System.Collections.Generic;

namespace LutBaker.Internal.Writing
{
    internal enum PixelType
    {
        BYTE,
        SHORT,
        INT,
        HALF_FLOAT,
        FLOAT,
        // TODO: ...
    }

    internal static class PixelTypes
    {
        private static readonly Dictionary<string, PixelType> parseMap = new(StringComparer.InvariantCultureIgnoreCase) {
            ["BYTE"] = PixelType.BYTE,
            ["SHORT"] = PixelType.SHORT,
            ["INT"] = PixelType.INT,
            ["HALF_FLOAT"] = PixelType.HALF_FLOAT,
            ["FLOAT"] = PixelType.FLOAT,
            // TODO: ...
        };


        public static PixelType Parse(string name)
        {
            if (parseMap.TryGetValue(name, out var pixelType)) return pixelType;
            throw new ApplicationException($"Unknown pixel type '{name}'!");
        }
    }
}
