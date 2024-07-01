using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.localization
{

    /// <summary>
    /// 陌生词记录器接口
    /// </summary>
    public interface INewWordRecorder
    {
        void Record(string lang, string word,string plug,string page);

    }
}

