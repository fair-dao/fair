using fairdao.extensions.shared.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.services
{
    public interface IAccountService
    {
        /// <summary>
        /// 当前账户
        /// </summary>
        Task<WalletAccount> GetWalletAccountAsync();

        /// <summary>
        /// 登录地址
        /// </summary>
        string LoginUrl { get; }

        /// <summary>
        /// 切换账户
        /// </summary>
        /// <param name="account"></param>
        Task ChangeAccountAsync(WalletAccount account);

        /// <summary>
        /// 退出
        /// </summary>
        Task Logout();




    }
}
