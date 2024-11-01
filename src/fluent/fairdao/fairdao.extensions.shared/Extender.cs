
using fairdao.extensions.shared.entity;
using fairdao.extensions.shared.services;
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

        public Extender() : this("", "")
        {


        }

        public Extender(string copyRight, string version, params VCommpent[] commpents)
        {

            var asm = SysHelper.GetAsmData(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
            if (string.IsNullOrEmpty(version))
            {
                Version = $"Version {asm.Ver}";
            }
            else Version = version;
            if (string.IsNullOrEmpty(copyRight))
            {
                CopyRight = $"Copyright by {asm.Company}";
            }
            else CopyRight = copyRight;
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
                        ShowMode = MenuShowModes.CommpentMode,
                        Link = "fairdao.extensions.shared.Pages.Home,fairdao.extensions.shared",
                        SubCommpents = new List<VCommpent>
                        {
                            new VCommpent
                            {
                                Icon =  new Icons.Regular.Size20.Info(),
                                Text = "软件信息",
                                SortId = 150,
                                ShowMode = MenuShowModes.IconMode,
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
                        ShowMode = MenuShowModes.CommpentMode,
                        Link = "fairdao.extensions.shared.Pages.Found,fairdao.extensions.shared"

                    },
                    new VCommpent
                    {
                        Id = "syssetup",
                        Parent = VCommpent.Sidebar,
                        Icon = new Icons.Regular.Size20.Settings(),
                        Text = "系统设置",
                        SortId = 111111111,
                        ShowMode = MenuShowModes.DropdownMode,
                        SubCommpents = new List<VCommpent>
                        {
                            new VCommpent
                            {
                                Icon =  new Icons.Regular.Size20.AppsSettings(),
                                Text = "外观设置",
                                SortId = 1,
                                ShowMode = MenuShowModes.IconMode,
                                Link = "fairdao.extensions.shared.Pages.SiteSettings,fairdao.extensions.shared"

                            }
                        }
                    },
                    new VCommpent
                    {
                        Id = "sysinfo",
                        Parent = VCommpent.Sidebar,
                        Icon = new Icons.Regular.Size20.Settings(),
                        Text = "关于软件",
                        SortId = Int32.MaxValue,
                        ShowMode = MenuShowModes.DropdownMode,
                        SubCommpents = new List<VCommpent>
                        {
                            new VCommpent
                            {
                                Icon =  new Icons.Regular.Size20.Info(),
                                Text = "软件信息",
                                SortId = 1150,
                                ShowMode = MenuShowModes.IconMode,
                                Link = "fairdao.extensions.shared.Pages.SoftInfo,fairdao.extensions.shared"

                            },
                            new VCommpent
                            {
                                Icon =  new Icons.Regular.Size20.Info(),
                                Text = "开源组件",
                                SortId = 1150,
                                ShowMode = MenuShowModes.IconMode,
                                Link = "fairdao.extensions.shared.Pages.SoftInfo,fairdao.extensions.shared"
                            }
                        }
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
