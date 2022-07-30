using System;
using System.IO;

namespace LuaLUT.Internal.PixelWriter;

internal class PixelWriterNorm : PixelWriterBase<double>
{
    private delegate void WritePixelNormAction(in double value);


    public PixelWriterNorm(Stream stream) : base(stream) { }

    public override void Write(in double[] pixel)
    {
        switch (PixelType) {
            case PixelType.BYTE:
                WritePixel_Norm(WriteByte_Norm, in pixel);
                break;
            case PixelType.SHORT:
                WritePixel_Norm(WriteShort_Norm, pixel);
                break;
            case PixelType.INT:
                WritePixel_Norm(WriteInt_Norm, pixel);
                break;
            case PixelType.UNSIGNED_BYTE:
                WritePixel_Norm(WriteUByte_Norm, pixel);
                break;
            case PixelType.UNSIGNED_SHORT:
                WritePixel_Norm(WriteUShort_Norm, pixel);
                break;
            case PixelType.UNSIGNED_INT:
                WritePixel_Norm(WriteUInt_Norm, pixel);
                break;
            case PixelType.HALF_FLOAT:
                WritePixel_Norm(WriteHalf_Norm, pixel);
                break;
            case PixelType.FLOAT:
                WritePixel_Norm(WriteFloat_Norm, pixel);
                break;
            default:
                throw new ApplicationException("Unsupported");
        }
    }

    private void WritePixel_Norm(WritePixelNormAction writeAction, in double[] pixel)
    {
        switch (PixelFormat) {
            case PixelFormat.R_NORM:
                writeAction(pixel[0]);
                break;
            case PixelFormat.RG_NORM:
                writeAction(pixel[0]);
                writeAction(pixel[1]);
                break;
            case PixelFormat.RGB_NORM:
                writeAction(pixel[0]);
                writeAction(pixel[1]);
                writeAction(pixel[2]);
                break;
            case PixelFormat.RGBA_NORM:
                writeAction(pixel[0]);
                writeAction(pixel[1]);
                writeAction(pixel[2]);
                writeAction(pixel[3]);
                break;
            case PixelFormat.BGR_NORM:
                writeAction(pixel[2]);
                writeAction(pixel[1]);
                writeAction(pixel[0]);
                break;
            case PixelFormat.BGRA_NORM:
                writeAction(pixel[2]);
                writeAction(pixel[1]);
                writeAction(pixel[0]);
                writeAction(pixel[3]);
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
        WriteHalf((Half)Math.Clamp(normValue * (double)Half.MaxValue, (double)Half.MinValue, (double)Half.MaxValue));

    private void WriteFloat_Norm(in double normValue) =>
        WriteFloat((float)Math.Clamp(normValue * float.MaxValue, float.MinValue, float.MaxValue));
}