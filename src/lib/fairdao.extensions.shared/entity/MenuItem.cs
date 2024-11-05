using Microsoft.FluentUI.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.entity
{

    /// <summary>
    /// 可视组件
    /// </summary>
    public class VCommpent : ICloneable
    {
        /// <summary>
        /// 边栏导航组件
        /// </summary>
        public const string Sidebar = "sidebar";
        /// <summary>
        /// 底部导航组件
        /// </summary>
        public const string Page = "tabpage";

        /// <summary>
        /// 右上角工具组件
        /// </summary>
        public const string Tool = "toolbar";

        /// <summary>
        /// 可视组件Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 父组件Id
        /// </summary>
        public string Parent { get; set; }



        /// <summary>
        /// 是否为系统保留组件
        /// </summary>
        public bool? Reserved { get; set; }


        /// <summary>
        /// 图标或图片文件
        /// </summary>
        public Icon Icon { get; set; }

        /// <summary>
        /// 选中状态下的图标
        /// </summary>
        public Icon SelectedIcon { get; set; }

        /// <summary>
        /// 菜单文本
        /// </summary>
        public string Text { get; set; }

      
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 热点数
        /// </summary>
        public int? Hots { get; set; }
        /// <summary>
        /// 颜色 
        /// </summary>
        public string IconColor { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 排序Id
        /// </summary>
        public int? SortId { get; set; }

        /// <summary>
        /// 启用禁用，展开
        /// </summary>
        public int? State { get; set; }
        /// <summary>
        /// 组件类型
        /// </summary>
        public ComponentType ComType { get; set; } = ComponentType.IconMode;

        /// <summary>
        /// 链接网址或组件类型（组件模式的时候）
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 允许操作的角色列表 
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        ///  参数列表
        /// </summary>
        public List<string> Params { get; set; }

        private List<VCommpent> vCommpents=new List<VCommpent>();

        /// <summary>
        /// 子组件集合
        /// </summary>
        public List<VCommpent> SubCommpents
        {
            get
            {
                return vCommpents;
            }
            set
            {
                var a = value;
                if (a != null)
                {
                    foreach (var sub in a)
                    {
                        if (sub.Parent == null)
                        {
                            sub.Parent = Id;
                        }
                        sub.Id = $"{Id}-{BitConverter.ToUInt32(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes($"{sub.Text}-{sub.ComType}-{sub.Link}-{sub.Params}")))}";
                    }
                }
                vCommpents = value;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 是否可展开
        /// </summary>
        public bool Expand { get; set; }
    }





    /// <summary>
    /// 组件呈现模式
    /// </summary>

    public enum ComponentType
    {
        /// <summary>
        /// 第三方链接
        /// </summary>
        ThirdLink = 0,
        /// <summary>
        /// 桌面图标模式
        /// </summary>
        IconMode = 1,

        /// <summary>
        /// 列表模式
        /// </summary>
        ListMode = 2,
        /// <summary>
        /// 大图菜单项模式
        /// </summary>
        BigMenuItem = 10,
        /// <summary>
        /// 下拉菜单模式
        /// </summary>
        DropdownMode = 20,
        /// <summary>
        /// 组件模式
        /// </summary>
        CommpentMode = 100,

        /// <summary>
        /// 任何模式
        /// </summary>
        Any=1000

    }

}
