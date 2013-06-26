using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting;
using System.Xml.Serialization;
using System.IO;

namespace KGI.TW.Der.DynaModuleBase
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
        /// 要載入的模組型別
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
    /// 設定檔載入器
    /// 因為要載入的設定檔實際型別是在未來實作端才知道的，所以提供實作的人需要實作一個載入實際設定檔型別的載入器
    /// </summary>
    /// <typeparam name="TSetting"></typeparam>
    public interface IDynaModuleSettingLoader<TSetting>
    {
        /// <summary>
        /// 由指定的檔案(通常是份xml檔)載入實際的設定檔
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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

    #endregion

    public class DynaModuleInfo<TModule,TSetting>
    {
        public TModule Module = default(TModule);
        public TSetting Setting = default(TSetting);
    }
    
    /// <summary>
    /// 動態模組管理員
    /// </summary>    
    public class DynaModuleManager
    {
        /// <summary>
        /// 由PluginInfo中指定的資訊載入設定檔，並產生PluginInfo.CreateType所指定之型別的物件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSetting"></typeparam>
        /// <param name="pi1"></param>
        /// <returns></returns>
        public static List<DynaModuleInfo<T, TSetting>> LoadModuleInfo<T, TSetting>(PluginInfo pi1, ref string errMsg )
        {
            List<DynaModuleInfo<T, TSetting>> result = new List<DynaModuleInfo<T, TSetting>>();

            try
            {
                Assembly aa = pi1.AssemblyFile.Length > 0 ? Assembly.LoadFrom(pi1.AssemblyFile) : Assembly.GetExecutingAssembly();
                // SettingLoaderType是實作IDynModuleSettingLoader的實體類別
                ObjectHandle oSetting = Activator.CreateInstance(aa.FullName, pi1.SettingLoaderType);
                if (oSetting != null && oSetting.Unwrap() is IDynaModuleSettingLoader<TSetting>)
                {
                    IDynaModuleSettingLoader<TSetting> loader = (IDynaModuleSettingLoader<TSetting>)oSetting.Unwrap();
                    IEnumerable<TSetting> settings = loader.LoadFromFile(pi1.SettingFile);

                    if (settings != null)
                    {
                        foreach (TSetting set in settings)
                        {
                            // CreateType實作IDynaModule
                            ObjectHandle o = Activator.CreateInstance(aa.FullName, pi1.CreateType);
                            if (o != null && o.Unwrap() is T)
                            {
                                T job = (T)(o.Unwrap());

                                // Add to result
                                result.Add(new DynaModuleInfo<T, TSetting>() { Module = job, Setting = set });
                            }
                        }
                    }
                    else
                    {
                        errMsg = "載入設定檔錯誤";
                    }
                }
                else
                {
                    errMsg = "指定的SettingLoaderType不是繼承自 " + typeof(IDynaModuleSettingLoader<TSetting>).ToString();
                }
            }
            catch (Exception exp)
            {
                errMsg = exp.Message;
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
