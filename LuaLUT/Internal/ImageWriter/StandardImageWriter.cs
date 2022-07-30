using LuaLUT.Internal.PixelWriter;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LuaLUT.Internal.ImageWriter;

internal class StandardImageWriter : ImageWriterBase
{
    private readonly ImageType imageType;


    public StandardImageWriter(Stream stream, ImageType imageType) : base(stream)
    {
        this.imageType = imageType;
    }

    public override async Task ProcessAsync(string luaScript, int width, int height, CancellationToken token = default)
    {
        using var image = CreateImage(width, height);

        image.Mutate(context => {
            // TODO: if integer format, don't use vec4

            context.ProcessPixelRowsAsVector4((row, point) => {
                using var processor = new LuaScriptProcessor {
                    CustomVariables = CustomVariables,
                    Script = luaScript,
                    Width = width,
                    Height = height,
                };

                processor.Initialize();

                for (var x = 0; x < width; x++) {
                    var pixel = processor.ProcessPixel(point.X + x, point.Y);

                    if (PixelFormat is PixelFormat.R_NORM or PixelFormat.R_INT) {
                        var value = Convert.ToSingle((double)pixel[0]);
                        row[point.X + x].X = value;
                        row[point.X + x].Y = value;
                        row[point.X + x].Z = value;
                    }
                    else {
                        if (pixel.Length >= 1) row[point.X + x].X = Convert.ToSingle((double)pixel[0]);
                        if (pixel.Length >= 2) row[point.X + x].Y = Convert.ToSingle((double)pixel[1]);
                        if (pixel.Length >= 3) row[point.X + x].Z = Convert.ToSingle((double)pixel[2]);
                        if (pixel.Length >= 4) row[point.X + x].W = Convert.ToSingle((double)pixel[3]);
                    }
                }
            });
        });

        await Stream.FlushAsync(token);

        switch (imageType) {
            case ImageType.Png:
                await image.SaveAsPngAsync(Stream, token);
                break;
            case ImageType.Bmp:
                await image.SaveAsBmpAsync(Stream, token);
                break;
            default:
                throw new ApplicationException();
        }
    }

    private Image CreateImage(in int width, in int height)
    {
        switch (PixelType) {
            case PixelType.UNSIGNED_BYTE:
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
            case PixelType.HALF_FLOAT:
                switch (PixelFormat) {
                    case PixelFormat.R_NORM:
                        return new Image<L16>(Configuration.Default, width, height);
                    case PixelFormat.RG_NORM:
                        return new Image<Rgb48>(Configuration.Default, width, height);
                    case PixelFormat.RGB_NORM:
                        return new Image<Rgb48>(Configuration.Default, width, height);
                    case PixelFormat.RGBA_NORM:
                        return new Image<Rgba64>(Configuration.Default, width, height);
                    default:
                        throw new ApplicationException("Unsupported");
                }
            default:
                throw new ApplicationException("Unsupported");
        }
    }
}