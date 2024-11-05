using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.entity
{

    /// <summary>
    /// 提示信息
    /// </summary>
    public class AlertMessage
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";

        /// <summary>
        /// 输入框动作
        /// </summary>
        public Action<string> Ok { get; set; }

        /// <summary>
        /// 确定框动作
        /// </summary>
        public Action Yes { get; set; }
        
        public Action Cancel { get; set; }

        /// <summary>
        /// 默认值 
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public AlertMessageType Type { get; set; } = AlertMessageType.Info;

        /// <summary>
        /// 展示类型
        /// </summary>
        public ShowTypes ShowType { get; set; } = ShowTypes.Alert;
        public string ButtonTexts { get; set; }
    }


    public enum ShowTypes
    {
        /// <summary>
        /// 提示信息框
        /// </summary>
        Alert = 1,
        /// <summary>
        /// 确认框
        /// </summary>
        Confirm = 2,
        /// <summary>
        /// 数据接收框
        /// </summary>
        Prompt = 3,
        /// <summary>
        /// 右下角信息
        /// </summary>
        Message =4,
        /// <summary>
        /// 顶部通知
        /// </summary>
        Notification=5
        
  
    }

    /// <summary>
    /// 提示信息类型
    /// </summary>
    public enum AlertMessageType
    {
        /// <summary>
        /// 普通信息
        /// </summary>
        Info =0,
        /// <summary>
        /// 警告信息
        /// </summary>
        Warn=8,
        /// <summary>
        /// 错误信息
        /// </summary>
        Err=16
    }


}
