using fairdao.extensions.main;
using fairdao.extensions.shared.entity;
using fairdao.extensions.shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.main.Pages
{
    public class Page : fairdao.extensions.shared.ComBase
    {

        public override string CurPlugId => "base";

        [Inject]
        protected SysHelper sysHelper { get; set; }

        [Inject]
        protected Env Env { get; set; }



        [Inject]
        protected fairdao.extensions.shared.localization.GensysLocaler _Localer { get; set; }

        public string this[string name, string page = null]
        {
            get
            {
                return _Localer[name, CurPlugId, page];
            }
        }

        protected async override void OnInitialized()
        {


            base.OnInitialized();

            Uri uri = new Uri(NavManager.Uri);
            string url = uri.ToString();

            if (!string.IsNullOrEmpty(url))
            {
                string back = sysHelper.GetQuery(url, "back");
                if (!string.IsNullOrEmpty(back))
                {//除掉?号并转码
                    BackUrl = back;
                }
            }

        }


    }
}