using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuaLUT.Internal.Writing
{
    internal class RawImageWriter : ImageWriterBase
    {
        public RawImageWriter(Stream stream) : base(stream) {}

        public override async Task ProcessAsync(string luaScript, int width, int height, CancellationToken token = default)
        {
            await using var binaryWriter = new BinaryWriter(Stream, Encoding.UTF8, true);

            // TODO: make row processing parallel, but keep writing synchronous
            for (var y = 0; y < height; y++) {
                var rowSpan = new Vector4[width];
                ProcessRow(luaScript, width, height, y, rowSpan.AsSpan());

                foreach (var pixel in rowSpan)
                    WritePixel(binaryWriter, pixel);
            }
        }

        private void ProcessRow(string luaScript, int width, int height, int y, Span<Vector4> rowSpan)
        {
            using var processor = new LuaScriptProcessor {
                CustomVariables = CustomVariables,
                Script = luaScript,
                Width = width,
                Height = height,
            };

            processor.Initialize();

            for (var x = 0; x < width; x++) {
                var pixel = processor.ProcessPixel(x, y);

                if (pixel.Length >= 1) rowSpan[x].X = (float)pixel[0];
                if (pixel.Length >= 2) rowSpan[x].Y = (float)pixel[1];
                if (pixel.Length >= 3) rowSpan[x].Z = (float)pixel[2];
                if (pixel.Length >= 4) rowSpan[x].W = (float)pixel[3];
            }
        }

        private void WritePixel(BinaryWriter binaryWriter, in Vector4 pixel)
        {
            switch (PixelType) {
                case PixelType.BYTE:
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            binaryWriter.Write((sbyte)(pixel.X * sbyte.MaxValue));
                            break;
                        case PixelFormat.RG_NORM:
                            binaryWriter.Write((sbyte)(pixel.X * sbyte.MaxValue));
                            binaryWriter.Write((sbyte)(pixel.Y * sbyte.MaxValue));
                            break;
                        case PixelFormat.RGB_NORM:
                            binaryWriter.Write((sbyte)(pixel.X * sbyte.MaxValue));
                            binaryWriter.Write((sbyte)(pixel.Y * sbyte.MaxValue));
                            binaryWriter.Write((sbyte)(pixel.Z * sbyte.MaxValue));
                            break;
                        case PixelFormat.RGBA_NORM:
                            binaryWriter.Write((sbyte)(pixel.X * sbyte.MaxValue));
                            binaryWriter.Write((sbyte)(pixel.Y * sbyte.MaxValue));
                            binaryWriter.Write((sbyte)(pixel.Z * sbyte.MaxValue));
                            binaryWriter.Write((sbyte)(pixel.W * sbyte.MaxValue));
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }
                    break;
                case PixelType.SHORT:
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            binaryWriter.Write((short)(pixel.X * short.MaxValue));
                            break;
                        case PixelFormat.RG_NORM:
                            binaryWriter.Write((short)(pixel.X * short.MaxValue));
                            binaryWriter.Write((short)(pixel.Y * short.MaxValue));
                            break;
                        case PixelFormat.RGB_NORM:
                            binaryWriter.Write((short)(pixel.X * short.MaxValue));
                            binaryWriter.Write((short)(pixel.Y * short.MaxValue));
                            binaryWriter.Write((short)(pixel.Z * short.MaxValue));
                            break;
                        case PixelFormat.RGBA_NORM:
                            binaryWriter.Write((short)(pixel.X * short.MaxValue));
                            binaryWriter.Write((short)(pixel.Y * short.MaxValue));
                            binaryWriter.Write((short)(pixel.Z * short.MaxValue));
                            binaryWriter.Write((short)(pixel.W * short.MaxValue));
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }
                    break;
                case PixelType.INT:
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            binaryWriter.Write((int)(pixel.X * int.MaxValue));
                            break;
                        case PixelFormat.RG_NORM:
                            binaryWriter.Write((int)(pixel.X * int.MaxValue));
                            binaryWriter.Write((int)(pixel.Y * int.MaxValue));
                            break;
                        case PixelFormat.RGB_NORM:
                            binaryWriter.Write((int)(pixel.X * int.MaxValue));
                            binaryWriter.Write((int)(pixel.Y * int.MaxValue));
                            binaryWriter.Write((int)(pixel.Z * int.MaxValue));
                            break;
                        case PixelFormat.RGBA_NORM:
                            binaryWriter.Write((int)(pixel.X * int.MaxValue));
                            binaryWriter.Write((int)(pixel.Y * int.MaxValue));
                            binaryWriter.Write((int)(pixel.Z * int.MaxValue));
                            binaryWriter.Write((int)(pixel.W * int.MaxValue));
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }
                    break;
                case PixelType.UNSIGNED_BYTE:
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            binaryWriter.Write((byte)(pixel.X * byte.MaxValue));
                            break;
                        case PixelFormat.RG_NORM:
                            binaryWriter.Write((byte)(pixel.X * byte.MaxValue));
                            binaryWriter.Write((byte)(pixel.Y * byte.MaxValue));
                            break;
                        case PixelFormat.RGB_NORM:
                            binaryWriter.Write((byte)(pixel.X * byte.MaxValue));
                            binaryWriter.Write((byte)(pixel.Y * byte.MaxValue));
                            binaryWriter.Write((byte)(pixel.Z * byte.MaxValue));
                            break;
                        case PixelFormat.RGBA_NORM:
                            binaryWriter.Write((byte)(pixel.X * byte.MaxValue));
                            binaryWriter.Write((byte)(pixel.Y * byte.MaxValue));
                            binaryWriter.Write((byte)(pixel.Z * byte.MaxValue));
                            binaryWriter.Write((byte)(pixel.W * byte.MaxValue));
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }
                    break;
                case PixelType.UNSIGNED_SHORT:
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            binaryWriter.Write((ushort)(pixel.X * ushort.MaxValue));
                            break;
                        case PixelFormat.RG_NORM:
                            binaryWriter.Write((ushort)(pixel.X * ushort.MaxValue));
                            binaryWriter.Write((ushort)(pixel.Y * ushort.MaxValue));
                            break;
                        case PixelFormat.RGB_NORM:
                            binaryWriter.Write((ushort)(pixel.X * ushort.MaxValue));
                            binaryWriter.Write((ushort)(pixel.Y * ushort.MaxValue));
                            binaryWriter.Write((ushort)(pixel.Z * ushort.MaxValue));
                            break;
                        case PixelFormat.RGBA_NORM:
                            binaryWriter.Write((ushort)(pixel.X * ushort.MaxValue));
                            binaryWriter.Write((ushort)(pixel.Y * ushort.MaxValue));
                            binaryWriter.Write((ushort)(pixel.Z * ushort.MaxValue));
                            binaryWriter.Write((ushort)(pixel.W * ushort.MaxValue));
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }
                    break;
                case PixelType.UNSIGNED_INT:
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            binaryWriter.Write((uint)(pixel.X * uint.MaxValue));
                            break;
                        case PixelFormat.RG_NORM:
                            binaryWriter.Write((uint)(pixel.X * uint.MaxValue));
                            binaryWriter.Write((uint)(pixel.Y * uint.MaxValue));
                            break;
                        case PixelFormat.RGB_NORM:
                            binaryWriter.Write((uint)(pixel.X * uint.MaxValue));
                            binaryWriter.Write((uint)(pixel.Y * uint.MaxValue));
                            binaryWriter.Write((uint)(pixel.Z * uint.MaxValue));
                            break;
                        case PixelFormat.RGBA_NORM:
                            binaryWriter.Write((uint)(pixel.X * uint.MaxValue));
                            binaryWriter.Write((uint)(pixel.Y * uint.MaxValue));
                            binaryWriter.Write((uint)(pixel.Z * uint.MaxValue));
                            binaryWriter.Write((uint)(pixel.W * uint.MaxValue));
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }
                    break;
                //case PixelType.HALF_FLOAT:
                //    switch (PixelFormat) {
                //        case PixelFormat.R_NORM:
                //            var bytes = Half.GetBytes((Half)pixel[0]);
                //            binaryWriter.Write();
                //            break;
                //        case PixelFormat.RG_NORM:
                //            binaryWriter.Write((float)pixel[0]);
                //            binaryWriter.Write((float)pixel[1]);
                //            break;
                //        case PixelFormat.RGB_NORM:
                //            binaryWriter.Write((float)pixel[0]);
                //            binaryWriter.Write((float)pixel[1]);
                //            binaryWriter.Write((float)pixel[2]);
                //            break;
                //        case PixelFormat.RGBA_NORM:
                //            binaryWriter.Write((float)pixel[0]);
                //            binaryWriter.Write((float)pixel[1]);
                //            binaryWriter.Write((float)pixel[2]);
                //            binaryWriter.Write((float)pixel[3]);
                //            break;
                //        default:
                //            throw new ApplicationException("Unsupported");
                //    }
                //    break;
                case PixelType.FLOAT:
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            binaryWriter.Write(pixel.X);
                            break;
                        case PixelFormat.RG_NORM:
                            binaryWriter.Write(pixel.X);
                            binaryWriter.Write(pixel.Y);
                            break;
                        case PixelFormat.RGB_NORM:
                            binaryWriter.Write(pixel.X);
                            binaryWriter.Write(pixel.Y);
                            binaryWriter.Write(pixel.Z);
                            break;
                        case PixelFormat.RGBA_NORM:
                            binaryWriter.Write(pixel.X);
                            binaryWriter.Write(pixel.Y);
                            binaryWriter.Write(pixel.Z);
                            binaryWriter.Write(pixel.W);
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }
                    break;
                default:
                    throw new ApplicationException("Unsupported");
            }
        }
    }
}
