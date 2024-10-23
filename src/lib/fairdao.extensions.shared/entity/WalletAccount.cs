using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.entity
{

    /// <summary>
    /// 区块链账户
    /// </summary>
    public class BlockchainAccount
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 网络类型
        /// </summary>
        public string ChainId { get; set; }


        /// <summary>
        /// 账户类型（inner 本地账户，outer 外部账户)
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// 通讯Token([日期+账号组成的字符串的签名]组成：如 2024-01-22#abcdef12335888....)
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string ProfilePhoto { get; set; }

        /// <summary>
        /// Token过期时间
        /// </summary>
        public long TokenExpTime { get; set; }

        /// <summary>
        /// 获取头部添加的身份信息
        /// </summary>
        /// <returns></returns>
        public string GetHttpHeader()
        {
            return $"{ChainId}#{Account}#{NickName}#{TokenExpTime}#{Sign}";
        }


        /// <summary>
        /// 检测账户信息
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void CheckAccountData()
        {
            if (!Account.StartsWith("0x")) { throw new ArgumentException("账号应为一个标准的ETH钱包地址"); };
            if (NickName==null || NickName.Length>8 || NickName.Length<2) { throw new ArgumentException("昵称应在2~8个字符之间"); };
        }

    }
}
