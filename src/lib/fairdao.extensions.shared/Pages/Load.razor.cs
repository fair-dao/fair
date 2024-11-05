using fairdao.extensions.shared.entity;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.Pages
{
    public partial class Load
    {
        [Parameter]
        public string ComponentId { get; set; }

        [Parameter]
        public string? ComponentParamters { get; set; }

        public Type comm;



        protected override Task OnInitializedAsync()
        {
            Type type = null;
            if (ComponentId.IndexOf("--") > 0)
            {
                string strType = ComponentId?.Replace("--", ".");
                type = Type.GetType(strType);


            }
            else
            {
                VCommpent com = sysHelper.GetVCommpent(ComponentId);
                if (com != null)
                {
                    if (com.ComType == ComponentType.ThirdLink)
                    {
                        navManager.NavigateTo(com.Link);
                        return base.OnInitializedAsync();
                    }
                    else
                    {
                        type = Type.GetType(com.Link);

                    }
                }
            }

            if (type != null)
            {
                comm = type;
            }
            return base.OnInitializedAsync();
        }
    }
}
