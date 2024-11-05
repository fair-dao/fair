using System;

namespace fairdao.ui
{

    /// <summary>
    /// 显示列表项
    /// </summary>
    public class UIItem
    {
        /// <summary>
        /// 项目标题
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 项目图标
        /// </summary>
        public string Icon { get; set; }



        /// <summary>
        /// 项目图标颜色 
        /// </summary>
        public string IconColor { get; set; }

        /// <summary>
        /// 项绑定对象
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 是否已选中
        /// </summary>
        public bool Actived { get; set; }

        /// <summary>
        /// 点击事件
        /// </summary>
        public UIItemHandler Clicked { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }

    public delegate void UIItemHandler(UIItem item, EventArgs e);
}
