using System;
using System.IO;

namespace LuaLUT.Internal.PixelWriter;

internal class PixelWriterNorm : PixelWriterBase<double>
{
    public PixelWriterNorm(Stream stream) : base(stream) {}

    public override void Write(in double[] pixel)
    {
        switch (PixelType) {
            case PixelType.BYTE:
                WritePixelChannels(WriteByte_Norm, in pixel);
                break;
            case PixelType.SHORT:
                WritePixelChannels(WriteShort_Norm, pixel);
                break;
            case PixelType.INT:
                WritePixelChannels(WriteInt_Norm, pixel);
                break;
            case PixelType.UNSIGNED_BYTE:
                WritePixelChannels(WriteUByte_Norm, pixel);
                break;
            case PixelType.UNSIGNED_SHORT:
                WritePixelChannels(WriteUShort_Norm, pixel);
                break;
            case PixelType.UNSIGNED_INT:
                WritePixelChannels(WriteUInt_Norm, pixel);
                break;
            case PixelType.HALF_FLOAT:
                WritePixelChannels(WriteHalf_Norm, pixel);
                break;
            case PixelType.FLOAT:
                WritePixelChannels(WriteFloat_Norm, pixel);
                break;
            default:
                throw new ApplicationException("Unsupported");
        }
    }

    private void WriteByte_Norm(in double normValue) =>
        WriteByte((sbyte)Math.Clamp(normValue * sbyte.MaxValue, sbyte.MinValue, sbyte.MaxValue));

    private void WriteUByte_Norm(in double normValue) =>
        WriteUByte((byte)Math.Clamp(normValue * byte.MaxValue, byte.MinValue, byte.MaxValue));

    private void WriteShort_Norm(in double normValue) =>
        WriteShort((short)Math.Clamp(normValue * short.MaxValue, short.MinValue, short.MaxValue));

    private void WriteUShort_Norm(in double normValue) =>
        WriteUShort((ushort)Math.Clamp(normValue * ushort.MaxValue, ushort.MinValue, ushort.MaxValue));

    private void WriteInt_Norm(in double normValue) =>
        WriteInt((int)Math.Clamp(normValue * int.MaxValue, int.MinValue, int.MaxValue));

    private void WriteUInt_Norm(in double normValue) =>
        WriteUInt((uint)Math.Clamp(normValue * uint.MaxValue, uint.MinValue, uint.MaxValue));

    private void WriteHalf_Norm(in double normValue) =>
        WriteHalf((Half)Math.Clamp(normValue, (double)Half.MinValue, (double)Half.MaxValue));

    private void WriteFloat_Norm(in double normValue) =>
        WriteFloat((float)Math.Clamp(normValue, float.MinValue, float.MaxValue));
}