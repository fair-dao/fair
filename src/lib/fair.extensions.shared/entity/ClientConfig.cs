using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

using System.Text.Json.Serialization;
using System.Reflection;


namespace fair.extensions.shared.entity
{

    /// <summary>
    /// 客户端配置信息
    /// </summary>
    public class ClientConfig
    {
        public const string StoreId = "clientConfig";

        /// <summary>
        /// 在数据库中存储的Id
        /// </summary>
        public string Id { get; } = StoreId;




        /// <summary>
        /// APP名字
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// log图片
        /// </summary>
        public string Logo { get; set; }


        /// <summary>
        /// 手机端Tabs菜单
        /// </summary>
        public List<fair.extensions.shared.entity.VCommpent> MTabs { get; set; }


        /// <summary>
        /// 边栏菜单
        /// </summary>
        public List<fair.extensions.shared.entity.VCommpent> Sides { get; set; }

     




        /// <summary>
        /// 视频中转服务列表
        /// </summary>
        public List<IceServer> IceServers { get; set; }

    }





}
