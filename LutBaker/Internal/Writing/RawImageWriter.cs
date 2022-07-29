using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LutBaker.Internal.Writing
{
    internal class RawImageWriter : ImageWriterBase
    {
        public RawImageWriter(Stream stream) : base(stream) {}

        public override async Task ProcessAsync(LuaScriptProcessor processor, CancellationToken token = default)
        {
            // TODO
        }
    }
}
