using LuaLUT.Internal.PixelWriter;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace LuaLUT.Internal.ImageWriter;

internal class RawImageWriter<T> : ImageWriterBase
{
    private const int MaxDegreeOfParallelism = 8;

    private readonly PixelWriterBase<T> pixelWriter;


    public RawImageWriter(PixelWriterBase<T> pixelWriter) : base(pixelWriter.Stream)
    {
        this.pixelWriter = pixelWriter;
    }

    public override async Task ProcessAsync(string luaScript, CancellationToken token = default)
    {
        var stride = PixelFormats.GetStride(PixelFormat);

        var processPixelRowBlock = new TransformBlock<(int z, int y), T[]>(args => {
            var rowSpan = new T[ImageWidth*stride];

            void SetRowPixel(int i, object value) {
                rowSpan[i] = value is T valueT ? valueT : (T)Convert.ChangeType(value, typeof(T));
            }

            using var processor = new LuaScriptProcessor {
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
                    1 => processor.ProcessPixel(x),
                    2 => processor.ProcessPixel(x, args.y),
                    3 => processor.ProcessPixel(x, args.y, args.z),
                    _ => throw new ApplicationException($"Unsupported dimension count '{ImageDimensions}'!"),
                };

                if (pixel.Length != stride) throw new ApplicationException($"Returned pixel length {pixel.Length} does not match expected stride length of {stride}!");

                var i = x * stride;
                if (stride >= 1) SetRowPixel(i  , pixel[0]);
                if (stride >= 2) SetRowPixel(i+1, pixel[1]);
                if (stride >= 3) SetRowPixel(i+2, pixel[2]);
                if (stride >= 4) SetRowPixel(i+3, pixel[3]);
            }

            return rowSpan;
        }, new ExecutionDataflowBlockOptions {
            MaxDegreeOfParallelism = MaxDegreeOfParallelism,
            EnsureOrdered = true,
        });

        var writeRowBlock = new ActionBlock<T[]>(row => {
            foreach (var pixel in row.Chunk(stride))
                pixelWriter.Write(pixel);
        }, new ExecutionDataflowBlockOptions {
            EnsureOrdered = true,
        });

        processPixelRowBlock.LinkTo(writeRowBlock, new DataflowLinkOptions {
            PropagateCompletion = true,
        });

        var yMax = ImageDimensions >= 2 ? ImageHeight : 1;
        var zMax = ImageDimensions >= 3 ? ImageDepth : 1;

        for (var z = 0; z < zMax; z++)
            for (var y = 0; y < yMax; y++)
                processPixelRowBlock.Post((z, y));

        processPixelRowBlock.Complete();
        await writeRowBlock.Completion;
    }
}
