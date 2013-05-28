using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KGI.TW.Der.DynaModuleBase;

namespace DynaModuleImpl
{
    #region 實作DynaModule
    public class DynaModuleA : IDynaModule
    {
        #region IDynaModule 成員

        public bool Init(IDynaModuleSetting setting, ref string msg)
        {
            msg = "Test Error";
            return true;
        }

        #endregion
    }

    public class DynaModuleSettingA : IDynaModuleSetting
    {
        public bool IsLoad
        {
            get;
            set;
        }

        public string ServiceCode
        {
            get;
            set;
        }
    }

    public class DynaModuleALoader : IDynaModuleSettingLoader
    {
        #region IDynaModuleSettingLoader<TSetting> 成員

        public IEnumerable<IDynaModuleSetting> LoadFromFile(string file)
        {
            DynaModuleSettingPool<DynaModuleSettingA> pool = XmlHelper.Load<DynaModuleSettingPool<DynaModuleSettingA>>(file);

            if (pool == null)
                return null;

            // .NET 4.0 可能可以用Covariance方式直接傳回
            List<IDynaModuleSetting> tmp = new List<IDynaModuleSetting>();
            foreach( IDynaModuleSetting set in pool.Settings )
                tmp.Add(set); 
            return tmp;
        }

        #endregion
    }    

    #endregion
}
