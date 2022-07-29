using CommandLine;
using LuaLUT.Internal;
using LuaLUT.Internal.Writing;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LuaLUT
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            try {
                var parser = Parser.Default.ParseArguments<MainOptions>(args);
                if (parser.Errors.Any()) return 1;

                await parser.WithParsedAsync(RunOptionsAsync);
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
            var timer = Stopwatch.StartNew();

            try {
                var luaScript = await File.ReadAllTextAsync(options.ScriptFilename);
                await using var outputStream = File.Open(options.ImageFilename, FileMode.Create, FileAccess.Write);
                
                var imageType = ImageTypes.Parse(options.ImageType);
                IImageWriter writer = imageType == ImageType.Raw
                    ? new RawImageWriter(outputStream)
                    : new StandardImageWriter(outputStream, imageType);

                writer.PixelFormat = PixelFormats.Parse(options.PixelFormat);
                writer.PixelType = PixelTypes.Parse(options.PixelType);

                if (options.CustomVariables.Any()) {
                    foreach (var part in options.CustomVariables) {
                        var i = part.IndexOf('=');
                        if (i < 0) throw new ApplicationException($"Failed to parse variable '{part}'!");

                        var varName = part[..i];
                        var varValue = part[(i+1)..];
                        writer.CustomVariables[varName] = varValue;

                        if (options.Verbose) Console.WriteLine($"Adding custom variable '{varName}' with value '{varValue}'.");
                    }
                }

                await writer.ProcessAsync(luaScript, options.ImageWidth, options.ImageHeight);
                timer.Stop();

                Console.WriteLine($"LUT generated successfully! Duration: {timer.Elapsed:g}");
            }
            catch (Exception error) {
                Console.WriteLine($"Failed to build LUT! {error.Message}\n{error}");
                timer.Stop();
            }
        }
    }
}
