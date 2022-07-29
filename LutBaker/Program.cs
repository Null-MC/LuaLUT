using CommandLine;
using LutBaker.Internal;
using LutBaker.Internal.Writing;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("LutBaker.Tests")]

namespace LutBaker
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            try {
                var parser = Parser.Default.ParseArguments<MainOptions>(args);
                if (parser.Errors.Any()) return 1;

                await parser.WithParsedAsync(RunOptionsAsync);
                Console.WriteLine("LUT generated successfully.");
                return 0;
            }
            catch (Exception error) {
                Console.WriteLine($"An unhandled exception has occurred! {error.Message}\n{error}");
                return -1;
            }
        }

        private static async Task RunOptionsAsync(MainOptions options)
        {
            if (options.Verbose) Console.WriteLine("Verbose output enabled.");

            try {
                using var processor = new LuaScriptProcessor {
                    Script = await File.ReadAllTextAsync(options.ScriptFilename),
                    Width = options.ImageWidth,
                    Height = options.ImageHeight,
                };

                // TODO: populate custom variables
                // processor.CustomVariables = ;

                await using var outputStream = File.Open(options.ImageFilename, FileMode.Create, FileAccess.Write);
                
                var imageType = ImageTypes.Parse(options.ImageType);
                IImageWriter writer = imageType == ImageType.Raw
                    ? new RawImageWriter(outputStream)
                    : new StandardImageWriter(outputStream, imageType);
                
                writer.PixelFormat = PixelFormats.Parse(options.PixelFormat);
                writer.PixelType = PixelTypes.Parse(options.PixelType);

                await processor.InitializeAsync();

                await writer.ProcessAsync(processor);
            }
            catch (Exception error) {
                Console.WriteLine($"Failed to build LUT! {error.Message}\n{error}");
            }
        }
    }
}
