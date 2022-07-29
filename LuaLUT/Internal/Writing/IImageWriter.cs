using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LuaLUT.Internal.Writing
{
    internal interface IImageWriter
    {
        Dictionary<string, object> CustomVariables {get; set;}
        PixelFormat PixelFormat {get; set;}
        PixelType PixelType {get; set;}

        Task ProcessAsync(string luaScript, int width, int height, CancellationToken token = default);
    }

    internal abstract class ImageWriterBase : IImageWriter
    {
        protected readonly Stream Stream;

        public Dictionary<string, object> CustomVariables {get; set;}
        public PixelFormat PixelFormat {get; set;}
        public PixelType PixelType {get; set;}


        protected ImageWriterBase(Stream stream)
        {
            Stream = stream;

            CustomVariables = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        public abstract Task ProcessAsync(string luaScript, int width, int height, CancellationToken token = default);
    }
}
