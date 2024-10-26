using fairdao.extensions.shared.services;
using fairdao.extensions.shared.localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
using fairdao.extensions.shared;
using fairdao.extensions.shared.entity;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;

namespace fairdao.extensions.shared
{
    public class Configure
    {


        public static List<Assembly>? PageAssemblies;
        private static List<fairdao.extensions.shared.Extender>? extenders;



        private static void ConfigureExtenders(IServiceCollection services, fairdao.extensions.shared.Extender[]? extenders)
        {
            if (!(extenders?.Length > 0))
            {
                throw new Exception("请配置插件");
            }

            //系统主扩展
            if (!(extenders[0] is MainServiceExtender))
            {
                throw new Exception("第一个必须为主插件");
            }
            Configure.extenders = extenders.ToList();

            //加载插件配置
            Configure.extenders.ForEach(runners =>
              {
                  if (runners is ServiceExtender extender)
                  {
                      extender.Config(services);
                  }

              }
            );

            SysHelper.Extenders = Configure.extenders;

        }

        static void ProcessAss(Assembly ass)
        {
            string? name = ass.FullName;
            if (name?.StartsWith("fair") == true)
            {
                Console.WriteLine($"发现：============================  {name}");
                int index = name.IndexOf(',');
                string spaceName = name.Substring(0, index);
                if (spaceName == "fairdao.extensions.shared") return;
                string className = $"{spaceName}.Extender";
                string typeName = $"{className},{name}";
                Type type = ass.GetType(className);

                if (type != null)
                {
                    try
                    {
                        // 使用 Activator.CreateInstance 方法实例化类
                        object instance = Activator.CreateInstance(type);
                        // 判断实例化结果是否成功
                        if (instance is fairdao.extensions.shared.Extender myObject)
                        {
                            Configure.extenders.Add(myObject);
                        }
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e);
                    }
                }

            }
        }


        static Type? GetClassType(string? className)
        {
            if (className == null) return null;
            try
            {
                Type type = Type.GetType(className);

                if (type != null && !type.IsAbstract && !type.IsInterface)
                {
                    return type;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public static IServiceCollection ConfigureServices(IServiceCollection services, fairdao.extensions.shared.Extender[]? extenders) 
        {
            ConfigureExtenders(services, extenders);
            fairdao.extensions.shared.SysHelper.Services = services;
            services.AddTransient<fairdao.extensions.shared.IAPIHttpClient, fairdao.extensions.shared.APIHttpClient>();
            // 添加默认的账号服务
            var account = services.FirstOrDefault(m => m.ServiceType == typeof(IAccountService));
            if (account == null)
            {
                services.AddSingleton<IAccountService, AccountService>();
            }
            services.AddOptions();
            services.Configure<JsonSerializerOptions>(options =>
            {
                //如果属性值为空则不生成属性，减少传输量
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNamingPolicy = null;
                options.PropertyNameCaseInsensitive = true;
            });


            try
            {

                services.AddSingleton(typeof(fairdao.extensions.shared.services.TransService));
                #region 本地化
                services.AddLocalization(o => o.ResourcesPath = "resources");
                services.AddSingleton<INewWordRecorder, NewWordRecorder>();
                services.AddSingleton<fairdao.extensions.shared.localization.GensysLocaler>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return services;

            #endregion


        }


        public void Load(Extender extender)
        {

        }


        public static async Task ConfigureProviders(IServiceProvider provider)
        {
            await provider.UseHelper();
            var sysHelper = provider.GetRequiredService<SysHelper>();

            //加载存储的扩展
            var extdata = await sysHelper.GetCache<List<byte[]>>("extensions");
            //Assembly thisAss = typeof(Configure).Assembly;
            //string space = thisAss.FullName.Split(',')[0];
            //Stream? a= thisAss.GetManifestResourceStream($"{space}.fairSample.plug.dll");
            //byte[] buffer = new byte[a.Length];
            //a.Read(buffer,0,buffer.Length);
            //a.Close();
            //Assembly assembly2 = Assembly.Load(buffer);

            //ProcessAss(assembly2);

            if (extdata?.Count() > 0)
            {
                foreach (byte[] bts in extdata)
                {
                    Assembly assembly = Assembly.Load(bts);

                    ProcessAss(assembly);
                }
            }

            //处理可视组件
            extenders?.ForEach(extender =>
            {
                extender.Use(provider);
            });


            PageAssemblies = new List<Assembly>();
            foreach (var extender in Configure.extenders)
            {
                PageAssemblies.Add(extender.GetType().Assembly);
            }

            if (!(sysHelper.ClientConfig?.MTabs?.Count > 0))
            {
                await sysHelper.ReloadConfig();

            }


        }



    }
}
