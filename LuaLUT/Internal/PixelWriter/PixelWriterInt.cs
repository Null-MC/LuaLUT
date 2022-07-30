using System;
using System.IO;

namespace LuaLUT.Internal.PixelWriter;

internal class PixelWriterInt : PixelWriterBase<long>
{
    public PixelWriterInt(Stream stream) : base(stream) {}

    public override void Write(in long[] pixel)
    {
        switch (PixelType) {
            case PixelType.BYTE:
                WritePixelChannels(WriteByte_Int, in pixel);
                break;
            case PixelType.SHORT:
                WritePixelChannels(WriteShort_Int, pixel);
                break;
            case PixelType.INT:
                WritePixelChannels(WriteInt_Int, pixel);
                break;
            case PixelType.UNSIGNED_BYTE:
                WritePixelChannels(WriteUByte_Int, pixel);
                break;
            case PixelType.UNSIGNED_SHORT:
                WritePixelChannels(WriteUShort_Int, pixel);
                break;
            case PixelType.UNSIGNED_INT:
                WritePixelChannels(WriteUInt_Int, pixel);
                break;
            //case PixelType.HALF_FLOAT:
            //    WritePixel_Int(WriteHalf_Int, pixel);
            //    break;
            //case PixelType.FLOAT:
            //    WritePixel_Int(WriteFloat_Int, pixel);
            //    break;
            default:
                throw new ApplicationException("Unsupported");
        }
    }

    private void WriteByte_Int(in long intValue) =>
        WriteByte((sbyte)Math.Clamp(intValue, sbyte.MinValue, sbyte.MaxValue));

    private void WriteUByte_Int(in long intValue) =>
        WriteUByte((byte)Math.Clamp(intValue, byte.MinValue, byte.MaxValue));

    private void WriteShort_Int(in long intValue) =>
        WriteShort((short)Math.Clamp(intValue, short.MinValue, short.MaxValue));

    private void WriteUShort_Int(in long intValue) =>
        WriteUShort((ushort)Math.Clamp(intValue, ushort.MinValue, ushort.MaxValue));

    private void WriteInt_Int(in long intValue) =>
        WriteInt((int)Math.Clamp(intValue, int.MinValue, int.MaxValue));

    private void WriteUInt_Int(in long intValue) =>
        WriteUInt((uint)Math.Clamp(intValue, uint.MinValue, uint.MaxValue));

    //private void WriteHalf_Int(in long intValue) =>
    //    WriteHalf((Half)Math.Clamp(intValue, (double)Half.MinValue, (double)Half.MaxValue));

    //private void WriteFloat_Int(in long intValue) =>
    //    WriteFloat((float)Math.Clamp(intValue, float.MinValue, float.MaxValue));
}