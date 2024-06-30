using System;
using System.Net.Http;

namespace fair.ui
{

    /// <summary>
    /// 自动提交表单参数表
    /// </summary>
    public class AutoSubmit
    {

        /// <summary>
        /// 提交地址(必须提供)
        /// </summary>
        public string Url { get; set; }


        /// <summary>
        /// 提交方式（from或json)
        /// </summary>
        public string Type { get; set; } = "form";

        /// <summary>
        /// 提交方法
        /// </summary>
        public HttpMethod Method { get; set; } = HttpMethod.Post;


        /// <summary>
        /// 成功提交后重定向地址(back[默认]:返回不刷新;go,返回并刷新;链接:直接跳转到指定页面)
        /// </summary>
        public string FinishedRedirect { get; set; }



        /// <summary>
        /// 提交后触发
        /// </summary>
        public Action<fair.extensions.shared.Result> ProcessResult { get; set; }





    }
}
