
using fair.extensions.shared;
using Microsoft.AspNetCore.Components;
using System;


namespace fair.ui
{
    /// <summary>
    /// 表单项
    /// </summary>
    public class FormItem<T> : UIBase
    {
        [Parameter]
        /// <summary>
        /// 表单Label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 父容器
        /// </summary>
        [CascadingParameter]
        public UForm Container { get; set; }


        /// <summary>
        /// 表单名
        /// </summary>
        [Parameter]
        public string Name { get; set; }


        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }



        /// <summary>
        /// 修改后自动提交地址(提交为Form方式，key=Name&val=Value)
        /// </summary>
        [Parameter]
        public string ChangedPostUrl { get; set; }


        /// <summary>
        /// 是否通过了验证
        /// </summary>
        public bool VailPassed { get; set; } = true;

        public object SetContainerValue(string name, object value)
        {
            object val = value;
            if (value is DateTime)
            {
                DateTime utcTime = (DateTime)value;
                if (utcTime.Kind != DateTimeKind.Utc)
                {
                    utcTime = utcTime.ToUniversalTime();
                }
                val = utcTime.ToString("r");
            }
            if (Container != null && this.Name!=null) Container.SubmitData[this.Name] = val;
            return val;
        }


        public virtual void OnValueChanged(ChangeEventArgs e)
        {
            //string val = e.Value.ToString();

            if (VailPassed)
            {
                if (this.Name != null)
                {
                    object value = SetContainerValue(this.Name, e.Value);

                    if (ValueChanged.HasDelegate)
                    {
                        ValueChanged.InvokeAsync((T)e.Value);
                    }

                    if (!string.IsNullOrEmpty(ChangedPostUrl))
                    {
                        httpClient.With(ThrowException).PostResult<string>(ChangedPostUrl, $"key={this.Name}&val={value}").ContinueWith(t =>
                        {
                            if (t.Exception == null)
                            {
                                if (ValueChanged.HasDelegate)
                                {
                                    this.InvokeAsync(() =>
                                    {
                                        ValueChanged.InvokeAsync((T)e.Value);
                                    });

                                }
                            }

                        });
                    }
                }
                else
                {

                    if (ValueChanged.HasDelegate)
                    {
                        ValueChanged.InvokeAsync((T)e.Value);
                    }
                }
            }

        }
    }
}
