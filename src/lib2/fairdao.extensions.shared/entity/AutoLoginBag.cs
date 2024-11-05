using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.entity
{
    /// <summary>
    /// 自动登录包
    /// </summary>
    public class AutoLoginBag
    {

        /// <summary>
        /// 账号（公钥）
        /// </summary>
        public string Account { get; set; }

        public string NickName { get; set; }
        public long ExpTime { get; set; }

        /// <summary>
        /// 请求域
        /// </summary>
        public string Domain { get; set; }


        /// <summary>
        /// 签名类型（签名算法）
        /// </summary>
        public string SignType { get; set; }


        /// <summary>
        /// 签名 （针对 Account+"#"+NickName+"#"+ExpTime+"#"+Domain 拼接的字符串进行签名）
        /// </summary>
        public string Sign { get; set; }
    }
}
