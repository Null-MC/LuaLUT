using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace LutBaker.Internal.Writing
{
    internal class StandardImageWriter : ImageWriterBase
    {
        private readonly ImageType imageType;
        private Action<int, int, double[]> setPixelFunc;
        private Image imageHandle;


        public StandardImageWriter(Stream stream, ImageType imageType) : base(stream)
        {
            this.imageType = imageType;
        }

        public override void Initialize(in int width, in int height)
        {
            imageHandle = Initialize2(width, height);
        }

        public Image Initialize2(in int width, in int height)
        {
            switch (PixelType) {
                case PixelType.BYTE:
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            var imageL8 = new Image<L8>(Configuration.Default, width, height);
                            setPixelFunc = (x, y, p) => {
                                var data = new Vector4((float)p[0], (float)p[0], (float)p[0], 1f);
                                imageL8[x, y].FromScaledVector4(data);
                            };
                            return imageL8;
                        case PixelFormat.RG_NORM:
                            var imageRg16 = new Image<Rgb24>(Configuration.Default, width, height);
                            setPixelFunc = (x, y, p) => {
                                var data = new Vector4((float)p[0], (float)p[1], 0f, 1f);
                                imageRg16[x, y].FromScaledVector4(data);
                            };
                            return imageRg16;
                        case PixelFormat.RGB_NORM:
                            var imageRgb24 = new Image<Rgb24>(Configuration.Default, width, height);
                            setPixelFunc = (x, y, p) => {
                                var data = new Vector4((float)p[0], (float)p[1], (float)p[2], 1f);
                                imageRgb24[x, y].FromScaledVector4(data);
                            };
                            return imageRgb24;
                        case PixelFormat.RGBA_NORM:
                            var imageRgba32 = new Image<Rgba32>(Configuration.Default, width, height);
                            setPixelFunc = (x, y, p) => {
                                var data = new Vector4((float)p[0], (float)p[1], (float)p[2], (float)p[3]);
                                imageRgba32[x, y].FromScaledVector4(data);
                            };
                            return imageRgba32;
                        default:
                            throw new ApplicationException("Unsupported");
                    }
                default:
                    throw new ApplicationException("Unsupported");
            }
        }

        public override void AppendPixel(int x, int y, double[] pixelData)
        {
            setPixelFunc(x, y, pixelData);
        }

        public override async Task CompleteAsync(CancellationToken token = default)
        {
            switch (imageType) {
                case ImageType.Png:
                    await imageHandle.SaveAsPngAsync(Stream, token);
                    break;
                case ImageType.Bmp:
                    await imageHandle.SaveAsBmpAsync(Stream, token);
                    break;
                default:
                    throw new ApplicationException();
            }
        }
    }
}
