using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace LuaLUT.Tests.Internal
{
    internal static class PixelAssert
    {
        public static void ValueEquals(this Image<L8> image, int x, int y, byte expectedValue)
        {
            Assert.Equal(expectedValue, image[x, y].PackedValue);
        }

        public static void ValueEquals(this Image<L16> image, int x, int y, ushort expectedValue)
        {
            Assert.Equal(expectedValue, image[x, y].PackedValue);
        }

        public static void RedEquals<T>(this Image<T> image, int x, int y, byte expectedValue) where T : unmanaged, IPixel<T>
        {
            var pixel = new Rgba32();
            image[x, y].ToRgba32(ref pixel);
            Assert.Equal(expectedValue, pixel.R);
        }

        public static void RedEquals<T>(this Image<T> image, int x, int y, short expectedValue) where T : unmanaged, IPixel<T>
        {
            var pixel = new Rgba32();
            image[x, y].ToRgba32(ref pixel);
            Assert.Equal(expectedValue, pixel.R);
        }
    }
}
