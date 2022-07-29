using CommandLine;

namespace LuaLUT.Internal
{
    internal class MainOptions
    {
        [Option('s', "script", Required = true, HelpText = "The LUA script to use when generating LUT data.")]
        public string ScriptFilename { get; set; }

        [Option('o', "output", Required = true, HelpText = "The filename of the image to write the LUT content to.")]
        public string ImageFilename { get; set; }

        [Option('i', "image", Required = true, HelpText = "The format of the image to generate.")]
        public string ImageType { get; set; }

        [Option('f', "format", Required = true, HelpText = "The format of the pixels in the LUT image to generate.")]
        public string PixelFormat { get; set; }

        [Option('t', "type", Required = true, HelpText = "The type of the pixels in the LUT image to generate.")]
        public string PixelType { get; set; }

        [Option('w', "width", Required = true, HelpText = "The width of the LUT image to generate.")]
        public int ImageWidth { get; set; }

        [Option('h', "height", Required = true, HelpText = "The height of the LUT image to generate.")]
        public int ImageHeight { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option("var", Required = false, HelpText = "An optional list of variables.")]
        public string CustomVariables { get; set; }
    }
}
