using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LutBaker.Internal.Writing
{
    internal interface IImageWriter
    {
        PixelFormat PixelFormat {get; set;}
        PixelType PixelType {get; set;}

        void Initialize(in int width, in int height);
        void AppendPixel(int x, int y, double[] pixel);
        Task CompleteAsync(CancellationToken token = default);
    }

    internal abstract class ImageWriterBase : IImageWriter
    {
        protected readonly Stream Stream;

        public PixelFormat PixelFormat {get; set;}
        public PixelType PixelType {get; set;}


        protected ImageWriterBase(Stream stream)
        {
            Stream = stream;
        }

        public abstract void AppendPixel(int x, int y, double[] pixel);

        public virtual void Initialize(in int width, in int height) {}
        public virtual Task CompleteAsync(CancellationToken token = default) => Task.CompletedTask;
    }
}
