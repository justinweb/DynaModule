using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting;
using System.Xml.Serialization;
using System.IO;

namespace KGI.IT.Der.DynaModuleBase
{ 
    #region 介面
    /// <summary>
    /// 模組載入設定檔
    /// </summary>
    public class PluginInfo
    {
        /// <summary>
        /// DLL檔案名稱
        /// </summary>
        public string AssemblyFile
        {
            get;
            set;
        }
        /// <summary>
        /// 要載入的模組型別，需實作IDyanModule
        /// </summary>
        public string CreateType
        {
            get;
            set;
        }
        /// <summary>
        /// 模組設定檔載入器，需實作IDynModuleSettingLoader
        /// </summary>
        public string SettingLoaderType
        {
            get;
            set;
        }
        /// <summary>
        /// 設定檔名稱
        /// </summary>
        public string SettingFile
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 支援動態載入模組的介面
    /// </summary>
    public interface IDynaModule
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="setting">設定檔(實作的模組需要自行轉型成自己的設定檔型別)</param>
        /// <param name="msg">錯誤訊息</param>
        /// <returns>成功與否</returns>
        bool Init(IDynaModuleSetting setting, ref string msg);
    }

    /// <summary>
    /// 動態載入模組的設定值
    /// </summary>
    public interface IDynaModuleSetting
    {        
    }

    public interface IDynaModuleSettingLoader<TSetting>
    {
        IEnumerable<TSetting> LoadFromFile(string file); 
    }
    /// <summary>
    /// 設定檔的XML序列化及反序列化類別，用來儲存及讀取設定檔用的
    /// </summary>
    /// <typeparam name="TSetting"></typeparam>
    public class DynaModuleSettingPool<TSetting>
    {
        private List<TSetting> MySetting = new List<TSetting>();

        public List<TSetting> Settings
        {
            get { return MySetting; } 
        }
    }

    public class DynaModuleLoaderImpl<TSetting> : IDynaModuleSettingLoader<TSetting> 
    {
        #region IDynaModuleSettingLoader<TSetting> 成員

        public IEnumerable<TSetting> LoadFromFile(string file)
        {
            DynaModuleSettingPool<TSetting> pool = XmlHelper.Load<DynaModuleSettingPool<TSetting>>(file);

            return pool.Settings;
        }

        #endregion
    }    

    #endregion
    
    /// <summary>
    /// 動態模組管理員
    /// </summary>
    public class DynaModuleManager
    {
        /// <summary>
        /// 回報載入時發生錯誤的事件
        /// </summary>
        public event Action<IDynaModule, string> OnError = null;

        /// <summary>
        /// 載入動態模組
        /// </summary>
        /// <typeparam name="T">實際實作IDynaModule的型別</typeparam> 
        /// <param name="pi1"></param>
        /// <returns></returns>
        public List<T> Load<T,TSetting>(PluginInfo pi1) 
            where T : IDynaModule 
            where TSetting : IDynaModuleSetting 
        {
            List<T> result = new List<T>();

            Assembly aa = pi1.AssemblyFile.Length > 0 ? Assembly.LoadFrom(pi1.AssemblyFile) : Assembly.GetExecutingAssembly();
            // SettingLoaderType是實作IDynModuleSettingLoader的實體類別
            ObjectHandle oSetting = Activator.CreateInstance(aa.FullName, pi1.SettingLoaderType);
            if (oSetting != null && oSetting.Unwrap() is IDynaModuleSettingLoader<TSetting>)
            {
                IDynaModuleSettingLoader<TSetting> loader = (IDynaModuleSettingLoader<TSetting>)oSetting.Unwrap();
                IEnumerable <TSetting> settings = loader.LoadFromFile(pi1.SettingFile);

                if (settings != null)
                {
                    string msg = "";
                    foreach (TSetting set in settings)
                    {
                        // CreateType實作IDynaModule
                        ObjectHandle o = Activator.CreateInstance(aa.FullName, pi1.CreateType);
                        if (o != null && o.Unwrap() is T)
                        {
                            T job = (T)(o.Unwrap());
                            if (job.Init(set, ref msg) == true)
                                result.Add(job);
                            else
                            {
                                if (OnError != null) OnError(job, msg);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }

    public sealed class XmlHelper
    {
        public static bool Save<T>(string filename, T data)
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default);
                xml.Serialize(sw, data);
                sw.Close();

                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public static T Load<T>(string filename)
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default);
                object o = xml.Deserialize(sr);
                T tmp = (T)o;
                sr.Close();

                return tmp;

            }
            catch (Exception exp)
            {
                return default(T);
            }

        }
    }
}
