using LuaLUT.Internal.PixelWriter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LuaLUT.Internal.ImageWriter;

internal interface IImageWriter
{
    int ImageWidth {get; set;}
    int ImageHeight {get; set;}
    int ImageDepth {get; set;}
    int ImageDimensions {get; set;}
    IList<string> IncludedFiles {get;}
    IDictionary<string, object> CustomVariables {get;}

    Task ProcessAsync(string luaScript, CancellationToken token = default);
}

internal abstract class ImageWriterBase : IImageWriter
{
    protected readonly Stream Stream;

    public int ImageWidth {get; set;}
    public int ImageHeight {get; set;}
    public int ImageDepth {get; set;}
    public int ImageDimensions {get; set;}
    public List<string> IncludedFiles {get; set;}
    public Dictionary<string, object> CustomVariables {get; set;}
    public PixelFormat PixelFormat {get; set;}
    public PixelType PixelType {get; set;}

    IList<string> IImageWriter.IncludedFiles => IncludedFiles;
    IDictionary<string, object> IImageWriter.CustomVariables => CustomVariables;


    protected ImageWriterBase(Stream stream)
    {
        Stream = stream;

        IncludedFiles = new List<string>();
        CustomVariables = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        ImageDimensions = 2;
        ImageWidth = ImageHeight = ImageDepth = 1;
    }

    public abstract Task ProcessAsync(string luaScript, CancellationToken token = default);
}
