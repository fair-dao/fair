using fair.extensions.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fair.extensions.shared
{
    public interface IDataStore
    {

        #region 配置操作
        /// <summary>
        /// 根据配置名获取配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<T> GetConfig<T>(string configId);


        Task RemoveConfig(string configId);


        Task SetConfig(string configId, object config);
        #endregion

    }

}
