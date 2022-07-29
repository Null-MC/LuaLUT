using LuaLUT.Internal;
using System.Reflection;
using System.Threading.Tasks;

namespace LuaLUT.Tests.Internal
{
    public abstract class TestBase
    {
        protected static Task<string> LoadScriptAsync(string localPath)
        {
            return Assembly.GetExecutingAssembly()
                .ReadTextAsync($"LuaLUT.Tests.TestScripts.{localPath}");
        }
    }
}
