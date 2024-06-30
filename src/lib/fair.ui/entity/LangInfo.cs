using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fair.extensions.shared.entity
{

    /// <summary>
    /// 描述语言包的类
    /// </summary>
    public class LangInfo
    {

        /// <summary>
        /// 语言代号
        /// </summary>
        public string Code { get; set; }


        /// <summary>
        /// 语言显示名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 语言字典列表
        /// </summary>
        public Dictionary<string,string> Dict { get; set; }
    }
}
