using System;
using System.Collections.Generic;

namespace LuaLUT.Internal.PixelWriter;

public enum PixelType
{
    UNDEFINED,
    BYTE,
    SHORT,
    INT,
    HALF_FLOAT,
    FLOAT,
    UNSIGNED_BYTE,
    UNSIGNED_BYTE_3_3_2,
    UNSIGNED_BYTE_2_3_3_REV,
    UNSIGNED_SHORT,
    UNSIGNED_SHORT_5_6_5,
    UNSIGNED_SHORT_5_6_5_REV,
    UNSIGNED_SHORT_4_4_4_4,
    UNSIGNED_SHORT_4_4_4_4_REV,
    UNSIGNED_SHORT_5_5_5_1,
    UNSIGNED_SHORT_1_5_5_5_REV,
    UNSIGNED_INT,
    UNSIGNED_INT_8_8_8_8,
    UNSIGNED_INT_8_8_8_8_REV,
    UNSIGNED_INT_10_10_10_2,
    UNSIGNED_INT_2_10_10_10_REV,
}

internal static class PixelTypes
{
    private static readonly Dictionary<string, PixelType> parseMap = new(StringComparer.InvariantCultureIgnoreCase) {
        ["BYTE"] = PixelType.BYTE,
        ["SHORT"] = PixelType.SHORT,
        ["INT"] = PixelType.INT,
        ["HALF_FLOAT"] = PixelType.HALF_FLOAT,
        ["FLOAT"] = PixelType.FLOAT,
        ["UNSIGNED_BYTE"] = PixelType.UNSIGNED_BYTE,
        ["UNSIGNED_BYTE_3_3_2"] = PixelType.UNSIGNED_BYTE_3_3_2,
        ["UNSIGNED_BYTE_2_3_3_REV"] = PixelType.UNSIGNED_BYTE_2_3_3_REV,
        ["UNSIGNED_SHORT"] = PixelType.UNSIGNED_SHORT,
        ["UNSIGNED_SHORT_5_6_5"] = PixelType.UNSIGNED_SHORT_5_6_5,
        ["UNSIGNED_SHORT_5_6_5_REV"] = PixelType.UNSIGNED_SHORT_5_6_5_REV,
        ["UNSIGNED_SHORT_4_4_4_4"] = PixelType.UNSIGNED_SHORT_4_4_4_4,
        ["UNSIGNED_SHORT_4_4_4_4_REV"] = PixelType.UNSIGNED_SHORT_4_4_4_4_REV,
        ["UNSIGNED_SHORT_5_5_5_1"] = PixelType.UNSIGNED_SHORT_5_5_5_1,
        ["UNSIGNED_SHORT_1_5_5_5_REV"] = PixelType.UNSIGNED_SHORT_1_5_5_5_REV,
        ["UNSIGNED_INT"] = PixelType.UNSIGNED_INT,
        ["UNSIGNED_INT_8_8_8_8"] = PixelType.UNSIGNED_INT_8_8_8_8,
        ["UNSIGNED_INT_8_8_8_8_REV"] = PixelType.UNSIGNED_INT_8_8_8_8_REV,
        ["UNSIGNED_INT_10_10_10_2"] = PixelType.UNSIGNED_INT_10_10_10_2,
        ["UNSIGNED_INT_2_10_10_10_REV"] = PixelType.UNSIGNED_INT_2_10_10_10_REV,
    };


    public static PixelType Parse(string name)
    {
        if (parseMap.TryGetValue(name, out var pixelType)) return pixelType;
        throw new ApplicationException($"Unknown pixel type '{name}'!");
    }
}