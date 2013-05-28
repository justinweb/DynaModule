#define _NET_4_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KGI.IT.Der.DynaModuleBase;

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
        public string ServiceCode
        {
            get;
            set;
        }
    }

#if _NET_4_
    public class DynaModuleASettingPool : DynaModuleSettingPool<DynaModuleSettingA>
    {       
    }

    public class DynaModuleALoader : IDynaModuleSettingLoader<DynaModuleSettingA> 
    {
        #region IDynModuleSettingLoader 成員

        public IEnumerable<DynaModuleSettingA> LoadFromFile(string file)
        {
            DynaModuleASettingPool pool = XmlHelper.Load<DynaModuleASettingPool>(file);
            if (pool != null)
            {
                return pool.Settings;
            }
            else
                return null;
        }

        #endregion
    }
#else
    public class DynaModuleALoader : IDynaModuleSettingLoader
    {
        #region IDynModuleSettingLoader 成員

        public IEnumerable<IDynaModuleSetting> LoadFromFile(string file)
        {
            DynaModuleASettingPool pool = XmlHelper.Load<DynaModuleASettingPool>(file);
            if (pool != null)
            {
                return pool.Settings;
            }
            else
                return null;
        }

        #endregion
    }

    /// <summary>
    /// 要自行實作一個放置IDynaModuleSetting的容器類別
    /// 可以用泛型實作，但要.NET 4.0才行
    /// </summary>
    public class DynaModuleASettingPool
    {
        public List<DynaModuleSettingA> Settings = new List<DynaModuleSettingA>();
    }
#endif

    public interface GDynaModuleSettingPool<T> where T : IDynaModuleSetting
    {
        List<T> Settings
        {
            get;
        }
    }

    public class GImplSettingPoolA : GDynaModuleSettingPool<DynaModuleSettingA>
    {
        public List<DynaModuleSettingA> MySettings = new List<DynaModuleSettingA>(); 

        #region GDynaModuleSettingPool<DynaModuleSettingA> 成員

        public List<DynaModuleSettingA> Settings
        {
            get { return MySettings; }
        }

        #endregion
    }
    #endregion
}
