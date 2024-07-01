using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.entity
{
    public class Env
    {
   
        /// <summary>
        /// 设备宽度
        /// </summary>
        public int DeviceWidth { get; set; }


        /// <summary>
        /// 设备高度
        /// </summary>
        public int DeviceHeight { get; set; }

        /// <summary>
        /// 加载毫秒数
        /// </summary>
        public int? LoadingSecs { get; set; }

        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model{get;set;}

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 客户端版本号
        /// </summary>
        public string ClientVer { get; set; }

        /// <summary>
        /// 发行代号（版本名)
        /// </summary>
        public string VersionString { get; set; }



        /// <summary>
        /// 操作系统
        /// </summary>
        public string OS { get; set; }

    }
}
