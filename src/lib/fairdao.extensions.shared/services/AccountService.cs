
using fairdao.extensions.shared;
using fairdao.extensions.shared.entity;
using fairdao.extensions.shared.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.services
{
    /// <summary>
    /// 账号服务
    /// </summary>
    public class AccountService : IAccountService
    {

        private SysHelper _Helper;
        public string LoginUrl => "";

        public virtual bool LoginRequired => false;

        public AccountService(SysHelper helper)
        {
            _Helper = helper;
          
        }

        private static BlockchainAccount? _account;
      
       public virtual string GetProfilePhoto(BlockchainAccount? account)
        {
            string face = null;
            if (account?.ProfilePhoto == null)
            {
                face = "face";
            }else if(account.ProfilePhoto.Length<5) {
                face = account.ProfilePhoto;
            }
            if (face != null)
            {
                return $"/_content/fairdao.extensions.shared/images.faces/{face}.svg";
            }
            else return account.ProfilePhoto;
        }

        public virtual  async Task<BlockchainAccount> GetAccountAsync()
        {
            //读取当前账号
            if (_account == null)
            {
                List<KeyValuePair<string, string>> accounts = await _Helper.GetCache<List<KeyValuePair<string,string>>>("fair-accounts");
                if (accounts?.Count() > 0)
                {
                    _account = await _Helper.GetCache<BlockchainAccount>($"fair-accounts-{accounts[0].Key}");
                }
            }
            return _account;
        }


        public virtual async Task ChangeAccountAsync(BlockchainAccount account)
        {
            string acc = account?.Account;
            if (!string.IsNullOrEmpty(acc))
            {
                List<KeyValuePair<string, string>> accounts = await _Helper.GetCache<List<KeyValuePair<string, string>>>("fair-accounts");
                if (accounts == null)
                {
                    accounts = new List<KeyValuePair<string, string>>();
                }
                acc = Encrypt.SHA1(acc);
                acc = acc.Substring(5, 6);
                KeyValuePair<string, string>? a = accounts.FirstOrDefault(m => m.Key == acc);
                if (a!=null)
                {
                    accounts.Remove(a.Value);
                }
                accounts.Insert(0, new KeyValuePair<string, string>(acc,account?.NickName??""));

                await _Helper.ReloadConfig();
                await _Helper.SetCache($"fair-accounts-{acc}", account);
                await _Helper.SetCache($"fair-accounts", accounts);
            }
        }

        public virtual async Task Logout()
        {
            _account = null;
            await _Helper.SetCache("mainAccount", null);
        }
    }
}
