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
        Task<BlockchainAccount> GetAccountAsync();

        /// <summary>
        /// 登录地址
        /// </summary>
        string LoginUrl { get; }

        /// <summary>
        ///  必须登录
        /// </summary>
        bool LoginRequired { get; }

        string GetProfilePhoto(BlockchainAccount? account);


        /// <summary>
        /// 切换账户
        /// </summary>
        /// <param name="account"></param>
        Task ChangeAccountAsync(BlockchainAccount account);

        /// <summary>
        /// 退出
        /// </summary>
        Task Logout();




    }
}
