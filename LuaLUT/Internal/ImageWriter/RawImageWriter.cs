using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using LuaLUT.Internal.PixelWriter;

namespace LuaLUT.Internal.ImageWriter;

internal class RawImageWriter<T> : ImageWriterBase
{
    private const int MaxDegreeOfParallelism = 8;

    private readonly PixelWriterBase<T> pixelWriter;


    public RawImageWriter(PixelWriterBase<T> pixelWriter) : base(pixelWriter.Stream)
    {
        this.pixelWriter = pixelWriter;
    }

    public override async Task ProcessAsync(string luaScript, int width, int height, CancellationToken token = default)
    {
        var stride = GetPixelStride();

        var processPixelRowBlock = new TransformBlock<int, T[]>(y => {
            var rowSpan = new T[width*stride];

            using var processor = new LuaScriptProcessor {
                CustomVariables = CustomVariables,
                Script = luaScript,
                Width = width,
                Height = height,
            };

            processor.Initialize();

            for (var x = 0; x < width; x++) {
                var pixel = processor.ProcessPixel(x, y);
                if (pixel.Length != stride) throw new ApplicationException($"Returned pixel length {pixel.Length} does not match expected stride length of {stride}!");

                var i = x * stride;
                if (stride >= 1) rowSpan[i  ] = (T)pixel[0];
                if (stride >= 2) rowSpan[i+1] = (T)pixel[1];
                if (stride >= 3) rowSpan[x+2] = (T)pixel[2];
                if (stride >= 4) rowSpan[x+3] = (T)pixel[3];
            }

            return rowSpan;
        }, new ExecutionDataflowBlockOptions {
            MaxDegreeOfParallelism = MaxDegreeOfParallelism,
        });

        var writeRowBlock = new ActionBlock<T[]>(row => {
            foreach (var pixel in row.Chunk(stride))
                pixelWriter.Write(pixel);
        });

        processPixelRowBlock.LinkTo(writeRowBlock, new DataflowLinkOptions {
            PropagateCompletion = true,
        });

        for (var y = 0; y < height; y++)
            processPixelRowBlock.Post(y);

        processPixelRowBlock.Complete();
        await writeRowBlock.Completion;
    }

    private int GetPixelStride()
    {
        switch (PixelFormat) {
            case PixelFormat.R_NORM:
            case PixelFormat.R_INT:
                return 1;
            case PixelFormat.RG_NORM:
            case PixelFormat.RG_INT:
                return 2;
            case PixelFormat.RGB_NORM:
            case PixelFormat.RGB_INT:
            case PixelFormat.BGR_NORM:
            case PixelFormat.BGR_INT:
                return 3;
            case PixelFormat.RGBA_NORM:
            case PixelFormat.RGBA_INT:
            case PixelFormat.BGRA_NORM:
            case PixelFormat.BGRA_INT:
                return 4;
            default:
                throw new ApplicationException("Unsupported");
        }
    }
}
