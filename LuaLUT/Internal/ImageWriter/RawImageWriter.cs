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

    public override async Task ProcessAsync(string luaScript, int width, int height, CancellationToken token = default)
    {
        var stride = PixelFormats.GetStride(PixelFormat);

        var processPixelRowBlock = new TransformBlock<int, T[]>(y => {
            var rowSpan = new T[width*stride];

            void SetRowPixel(int i, object value) {
                rowSpan[i] = value is T valueT ? valueT : (T)Convert.ChangeType(value, typeof(T));
            }

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
                if (stride >= 1) SetRowPixel(i  , pixel[0]);
                if (stride >= 2) SetRowPixel(i+1, pixel[1]);
                if (stride >= 3) SetRowPixel(i+2, pixel[2]);
                if (stride >= 4) SetRowPixel(i+3, pixel[3]);
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
}
