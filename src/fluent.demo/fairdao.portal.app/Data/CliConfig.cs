
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientApp.Data
{
    public class CliConfig
    {
        
        [SQLite.PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public string Config { get; set; }
    }
}
