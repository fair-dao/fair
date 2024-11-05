using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.entity
{
    /// <summary>
    /// 多语言配置类
    /// </summary>
    public class LocalLang
    {
        /// <summary>
        /// 翻译库
        /// </summary>
        public LangInfo LangWords { get; set; } 

        /// <summary>
        /// 语言表
        /// </summary>
        public List<Culture> Cultures { get; set; }=new List<Culture>();

    }


    /// <summary>
    /// 语言
    /// </summary>
    public class Culture
    {
        public string Name { get; set;  }

        public string DispName { get; set; }
        public string EnglishName { get; set; }
        public string NativeName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
