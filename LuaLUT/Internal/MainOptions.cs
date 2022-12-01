using CommandLine;
using System.Collections.Generic;
using System.IO;

namespace LuaLUT.Internal;

internal class MainOptions
{
    [Option("script", Required = true, HelpText = "The LUA script to use when generating LUT data.")]
    public FileInfo Script { get; set; }

    [Option("out", Required = true, HelpText = "The filename of the image to write the LUT content to.")]
    public FileInfo Output { get; set; }

    [Option("img", Required = true, HelpText = "The format of the image to generate.")]
    public string ImageType { get; set; }

    [Option("format", Required = true, HelpText = "The format of the pixels in the LUT image to generate.")]
    public string PixelFormat { get; set; }

    [Option("type", Required = true, HelpText = "The type of the pixels in the LUT image to generate.")]
    public string PixelType { get; set; }

    [Option('w', "width", Required = true, HelpText = "The width [x] of the LUT image to generate.")]
    public int ImageWidth { get; set; }

    [Option('h', "height", Required = false, HelpText = "The height [y] of the LUT image to generate.")]
    public int? ImageHeight { get; set; }

    [Option('d', "depth", Required = false, HelpText = "The depth [z] of the LUT image to generate.")]
    public int? ImageDepth { get; set; }

    [Option('s', "slice", Required = false, HelpText = "The depth slice [z] of the LUT image to generate.")]
    public int? DepthSlice { get; set; }

    [Option('v', "var", Required = false, Separator = '|', HelpText = "An optional list of variables.")]
    public IEnumerable<string> CustomVariables { get; set; }

    [Option('i', "include", Required = false, Separator = '|', HelpText = "An optional list of lua scripts to include.")]
    public IEnumerable<string> IncludeFiles { get; set; }

    [Option("verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }
}