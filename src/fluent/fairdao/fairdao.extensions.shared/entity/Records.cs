using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.entity
{
    public class Records
    {
        public record AsmData(string Name, string Ver, string Company);

        /// <summary>
        /// 插槽组件
        /// </summary>
        /// <param name="SoltId"></param>
        /// <param name="ComponentType"></param>
        public record SoltComponent(string parentComponentType, string SoltId, int SortId,bool Enabled,string Type,string Component);
    }
}
