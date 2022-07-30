using System;
using System.Buffers.Binary;
using Xunit;

namespace LuaLUT.Tests.Internal
{
    internal static class ByteAssert
    {
        public static void ByteEquals(in Span<byte> buffer, in int index, sbyte expectedValue)
        {
            throw new NotImplementedException();
        }

        public static void UByteEquals(in Span<byte> buffer, in int index, byte expectedValue)
        {
            Assert.Equal(expectedValue, buffer[index]);
        }

        public static void UShortEquals(in Span<byte> buffer, in int index, ushort expectedValue)
        {
            var actualValue = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(index, sizeof(ushort)));
            Assert.Equal(expectedValue, actualValue);
        }

        public static void UIntEquals(in Span<byte> buffer, in int index, uint expectedValue)
        {
            var actualValue = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(index, sizeof(uint)));
            Assert.Equal(expectedValue, actualValue);
        }
    }
}
