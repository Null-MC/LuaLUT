using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace LuaLUT.Tests.Internal
{
    internal static class PixelAssert
    {
        public static void RedEquals<T>(this Image<T> image, int x, int y, byte expectedValue) where T : unmanaged, IPixel<T>
        {
            var pixel = new Rgba32();
            image[x, y].ToRgba32(ref pixel);
            Assert.Equal(expectedValue, pixel.R);
        }
    }
}
