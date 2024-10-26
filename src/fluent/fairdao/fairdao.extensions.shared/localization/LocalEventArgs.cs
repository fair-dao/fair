using System;

namespace fairdao.extensions.shared.localization
{
   /// <summary>
   /// 语言事件参数
   /// </summary>
    public class LocalEventArgs
    {
        /// <summary>
        /// 事件名
        /// </summary>
        public EventType Type { get; set; }

        /// <summary>
        /// 应用名
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 短语
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string Lang { get; set; }

        public IServiceProvider ServiceProvider;
    }
    /// <summary>
    /// 本地化事件类型
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Init=0,
        Get=1,
        /// <summary>
        /// 没有发现短语
        /// </summary>
        NotFound=10
    }
}