using System;
using System.Buffers.Binary;
using System.IO;

namespace LuaLUT.Internal.PixelWriter;

internal abstract class PixelWriterBase<T>
{
    protected delegate void WritePixelAction(in T value);

    private readonly byte[] buffer;
    public readonly Stream Stream;

    public PixelFormat PixelFormat { get; set; }
    public PixelType PixelType { get; set; }


    protected PixelWriterBase(Stream stream)
    {
        Stream = stream;
        buffer = new byte[4];
    }

    public abstract void Write(in T[] pixel);

    protected void WritePixelChannels(WritePixelAction writeAction, in T[] pixel)
    {
        switch (PixelFormat) {
            case PixelFormat.R_NORM:
            case PixelFormat.R_INT:
                writeAction(pixel[0]);
                break;
            case PixelFormat.RG_NORM:
            case PixelFormat.RG_INT:
                writeAction(pixel[0]);
                writeAction(pixel[1]);
                break;
            case PixelFormat.RGB_NORM:
            case PixelFormat.RGB_INT:
                writeAction(pixel[0]);
                writeAction(pixel[1]);
                writeAction(pixel[2]);
                break;
            case PixelFormat.RGBA_NORM:
            case PixelFormat.RGBA_INT:
                writeAction(pixel[0]);
                writeAction(pixel[1]);
                writeAction(pixel[2]);
                writeAction(pixel[3]);
                break;
            case PixelFormat.BGR_NORM:
            case PixelFormat.BGR_INT:
                writeAction(pixel[2]);
                writeAction(pixel[1]);
                writeAction(pixel[0]);
                break;
            case PixelFormat.BGRA_NORM:
            case PixelFormat.BGRA_INT:
                writeAction(pixel[2]);
                writeAction(pixel[1]);
                writeAction(pixel[0]);
                writeAction(pixel[3]);
                break;
            default:
                throw new ApplicationException("Unsupported");
        }
    }

    protected void WriteByte(in sbyte value) =>
        throw new NotImplementedException();

    protected void WriteUByte(in byte value) =>
        Stream.WriteByte(value);

    protected void WriteShort(in short value)
    {
        BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
        Stream.Write(buffer);
    }

    protected void WriteUShort(in ushort value)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        Stream.Write(buffer);
    }

    protected void WriteInt(in int value)
    {
        BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        Stream.Write(buffer);
    }

    protected void WriteUInt(in uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        Stream.Write(buffer);
    }

    protected void WriteHalf(in Half value)
    {
        BinaryPrimitives.WriteHalfLittleEndian(buffer, Half.IsNaN(value) ? (Half)0 : value);
        Stream.Write(buffer);
    }

    protected void WriteFloat(in float value)
    {
        BinaryPrimitives.WriteSingleLittleEndian(buffer, float.IsNaN(value) ? 0f : value);
        Stream.Write(buffer);
    }
}