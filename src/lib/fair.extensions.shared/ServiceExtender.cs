using fair.extensions.shared.entity;
using fair.extensions.shared.services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fair.extensions.shared
{
    /// <summary>
    /// 延伸器
    /// </summary>
    public class ServiceExtender :Extender
    {

        #region 为扩展器注册服务及应用服务

        public virtual void Config(IServiceCollection services)
        {
            int index = this.GetType().Assembly.FullName.IndexOf(',');
            AssemblyName = this.GetType().Assembly.FullName.Substring(0, index);
        }

        #endregion




    }
}
