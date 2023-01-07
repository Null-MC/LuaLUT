using CommandLine;
using LuaLUT.Internal;
using LuaLUT.Internal.ImageWriter;
using LuaLUT.Internal.PixelWriter;
using LuaLUT.Internal.Samplers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuaLUT;

internal class Program
{
    private static readonly CancellationTokenSource tokenSource;


    static Program()
    {
        tokenSource = new CancellationTokenSource();
    }

    private static async Task<int> Main(string[] args)
    {
        Console.CancelKeyPress += Console_OnCancelKeyPress;

        try {
            var parser = Parser.Default.ParseArguments<MainOptions>(args);
            if (parser.Errors.Any()) return 1;

            await parser.WithParsedAsync(RunOptionsAsync);
            return 0;
        }
        catch (OperationCanceledException) {
            Console.WriteLine("Operation cancelled.");
            return 1;
        }
        catch (Exception error) {
            Console.WriteLine($"An unhandled exception has occurred! {error.Message}\n{error}");
            return -1;
        }
        finally {
            tokenSource.Dispose();
        }
    }

    private static async Task RunOptionsAsync(MainOptions options)
    {
        if (options.Verbose) Console.WriteLine("Verbose output enabled.");
        var token = tokenSource.Token;
        var timer = Stopwatch.StartNew();

        try {
            var imageType = ImageTypes.Parse(options.ImageType);

            var outputFile = options.Output.FullName;
            if (outputFile.EndsWith('\\') || outputFile.EndsWith('/')) {
                var ext = GetDefaultExtension(imageType);
                outputFile += Path.GetFileNameWithoutExtension(options.Script.Name)+ext;
            }

            var luaScript = await File.ReadAllTextAsync(options.Script.FullName, token);

            // Create directory for output file if it doesn't exist
            var outputPath = Path.GetDirectoryName(outputFile);
            if (outputPath != null && !Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            await using var outputStream = File.Open(outputFile, FileMode.Create, FileAccess.Write);
                
            var writer = GetImageWriter(outputStream, options);
            writer.ImageWidth = options.ImageWidth;
            writer.ImageHeight = options.ImageHeight ?? 1;
            writer.ImageDepth = options.ImageDepth ?? 1;

            writer.ImageDimensions = 1;
            if (options.ImageHeight.HasValue)
                writer.ImageDimensions = options.ImageDepth.HasValue ? 3 : 2;

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

            if (options.IncludeFiles.Any()) {
                foreach (var file in options.IncludeFiles) {
                    var fileName = Path.GetFullPath(file);

                    if (!File.Exists(fileName)) {
                        var scriptPath = options.Script.DirectoryName;
                        fileName = Path.Join(scriptPath, file);
                    }

                    if (!File.Exists(fileName))
                        throw new ApplicationException($"Failed to locate included script '{file}' in either current nor script directories!");

                    writer.IncludedFiles.Add(fileName);
                }
            }

            if (options.Samplers.Any()) {
                foreach (var samplerSpec in options.Samplers) {
                    var description = new SamplerDescription();
                    description.Parse(samplerSpec);

                    var sampler = SamplerFactory.Get(description);

                    var name = Path.GetFileNameWithoutExtension(description.Filename) ?? description.Filename;
                    if (name == null) throw new ApplicationException("Sampler name is undefined!");

                    await sampler.LoadImageAsync(token);

                    writer.Samplers[name] = sampler;
                }
            }

            await writer.ProcessAsync(luaScript, token);
            timer.Stop();

            Console.WriteLine($"LUT generated successfully! Duration: {timer.Elapsed:g}");
        }
        catch (Exception error) {
            Console.WriteLine($"Failed to build LUT! {error.Message}\n{error}");
            timer.Stop();
        }
    }

    private static IImageWriter GetImageWriter(Stream outputStream, MainOptions options)
    {
        var imageType = ImageTypes.Parse(options.ImageType);
        var pixelFormat = PixelFormats.Parse(options.PixelFormat);
        var pixelType = PixelTypes.Parse(options.PixelType);

        if (imageType != ImageType.Raw)
            return new StandardImageWriter(outputStream, imageType) {
                DepthSlice = options.DepthSlice ?? 0,
                PixelFormat = pixelFormat,
                PixelType = pixelType,
            };

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

    private static string GetDefaultExtension(ImageType imageType)
    {
        return imageType switch {
            ImageType.Bmp => ".bmp",
            ImageType.Png => ".png",
            ImageType.Raw => ".dat",
            _ => throw new ApplicationException($"Unsupported image type '{imageType}'!")
        };
    }

    private static void Console_OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        //Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Cancelling...");
        //Console.ResetColor();

        tokenSource.Cancel();
    }
}