using fairdao.extensions.shared;
using fairdao.extensions.shared.data;
using fairdao.extensions.shared.entity;
using fairdao.extensions.shared.services;
using fairdao.portal.h5;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FairExtensions
    {
        public static async Task RunFairHost( this WebAssemblyHostBuilder builder, params fairdao.extensions.shared.ExtenderBase[]? extenders)
        {

            builder.RootComponents.Add<fairdao.extensions.shared.Routes>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            SysHelper.AppStartTime = DateTime.Now;
            builder.Services.AddSingleton<FairdaoDatabase>();
            builder.Services.AddSingleton<IDataStore, LocaldbStore>();
            builder.Services.AddSingleton<Env>();
            builder.Services.AddSingleton<SysHelper, H5Helper>();
            builder.Services.AddFluentUIComponents();
            SysHelper.EntryAssembly = Assembly.GetExecutingAssembly();
            Configure.ConfigureServices(builder.Services, extenders);
            WebAssemblyHost host = builder.Build();
            await Configure.ConfigureProviders(host.Services);
            host?.RunAsync();
         
        }
    }
}
