using fairdao.extensions.shared.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.entity
{
    /// <summary>
    ///  本地语言库
    /// </summary>
    public class LangLib
    {
        /// <summary>
        ///  已下载的语言库
        /// </summary>
        public string LangsJson { get; set; }

        /// <summary>
        /// 所有语言
        /// </summary>
        public List<Culture> Culture { get; set; } = new List<Culture>();

    }


}
