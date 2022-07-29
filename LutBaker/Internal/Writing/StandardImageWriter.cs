using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LutBaker.Internal.Writing
{
    internal class StandardImageWriter : ImageWriterBase
    {
        private readonly ImageType imageType;
        private Image imageHandle;


        public StandardImageWriter(Stream stream, ImageType imageType) : base(stream)
        {
            this.imageType = imageType;
        }

        public override async Task ProcessAsync(LuaScriptProcessor processor, CancellationToken token = default)
        {
            imageHandle = CreateImage(processor.Width, processor.Height);

            imageHandle.Mutate(context => {
                context.ProcessPixelRowsAsVector4((row, point) => {
                    for (var x = 0; x < imageHandle.Width; x++) {
                        var pixel = processor.ProcessPixel(point.X + x, point.Y);

                        if (pixel.Length >= 1) row[point.X + x].X = (float)pixel[0];
                        if (pixel.Length >= 2) row[point.X + x].Y = (float)pixel[1];
                        if (pixel.Length >= 3) row[point.X + x].Z = (float)pixel[2];
                        if (pixel.Length >= 4) row[point.X + x].W = (float)pixel[3];
                    }
                });
            });

            await Stream.FlushAsync(token);

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

        private Image CreateImage(in int width, in int height)
        {
            switch (PixelType) {
                case PixelType.BYTE:
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            return new Image<L8>(Configuration.Default, width, height);
                        case PixelFormat.RG_NORM:
                            return new Image<Rgb24>(Configuration.Default, width, height);
                        case PixelFormat.RGB_NORM:
                            return new Image<Rgb24>(Configuration.Default, width, height);
                        case PixelFormat.RGBA_NORM:
                            return new Image<Rgba32>(Configuration.Default, width, height);
                        default:
                            throw new ApplicationException("Unsupported");
                    }
                default:
                    throw new ApplicationException("Unsupported");
            }
        }
    }
}
