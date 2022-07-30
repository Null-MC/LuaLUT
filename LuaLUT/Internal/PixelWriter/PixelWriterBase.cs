using System;
using System.Buffers.Binary;
using System.IO;

namespace LuaLUT.Internal.PixelWriter;

internal abstract class PixelWriterBase<T>
{
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