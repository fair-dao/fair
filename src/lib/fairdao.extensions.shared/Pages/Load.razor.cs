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

        public bool Loaded = false;

        public VCommpent CurCommpent;


        public DynamicComponent dynamicComponent;
        public Dictionary<string,object>? ComParameters { get; set; }
        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();
        }

         void LoadComponent()
        {
            
            Type type = null;
            comm = null;
     
            if (ComponentId.IndexOf("--") > 0)
            {
                string strType = ComponentId?.Replace("--", ".");
                type = Type.GetType(strType);


            }
            else
            {
                CurCommpent = sysHelper.GetVCommpent(ComponentId);
                if (CurCommpent != null)
                {
                    if (!string.IsNullOrEmpty(CurCommpent.Link))
                    {
                        ComParameters = new Dictionary<string, object>();
                        ComParameters.Add("VCommpent", CurCommpent);

                        if (CurCommpent.ComType == ComponentType.ThirdLink)
                        {
                            navManager.NavigateTo(CurCommpent.Link);
                            return;
                        }
                        else
                        {
                            type = Type.GetType(CurCommpent.Link);

                        }
                    }

                }
            }

            if (type != null)
            {
                comm = type;
            }
            Loaded = true;
        }

        protected override Task OnParametersSetAsync()
        {

            LoadComponent();

            return base.OnParametersSetAsync();
        }
    }
}
