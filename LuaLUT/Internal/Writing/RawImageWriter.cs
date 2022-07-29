using System;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace LuaLUT.Internal.Writing
{
    internal class RawImageWriter : ImageWriterBase
    {
        private const int MaxDegreeOfParallelism = 8;


        public RawImageWriter(Stream stream) : base(stream) {}

        public override async Task ProcessAsync(string luaScript, int width, int height, CancellationToken token = default)
        {
            var processPixelRowBlock = new TransformBlock<int, Memory<Vector4>>(y => {
                var rowSpan = new Vector4[width];
                ProcessRow(luaScript, width, height, y, rowSpan.AsSpan());
                return rowSpan;
            }, new ExecutionDataflowBlockOptions {
                MaxDegreeOfParallelism = MaxDegreeOfParallelism,
            });

            var writeRowBlock = new ActionBlock<Memory<Vector4>>(row => {
                foreach (var pixel in row.Span)
                    WritePixel(pixel);
            });

            processPixelRowBlock.LinkTo(writeRowBlock, new DataflowLinkOptions {
                PropagateCompletion = true,
            });

            for (var y = 0; y < height; y++)
                processPixelRowBlock.Post(y);

            processPixelRowBlock.Complete();
            await writeRowBlock.Completion;
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

        private void WritePixel(in Vector4 pixel)
        {
            switch (PixelType) {
                case PixelType.BYTE: {
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            WriteByte_Norm(pixel.X);
                            break;
                        case PixelFormat.RG_NORM:
                            WriteByte_Norm(pixel.X);
                            WriteByte_Norm(pixel.Y);
                            break;
                        case PixelFormat.RGB_NORM:
                            WriteByte_Norm(pixel.X);
                            WriteByte_Norm(pixel.Y);
                            WriteByte_Norm(pixel.Z);
                            break;
                        case PixelFormat.RGBA_NORM:
                            WriteByte_Norm(pixel.X);
                            WriteByte_Norm(pixel.Y);
                            WriteByte_Norm(pixel.Z);
                            WriteByte_Norm(pixel.W);
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }

                    break;
                }
                case PixelType.SHORT: {
                    var buffer = new byte[sizeof(short)];
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            WriteShort_Norm(buffer, pixel.X);
                            break;
                        case PixelFormat.RG_NORM:
                            WriteShort_Norm(buffer, pixel.X);
                            WriteShort_Norm(buffer, pixel.Y);
                            break;
                        case PixelFormat.RGB_NORM:
                            WriteShort_Norm(buffer, pixel.X);
                            WriteShort_Norm(buffer, pixel.Y);
                            WriteShort_Norm(buffer, pixel.Z);
                            break;
                        case PixelFormat.RGBA_NORM:
                            WriteShort_Norm(buffer, pixel.X);
                            WriteShort_Norm(buffer, pixel.Y);
                            WriteShort_Norm(buffer, pixel.Z);
                            WriteShort_Norm(buffer, pixel.W);
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }

                    break;
                }
                case PixelType.INT: {
                    var buffer = new byte[sizeof(int)];
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            WriteInt_Norm(buffer, pixel.X);
                            break;
                        case PixelFormat.RG_NORM:
                            WriteInt_Norm(buffer, pixel.X);
                            WriteInt_Norm(buffer, pixel.Y);
                            break;
                        case PixelFormat.RGB_NORM:
                            WriteInt_Norm(buffer, pixel.X);
                            WriteInt_Norm(buffer, pixel.Y);
                            WriteInt_Norm(buffer, pixel.Z);
                            break;
                        case PixelFormat.RGBA_NORM:
                            WriteInt_Norm(buffer, pixel.X);
                            WriteInt_Norm(buffer, pixel.Y);
                            WriteInt_Norm(buffer, pixel.Z);
                            WriteInt_Norm(buffer, pixel.W);
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }

                    break;
                }
                case PixelType.UNSIGNED_BYTE: {
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            WriteUByte_Norm(pixel.X);
                            break;
                        case PixelFormat.RG_NORM:
                            WriteUByte_Norm(pixel.X);
                            WriteUByte_Norm(pixel.Y);
                            break;
                        case PixelFormat.RGB_NORM:
                            WriteUByte_Norm(pixel.X);
                            WriteUByte_Norm(pixel.Y);
                            WriteUByte_Norm(pixel.Z);
                            break;
                        case PixelFormat.RGBA_NORM:
                            WriteUByte_Norm(pixel.X);
                            WriteUByte_Norm(pixel.Y);
                            WriteUByte_Norm(pixel.Z);
                            WriteUByte_Norm(pixel.W);
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }

                    break;
                }
                case PixelType.UNSIGNED_SHORT: {
                    var buffer = new byte[sizeof(ushort)];
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            WriteUShort_Norm(buffer, pixel.X);
                            break;
                        case PixelFormat.RG_NORM:
                            WriteUShort_Norm(buffer, pixel.X);
                            WriteUShort_Norm(buffer, pixel.Y);
                            break;
                        case PixelFormat.RGB_NORM:
                            WriteUShort_Norm(buffer, pixel.X);
                            WriteUShort_Norm(buffer, pixel.Y);
                            WriteUShort_Norm(buffer, pixel.Z);
                            break;
                        case PixelFormat.RGBA_NORM:
                            WriteUShort_Norm(buffer, pixel.X);
                            WriteUShort_Norm(buffer, pixel.Y);
                            WriteUShort_Norm(buffer, pixel.Z);
                            WriteUShort_Norm(buffer, pixel.W);
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }

                    break;
                }
                case PixelType.UNSIGNED_INT: {
                    var buffer = new byte[sizeof(uint)];
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            WriteUInt_Norm(buffer, pixel.X);
                            break;
                        case PixelFormat.RG_NORM:
                            WriteUInt_Norm(buffer, pixel.X);
                            WriteUInt_Norm(buffer, pixel.Y);
                            break;
                        case PixelFormat.RGB_NORM:
                            WriteUInt_Norm(buffer, pixel.X);
                            WriteUInt_Norm(buffer, pixel.Y);
                            WriteUInt_Norm(buffer, pixel.Z);
                            break;
                        case PixelFormat.RGBA_NORM:
                            WriteUInt_Norm(buffer, pixel.X);
                            WriteUInt_Norm(buffer, pixel.Y);
                            WriteUInt_Norm(buffer, pixel.Z);
                            WriteUInt_Norm(buffer, pixel.W);
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }

                    break;
                }
                case PixelType.HALF_FLOAT: {
                    var buffer = new byte[2]; //[sizeof(Half)];
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            WriteHalfFloat(buffer, in pixel.X);
                            break;
                        case PixelFormat.RG_NORM:
                            WriteHalfFloat(buffer, in pixel.X);
                            WriteHalfFloat(buffer, in pixel.Y);
                            break;
                        case PixelFormat.RGB_NORM:
                            WriteHalfFloat(buffer, in pixel.X);
                            WriteHalfFloat(buffer, in pixel.Y);
                            WriteHalfFloat(buffer, in pixel.Z);
                            break;
                        case PixelFormat.RGBA_NORM:
                            WriteHalfFloat(buffer, in pixel.X);
                            WriteHalfFloat(buffer, in pixel.Y);
                            WriteHalfFloat(buffer, in pixel.Z);
                            WriteHalfFloat(buffer, in pixel.W);
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }

                    break;
                }
                case PixelType.FLOAT: {
                    var buffer = new byte[sizeof(float)];
                    switch (PixelFormat) {
                        case PixelFormat.R_NORM:
                            WriteFloat(buffer, in pixel.X);
                            break;
                        case PixelFormat.RG_NORM:
                            WriteFloat(buffer, in pixel.X);
                            WriteFloat(buffer, in pixel.Y);
                            break;
                        case PixelFormat.RGB_NORM:
                            WriteFloat(buffer, in pixel.X);
                            WriteFloat(buffer, in pixel.Y);
                            WriteFloat(buffer, in pixel.Z);
                            break;
                        case PixelFormat.RGBA_NORM:
                            WriteFloat(buffer, in pixel.X);
                            WriteFloat(buffer, in pixel.Y);
                            WriteFloat(buffer, in pixel.Z);
                            WriteFloat(buffer, in pixel.W);
                            break;
                        default:
                            throw new ApplicationException("Unsupported");
                    }

                    break;
                }
                default:
                    throw new ApplicationException("Unsupported");
            }
        }

        #region Raw Writers

        private void WriteByte(in sbyte value) =>
            throw new NotImplementedException();

        private void WriteUByte(in byte value) =>
            Stream.WriteByte(value);

        private void WriteShort(in Span<byte> buffer, in short value)
        {
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
            Stream.Write(buffer);
        }

        private void WriteUShort(in Span<byte> buffer, in ushort value)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
            Stream.Write(buffer);
        }

        private void WriteInt(in Span<byte> buffer, in int value)
        {
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
            Stream.Write(buffer);
        }

        private void WriteUInt(in Span<byte> buffer, in uint value)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
            Stream.Write(buffer);
        }
 
        private void WriteHalfFloat(in Span<byte> buffer, in float value)
        {
            throw new NotImplementedException();
        }
 
        private void WriteFloat(in Span<byte> buffer, in float value)
        {
            //if (float.IsNaN(value)) throw new ApplicationException();
            BinaryPrimitives.WriteSingleLittleEndian(buffer, float.IsNaN(value) ? 0f : value);
            Stream.Write(buffer);
        }
 
        private void WriteDouble(in Span<byte> buffer, in double value)
        {
            BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
            Stream.Write(buffer);
        }

        #endregion

        #region Normalized Writers

        private void WriteByte_Norm(in float normValue) =>
            WriteByte((sbyte)Math.Clamp(normValue * sbyte.MaxValue, sbyte.MinValue, sbyte.MaxValue));

        private void WriteUByte_Norm(in float normValue) =>
            WriteUByte((byte)Math.Clamp(normValue * byte.MaxValue, byte.MinValue, byte.MaxValue));

        private void WriteShort_Norm(in Span<byte> buffer, in float normValue) =>
            WriteShort(in buffer, (short)Math.Clamp(normValue * short.MaxValue, short.MinValue, short.MaxValue));

        private void WriteUShort_Norm(in Span<byte> buffer, in float normValue) =>
            WriteUShort(in buffer, (ushort)Math.Clamp(normValue * ushort.MaxValue, ushort.MinValue, ushort.MaxValue));

        private void WriteInt_Norm(in Span<byte> buffer, in float normValue) =>
            WriteInt(in buffer, (int)Math.Clamp(normValue * int.MaxValue, int.MinValue, int.MaxValue));

        private void WriteUInt_Norm(in Span<byte> buffer, in float normValue) =>
            WriteUInt(in buffer, (uint)Math.Clamp((double)normValue * uint.MaxValue, uint.MinValue, uint.MaxValue));

        private void WriteDouble_Norm(in Span<byte> buffer, in double normValue) =>
            WriteDouble(in buffer, Math.Clamp(normValue * double.MaxValue, double.MinValue, double.MaxValue));

        #endregion
    }
}
