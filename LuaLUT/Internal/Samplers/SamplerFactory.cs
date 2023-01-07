using LuaLUT.Internal.Pixels;
using LuaLUT.Internal.PixelWriter;
using System;

namespace LuaLUT.Internal.Samplers;

internal static class SamplerFactory
{
    public static ISampler Get(SamplerDescription description)
    {
        return description.PixelType switch {
            PixelType.BYTE => description.PixelFormat switch {
                //PixelFormat.R_NORM => BuildSampler<R_BYTE>(description),
                //PixelFormat.RG_NORM => BuildSampler<RG_BYTE>(description),
                PixelFormat.RGB_NORM => BuildSampler<RGB_BYTE>(description),
                //PixelFormat.RGBA_NORM => BuildSampler<RGBA_BYTE>(description),
                _ => throw new ApplicationException($"Unsupported pixel format '{description.PixelFormat}'!"),
            },
            PixelType.FLOAT => description.PixelFormat switch {
                PixelFormat.R_NORM => BuildSampler<R_FLOAT>(description),
                PixelFormat.RG_NORM => BuildSampler<RG_FLOAT>(description),
                PixelFormat.RGB_NORM => BuildSampler<RGB_FLOAT>(description),
                PixelFormat.RGBA_NORM => BuildSampler<RGBA_FLOAT>(description),
                _ => throw new ApplicationException($"Unsupported pixel format '{description.PixelFormat}'!"),
            },
            PixelType.HALF_FLOAT => description.PixelFormat switch {
                //PixelFormat.R_NORM => BuildSampler<R_HALF_FLOAT>(description),
                //PixelFormat.RG_NORM => BuildSampler<RG_HALF_FLOAT>(description),
                PixelFormat.RGB_NORM => BuildSampler<RGB_HALF_FLOAT>(description),
                //PixelFormat.RGBA_NORM => BuildSampler<RGBA_HALF_FLOAT>(description),
                _ => throw new ApplicationException($"Unsupported pixel format '{description.PixelFormat}'!"),
            },
            _ => throw new ApplicationException($"Unsupported pixel type '{description.PixelType}'!")
        };
    }

    private static ISampler BuildSampler<TPixel>(SamplerDescription description)
        where TPixel : struct, IRawPixel
    {
        return description.Dimensions switch {
            1 => new Sampler1D<TPixel>(description),
            2 => new Sampler2D<TPixel>(description),
            3 => new Sampler3D<TPixel>(description),
            _ => throw new ApplicationException($"Unsupported dimension count '{description.Dimensions}'!"),
        };
    }
}