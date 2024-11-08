using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace fairdao.h5.demo
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            return builder.RunFairHost(
                new fairdao.extensions.shared.Extender(),new fairdao.extensions.appCenter.Extender());
        }
    }
}
