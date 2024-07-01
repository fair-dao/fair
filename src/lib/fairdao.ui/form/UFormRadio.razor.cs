using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace fairdao.ui
{
    public partial class UFormRadio
    {

        /// <summary>
        /// 值
        /// </summary>
        [Parameter]
        public string SelectValue { get; set; }


        ///// <summary>
        ///// 远程获取列表项
        ///// </summary>
        //[Parameter]
        //public string FromUrl { get; set; }

        //[Parameter]
        //public List<SelectItem> Items { get; set; }

        [Parameter]
        public EventCallback<string> Changed { get; set; }



        [Parameter]
        public RenderFragment ChildContent { get; set; }


        public void Radio_Clicked(string value)
        {
            if (Container != null && !string.IsNullOrEmpty(Name))
            {
                Container.SubmitData[this.Name] = value;
            }
            if (Changed.HasDelegate) Changed.InvokeAsync(value);
        }


        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        //protected override async Task OnInitializedAsync()
        //{

        //    //从服务器上获取当前用户可以创建的角色列表
        //    //if (!string.IsNullOrEmpty(FromUrl))
        //    //{
        //    //    Items = new List<SelectItem>();
        //    //    var o = await Http.PostForm<fairdao.extensions.shared.Result<List<SelectItem>>>(FromUrl, "");
        //    //    if (o.state == "ok")
        //    //    {
        //    //        Items = o.data;
        //    //    }
        //    //    else
        //    //    {
        //    //        this.AddErrNotification("加载数据出错",o.msg);
        //    //    }
        //    //}
        //    //if (Container != null)
        //    //{
        //    //    Container.SubmitData[this.Name] = Items.FirstOrDefault(m=>m.Selected)?.Value;
        //    //}

        //}

    }


}
