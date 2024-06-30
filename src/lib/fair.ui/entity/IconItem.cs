using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fair.extensions.shared.entity
{
    /// <summary>
    /// 图标显示项
    /// </summary>
    public class IconItem<T> : IconItem
    {


        /// <summary>
        /// 关联数据(组件，链接，扩展数据等)
        /// </summary>
        public T Link { get; set; }
    }


    public class IconItem
    {
        public string Id { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool? Selected { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Icon { get; set; }

        public string Value { get; set; }

       
    }
}
