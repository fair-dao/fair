
using fairdao.extensions.shared.entity;
using fairdao.extensions.shared.services;
using fairdao.ui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared
{
    /// <summary>
    /// 延伸器
    /// </summary>
    public class Extender : fairdao.extensions.shared.MainServiceExtender
    {

        public Extender() : this("FAIR DAO","CopyRight By FAIR DAO", "v1.0")
        {


        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logo">Logo图像（建议高度为32px,长度不超过100px）</param>
        /// <param name="copyRight"></param>
        /// <param name="version"></param>
        /// <param name="commpents"></param>
        public Extender(string logo, string copyRight, string version, params VCommpent[] commpents)
        {
            if (logo.StartsWith("<") || logo.IndexOf(".")<0 )
            {
                Logo = logo;
            }else
            {
                Logo = $"<img src=\"{logo}\" />";
            }

            Version = version;
            CopyRight = copyRight;
            if (commpents?.Count() > 0)
            {
                vCommpents = commpents.ToList();
            }
        }


        public override string Name { get => "系统"; set => base.Name = value; }

        public static string CopyRight { get; set; }


        /// <summary>
        /// 版本信息
        /// </summary>
        public static string Version { get; set; }


        public static string Logo { get; set; }


        public override List<string> SearchTypes { get => new List<string> { "功能", "扩展" }; set => base.SearchTypes = value; }


        private List<VCommpent> vCommpents;

        public override List<VCommpent> VCommpents
        {
            get
            {
                if (vCommpents == null)
                {
                    vCommpents = new List<VCommpent> {
                    new VCommpent
                    {
                        Id = "home",
                        Parent = VCommpent.Page,
                        Icon = new  Icons.Regular.Size20.Home(),
                            SelectedIcon=new Icons.Filled.Size20.Home(),
                        Text = "访达",
                        SortId = -1,
                        ComType = ComponentType.CommpentMode,
                        Link = "fairdao.extensions.shared.Pages.Home,fairdao.extensions.shared",
                        SubCommpents = new List<VCommpent>
                        {
                            new VCommpent
                            {
                                Icon =  new Icons.Regular.Size20.Info(),
                                Text = "软件信息",
                                SortId = 150,
                                ComType = ComponentType.IconMode,
                                Link = "fairdao.extensions.shared.Pages.SoftInfo,fairdao.extensions.shared"
                            }
                        }
                    },
                    new VCommpent
                    {
                        Id = "find",
                        Parent = VCommpent.Page,
                        Icon = new Icons.Regular.Size20.AppsAddIn(),
                        SelectedIcon=new Icons.Filled.Size20.AppsAddIn(),
                        Text = "探索",
                        SortId = 21111,
                        ComType = ComponentType.CommpentMode,
                        Link = "fairdao.extensions.shared.Pages.Found,fairdao.extensions.shared"

                    },

                    new VCommpent
                    {
                        Id = "appcenter",
                        Parent = VCommpent.Sidebar,
                        Icon = new Icons.Regular.Size20.AppsList(),
                        Text = "应用中心",
                        SortId = 10000,
                        ComType = ComponentType.DropdownMode
                    },
                    new VCommpent
                    {
                        Id = "syssetup",
                        Parent = VCommpent.Sidebar,
                        Icon = new Icons.Regular.Size20.Settings(),
                        Text = "系统设置",
                        SortId = Int32.MaxValue-1,
                        ComType = ComponentType.DropdownMode,
                        SubCommpents = new List<VCommpent>
                        {
                            new VCommpent
                            {
                                Icon =  new Icons.Regular.Size20.AppsSettings(),
                                Text = "外观设置",
                                SortId = 1,
                                ComType = ComponentType.IconMode,
                                Link = "fairdao.extensions.shared.Pages.SiteSettings,fairdao.extensions.shared"

                            }
                        }
                    },
                    new VCommpent
                    {
                        Id = "sysinfo",
                        Parent = VCommpent.Sidebar,
                        Icon = new Icons.Regular.Size20.InfoShield(),
                        Text = "关于软件",
                        SortId = Int32.MaxValue,
                        ComType = ComponentType.DropdownMode,
                        SubCommpents = new List<VCommpent>
                        {
                            new VCommpent
                            {
                                Icon =  new Icons.Regular.Size20.Info(),
                                Text = "插件列表",
                                SortId = 150,
                                ComType = ComponentType.IconMode,
                                Link = "fairdao.extensions.shared.Pages.SoftInfo,fairdao.extensions.shared"

                            },
                            new VCommpent
                            {
                                Icon =  new Icons.Regular.Size20.Info(),
                                Text = "开源组件",
                                SortId = 1150,
                                ComType = ComponentType.IconMode,
                                Link = "fairdao.extensions.shared.Pages.OpenSource"
                            }
                        }
                    },
                    new VCommpent
                    {
                        Id="search",
                        Parent=VCommpent.Tool,
                        Icon = new Icons.Regular.Size20.Search(),
                        Text = "查找",
                        SortId = 100,
                        ComType = ComponentType.CommpentMode,
                        Link=  typeof( Pages.Search).ToString()
                    },
                    new VCommpent
                    {
                        Id="github",
                        Parent=VCommpent.Tool,
                        Icon= new FairDaoIcons.Size20.GitHub(),
                        Text="github",
                        SortId=10,
                        ComType= ComponentType.ThirdLink,
                        Link="http://github.com/microsoft/fluentui-blazor"
                    }


                };


                }

                return vCommpents;
            }
        }

        public override void Config(IServiceCollection services)
        {

            base.Config(services);

        }

    }
}
