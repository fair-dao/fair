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
        public string ComponentType { get; set; }
     

        public Type comm;


        public DynamicComponent dyComponent;

        protected override Task OnInitializedAsync()
        {
          
            string strType = ComponentType?.Replace("-", ".");
            Console.WriteLine($"com:{strType}");
            Type type = Type.GetType(strType);
            
            if (type != null)
            {
                comm = type;
            }
            return base.OnInitializedAsync();
        }
    }
}
