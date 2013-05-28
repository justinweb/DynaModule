using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KGI.IT.Der.DynaModuleBase;

namespace DynaModuleImpl
{
    #region 實作DynaModule
    public class DynaModuleA : IDynaModule<DynaModuleSettingA>
    {
        #region IDynaModule 成員

        public bool Init(DynaModuleSettingA setting, ref string msg)
        {
            msg = "Test Error";
            return true;
        }

        #endregion
    }

    public class DynaModuleSettingA
    {
        public string ServiceCode
        {
            get;
            set;
        }
    }

    public class DynaModuleALoader : DynaModuleLoaderImpl<DynaModuleSettingA>
    {
    }

    #endregion
}
