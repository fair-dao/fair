
using fairdao.extensions.shared;
using clientApp.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fairdao.extensions.shared.entity;

namespace fairdao.portal.shared
{
    public class AppHelper : SysHelper
    {

        public AppHelper(IDataStore dataStore, Env env) : base(dataStore, env)
        {
            env.ClientVer = AppInfo.BuildString;
            env.VersionString = AppInfo.VersionString;


            //ClientEnv.NeedUpdate = true;
        }



        IAudioPlayer msgPlayer;
        IAudioPlayer _MsgPlayer
        {

            get
            {
                if (msgPlayer == null)
                {
                    msgPlayer = Plugin.Maui.Audio.AudioManager.Current.CreatePlayer(FileSystem.OpenAppPackageFileAsync("msg.mp3").Result);

                }
                return msgPlayer;

            }
        }

        IAudioPlayer callPlayer;
        IAudioPlayer _CallPlayer
        {

            get
            {
                if (callPlayer == null)
                {
                    callPlayer = Plugin.Maui.Audio.AudioManager.Current.CreatePlayer(FileSystem.OpenAppPackageFileAsync("call.wav").Result);

                }
                return callPlayer;

            }
        }

        public override async Task PlayAudio(string audio)
        {

            try
            {
                switch (audio)
                {
                    case "msg": //来消息提醒
                        _MsgPlayer.Play();
                        break;
                    case "call": //不循环放
                        _CallPlayer.Play();
                        break;
                    case "call-loop": //循环放
                        _CallPlayer.Play();
                        _CallPlayer.Play();
                        break;
                    case "call-stop":
                        try
                        {
                            _CallPlayer.Stop();
                        }
                        catch
                        {

                        }
                        try
                        {
                            _MsgPlayer.Stop();
                        }
                        catch { }

                        break;
                }
            }
            catch { }
            return;
        }






        public override string AppName => "fairdao";


        public override async Task Init()
        {

            try
            {

                #region 设置客户端配置信息
                string siteName = "程序", domain = "https://xy.run.place:8000";

                //本机测试
                //domain = "http://192.168.1.9:8000";
                //外网：
                //domain = "http://192.168.1.225:10005";// https://a.z0001.xyz:55005"; http://192.168.1.225:10005

                var clientConfig = await dataStore.GetConfig<ClientConfig>("clientConfig");


                if (clientConfig == null) clientConfig = new ClientConfig();

                this.ClientConfig = clientConfig;
                #endregion
                Console.WriteLine($"已配置好客户端数据，至此花费{RunedSpanTime}毫秒");


                await LoadLocalData();
                Console.WriteLine($"已加载本地数据，至此花费{RunedSpanTime}毫秒");


            }
            catch (Exception e)
            {
                Console.WriteLine($"Init失败:{e.Message}");
                // logBuilder.AppendLine(e.StackTrace ?? "");
            }


        }




    }
}




















