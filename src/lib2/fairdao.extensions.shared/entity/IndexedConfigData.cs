using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.entity
{
    /// <summary>
    /// IndexedDB配置表包装实体类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IndexedConfigData<T>
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 数据实体
        /// </summary>
        public T Entity { get; set; }
    }
}
