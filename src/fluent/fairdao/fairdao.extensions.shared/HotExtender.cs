using fairdao.extensions.shared.entity;
using fairdao.extensions.shared.services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared
{
    /// <summary>
    /// 热扩展（从网络中下载并加载的扩展，要求热扩展插件不能超过10kb，同时必须开源）
    /// </summary>
    public class HotExtender
    {
        #region 扩展器相关属性


        /// <summary>
        /// 插件Id
        /// </summary>
        public virtual string Id { get; set; } = "";


        /// <summary>
        /// 扩展名称
        /// </summary>
        public virtual string Name { get; set; } = "";


        public List<VCommpent> Menus { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public String Version { get; set; }

        /// <summary>
        /// 提供者
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 主程序集名
        /// </summary>
        public virtual string MainAssembly { get; set; }

        /// <summary>
        /// API服务地址列表
        /// </summary>
        public virtual List<string> APIAddresses { get; set; }




        /// <summary>
        /// 提供搜索的类型
        /// </summary>
        public virtual List<string> SearchTypes { get; set; }

        /// <summary>
        /// 提供的可视化组件列表
        /// </summary>
        public virtual VCommpent[]? VCommpents { get; }

        #endregion

        #region 初始化扩展

        public virtual async void Use(IServiceProvider provider)
        {
            var sysHelper = provider.GetRequiredService<SysHelper>();
            var curLang = await sysHelper.GetCurLang();
        }
        #endregion



        #region 为扩展器提供搜索功能

        /// <summary>
        /// 搜索信息
        /// </summary>
        /// <param name="type">信息类型</param>
        /// <param name="key">搜索关键字</param>
        public virtual void Search(string type, string key)
        {

        }

        /// <summary>
        /// 搜索停留提示
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual List<string> SearchPrompt(string type, string key)
        {
            return null;
        }


        /// <summary>
        /// 获取热搜关键词
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual List<string> GetHotKeys(string type)
        {
            return new List<string>();
        }

        #endregion


    }
}
