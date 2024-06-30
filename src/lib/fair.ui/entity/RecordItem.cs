using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fair.extensions.shared.entity
{
    /// <summary>
    /// 记录里常用的字段
    /// </summary>
    public class RecordItem
    {

        public Guid? Id { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string? Title { get; set; }

        public int? State { get; set; }

        public bool? IsDeleted { get; set; }

        public string? Address { get; set; }

        public string? Type { get; set; }

        public decimal? Balance { get; set; }   

        public decimal? Price { get; set; } 

        public decimal? Amount { get; set; }

        /// <summary>
        /// 币类型
        /// </summary>
        public int? CoinId { get; set; }

        public string? Key { get; set; }

        public int? Count { get; set; }  
        /// <summary>
        /// 关联Id
        /// </summary>
        public string? LinkId { get; set; }

        public string? Remark { get; set; } 

        /// <summary>
        /// 卡密信息
        /// </summary>
        public string? DeliveryMessage { get; set; }


        /// <summary>
        /// 小数点位数
        /// </summary>
        public int? Point { get; set; }
    }
}
