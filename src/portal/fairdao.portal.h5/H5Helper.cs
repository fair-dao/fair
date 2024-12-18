using fairdao.extensions.shared;
using fairdao.extensions.shared.entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace fairdao.portal.h5
{
    public class H5Helper : fairdao.extensions.shared.SysHelper
    {

      
        public H5Helper(IDataStore dataStore, Env env) : base(dataStore, env)
        {
            
            this.ClientEnv = env;
        
        }

        public override async Task Init()
        {
            Console.Write("init.........................");
            var jsRuntime = Services.BuildServiceProvider().GetService<IJSRuntime>();

            #region 用脚本获取当前环境配置
            Env? jsEnv = null;

            try
            {

                jsEnv = await jsRuntime.InvokeAsync<Env>("ClientEnv");
                //logBuilder.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(env));
                if (jsEnv != null)
                {
                    this.ClientEnv.DeviceId = jsEnv.DeviceId;
                    this.ClientEnv.DeviceHeight = jsEnv.DeviceHeight;
                    this.ClientEnv.DeviceWidth = jsEnv.DeviceWidth;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

         
            #endregion
            try
            {
                #region 设置客服端配置
                this.ClientConfig = await dataStore.GetConfig<fairdao.extensions.shared.entity.ClientConfig>("clientConfig");
                if (this.ClientConfig == null && jsRuntime != null) //如果数据库中不存在配置，从JS中读取配置
                {
                    this.ClientConfig = await jsRuntime.InvokeAsync<fairdao.extensions.shared.entity.ClientConfig>("window.ClientConfig");

               
                    if (this.ClientConfig == null)
                    {
                        throw new Exception("客户端未配置");
                    }

                    Console.WriteLine($"{this.ClientConfig?.AppName},{ApiUrl}");
                    //写入默认配置
                    SetClientConfig(this.ClientConfig);
                }
   

                //  this.ClientConfig = clientConfig;



                #endregion

            }
            catch (Exception ex)
            {

            }



            try
            {
                await LoadLocalData();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Init失败:{e.Message}");
                Console.WriteLine(e.StackTrace ?? "");
            }

        }


        public override Task PlayAudio(string audio)
        {
            var jsRuntime = Services.BuildServiceProvider().GetService<IJSRuntime>();
         jsRuntime?.InvokeVoidAsync("playMsgAudio", audio);
            return Task.CompletedTask;
        
        }



    }
}
