using System;
using System.Buffers.Binary;
using System.IO;

namespace LuaLUT.Internal.PixelWriter;

internal class PixelReader
{
    private readonly byte[] buffer;
    private readonly Stream stream;


    public PixelReader(Stream stream)
    {
        this.stream = stream;
        buffer = new byte[4];
    }

    public void ReadByte(out sbyte value) =>
        throw new NotImplementedException();

    public void ReadUByte(out byte value)
    {
        FillBuffer(1);
        value = buffer[0];
    }

    public void ReadShort(out short value)
    {
        FillBuffer(2);
        value = BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    public void ReadUShort(out ushort value)
    {
        FillBuffer(2);
        value = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    public void ReadInt(out int value)
    {
        FillBuffer(4);
        value = BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    public void ReadUInt(out uint value)
    {
        FillBuffer(4);
        value = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
    }

    public void ReadHalf(out Half value)
    {
        FillBuffer(2);
        value = BinaryPrimitives.ReadHalfLittleEndian(buffer);
    }

    public void ReadFloat(out float value)
    {
        FillBuffer(4);
        value = BinaryPrimitives.ReadSingleLittleEndian(buffer);
    }

    private void FillBuffer(int length)
    {
        var i = 0;
        while (i < length) {
            var size = stream.Read(buffer, i, length - i);
            if (size == 0) throw new EndOfStreamException();
            i += size;
        }
    }
}
