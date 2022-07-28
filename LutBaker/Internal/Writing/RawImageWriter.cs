using System.IO;

namespace LutBaker.Internal.Writing
{
    internal class RawImageWriter : ImageWriterBase
    {
        public RawImageWriter(Stream stream) : base(stream) {}

        public override void AppendPixel(int x, int y, double[] pixel)
        {
            // TODO
        }
    }
}
