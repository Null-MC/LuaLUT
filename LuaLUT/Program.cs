using CommandLine;
using LuaLUT.Internal;
using LuaLUT.Internal.ImageWriter;
using LuaLUT.Internal.PixelWriter;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LuaLUT;

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
            var pixelFormat = PixelFormats.Parse(options.PixelFormat);
            var pixelType = PixelTypes.Parse(options.PixelType);

            var writer = GetImageWriter(outputStream, imageType, pixelFormat, pixelType);

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

    private static IImageWriter GetImageWriter(Stream outputStream, ImageType imageType, PixelFormat pixelFormat, PixelType pixelType)
    {
        if (imageType != ImageType.Raw)
            return new StandardImageWriter(outputStream, imageType);

        if (PixelFormats.IsNormalized(pixelFormat)) {
            var pixelWriterNorm = new PixelWriterNorm(outputStream) {
                PixelFormat = pixelFormat,
                PixelType = pixelType,
            };

            return new RawImageWriter<double>(pixelWriterNorm) {
                PixelFormat = pixelFormat,
                PixelType = pixelType,
            };
        }

        var pixelWriterInt = new PixelWriterInt(outputStream) {
            PixelFormat = pixelFormat,
            PixelType = pixelType,
        };

        return new RawImageWriter<long>(pixelWriterInt) {
            PixelFormat = pixelFormat,
            PixelType = pixelType,
        };
    }
}