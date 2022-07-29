using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LutBaker.Internal.Writing
{
    internal interface IImageWriter
    {
        PixelFormat PixelFormat {get; set;}
        PixelType PixelType {get; set;}

        Task ProcessAsync(LuaScriptProcessor processor, CancellationToken token = default);
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

        public abstract Task ProcessAsync(LuaScriptProcessor processor, CancellationToken token = default);
    }
}
