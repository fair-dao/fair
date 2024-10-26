
using fairdao.extensions.shared.entity;
using fairdao.extensions.shared.services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.main
{
    /// <summary>
    /// 延伸器
    /// </summary>
    public class Extender : fairdao.extensions.shared.MainServiceExtender
    {
        public override string Name { get => "系统"; set => base.Name = value; }

        public override List<string> SearchTypes { get => new List<string> { "功能", "扩展" }; set => base.SearchTypes = value; }

        public override VCommpent[]? VCommpents
        {
            get
            {
                return new[] {
                    new VCommpent
                    {
                        Id = "home",
                        Parent = VCommpent.Page,
                        Icon = new  Icons.Regular.Size20.Home(),
                            SelectedIcon=new Icons.Filled.Size20.Home(),
                        Text = "访达",
                        SortId = -1111111111,
                        ShowMode = MenuShowModes.CommpentMode,
                        Link = "fairdao.extensions.main.Pages.Home,fairdao.extensions.main",
                        SubMenus = new List<VCommpent>
                        {
                            new VCommpent
                            {
                                Id="home-softinfo",
                                Icon =  new Icons.Regular.Size20.Info(),
                                Text = "软件信息",
                                SortId = 150,
                                ShowMode = MenuShowModes.IconMode,
                                Link = "/fair/sysInfo"

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
                        Link = "fairdao.extensions.main.Pages.Found,fairdao.extensions.main"

                    },
                    new VCommpent
                    {
                        Id = "syssetup",
                        Parent = VCommpent.Sidebar,
                        Icon = new Icons.Regular.Size20.Settings(),
                        Text = "系统设置",
                        SortId = 111111111,
                        ShowMode = MenuShowModes.DropdownMode,
                        SubMenus = new List<VCommpent>
                        {
                            new VCommpent
                            {
                                Id = "softinfo",
                                Icon =  new Icons.Regular.Size20.Info(),
                                Text = "软件信息",
                                SortId = 150,
                                ShowMode = MenuShowModes.IconMode,
                                Link = "/fair/sysInfo"

                            }
                        }
                    }


                };
            }
        }

        public override void Config(IServiceCollection services)
        {

            base.Config(services);

        }

    }
}
