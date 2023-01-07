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

    public int DepthSlice {get; set;}


    public StandardImageWriter(Stream stream, ImageType imageType) : base(stream)
    {
        this.imageType = imageType;

        DepthSlice = 0;
    }

    public override async Task ProcessAsync(string luaScript, CancellationToken token = default)
    {
        // TODO: add special layout for 3D grid?

        using var image = CreateImage(ImageWidth, ImageHeight);

        image.Mutate(context => {
            // TODO: if integer format, don't use vec4

            context.ProcessPixelRowsAsVector4((row, point) => {
                using var processor = new LuaScriptProcessor(Samplers) {
                    IncludedFiles = IncludedFiles,
                    CustomVariables = CustomVariables,
                    Dimensions = ImageDimensions,
                    Width = ImageWidth,
                    Height = ImageHeight,
                    Depth = ImageDepth,
                    Script = luaScript,
                };

                processor.Initialize();

                for (var x = 0; x < ImageWidth; x++) {
                    var pixel = ImageDimensions switch {
                        3 => processor.ProcessPixel(point.X + x, point.Y, DepthSlice),
                        2 => processor.ProcessPixel(point.X + x, point.Y),
                        1 => processor.ProcessPixel(point.X + x),
                        _ => throw new ApplicationException($"Unsupported dimension count '{ImageDimensions}'!"),
                    };

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
                throw new ApplicationException("Only BMP and PNG formats are currently supported for non-raw images!");
        }
    }

    private Image CreateImage(in int width, in int height)
    {
        return PixelType switch {
            PixelType.UNSIGNED_BYTE => PixelFormat switch {
                PixelFormat.R_NORM => new Image<L8>(Configuration.Default, width, height),
                PixelFormat.RG_NORM => new Image<Rgb24>(Configuration.Default, width, height),
                PixelFormat.RGB_NORM => new Image<Rgb24>(Configuration.Default, width, height),
                PixelFormat.RGBA_NORM => new Image<Rgba32>(Configuration.Default, width, height),
                _ => throw new ApplicationException("Unsupported")
            },
            PixelType.HALF_FLOAT => PixelFormat switch {
                PixelFormat.R_NORM => new Image<L16>(Configuration.Default, width, height),
                PixelFormat.RG_NORM => new Image<Rgb48>(Configuration.Default, width, height),
                PixelFormat.RGB_NORM => new Image<Rgb48>(Configuration.Default, width, height),
                PixelFormat.RGBA_NORM => new Image<Rgba64>(Configuration.Default, width, height),
                _ => throw new ApplicationException("Unsupported")
            },
            _ => throw new ApplicationException("Unsupported")
        };
    }
}
