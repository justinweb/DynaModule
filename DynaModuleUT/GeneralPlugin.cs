using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using KGI.TW.Der.DynaModuleBase;
using System.Reflection;
using System.Runtime.Remoting;

namespace DynaModuleUT
{
    public interface IPluginSet
    {
        string AssemblyName
        {
            get;
        }

        string InstanceType
        {
            get;
        }
    }

    public class MyPluginSet : IPluginSet
    {
        #region IPluginSet 成員

        public string AssemblyName
        {
            get;
            set;
        }

        public string InstanceType
        {
            get;
            set;
        }

        #endregion

        public string IP
        {
            get;
            set;
        }
    }

    public class PluginSetCollection<TPlugin>
    {
        public List<TPlugin> PluginSet = new List<TPlugin>();
    }

    public class GeneralPlugin
    {
        public static void SavePluginSet()
        {
            PluginSetCollection<MyPluginSet> mySet = new PluginSetCollection<MyPluginSet>();
            mySet.PluginSet.Add(new MyPluginSet() { AssemblyName = "", InstanceType = "DynaModuleUT.MyModule", IP = "10.10.1.79" });
            mySet.PluginSet.Add(new MyPluginSet() { AssemblyName = "", InstanceType = "DynaModuleUT.MyModule", IP = "10.10.1.1" });
            mySet.PluginSet.Add(new MyPluginSet() { AssemblyName = "", InstanceType = "DynaModuleUT.MyModule", IP = "10.10.1.2" });

            XmlHelper.Save("MyPlugIn.xml", mySet); 
        }

        /// <summary>
        /// TMoudle可以用Interface，但TPlugin就必須是最後要使用的型別，因為它會需要讀取xml的資訊
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        /// <typeparam name="TModule"></typeparam>
        /// <returns></returns>
        public static List<DynaModuleInfo<TModule,TPlugin>> LoadPluginSet<TPlugin, TModule>(string filename) where TPlugin : IPluginSet 
        {
            List<DynaModuleInfo<TModule,TPlugin>> result = new List<DynaModuleInfo<TModule,TPlugin>>();

            PluginSetCollection<TPlugin> mySet = XmlHelper.Load<PluginSetCollection<TPlugin>>(filename);

            foreach (TPlugin set in mySet.PluginSet)
            {
                Assembly aa = set.AssemblyName.Length > 0 ? Assembly.LoadFrom(set.AssemblyName) : Assembly.GetExecutingAssembly();
                ObjectHandle oSetting = Activator.CreateInstance(aa.FullName, set.InstanceType);
                if (oSetting != null && oSetting.Unwrap() is TModule)
                {
                    TModule obj = (TModule)oSetting.Unwrap();

                    result.Add( new DynaModuleInfo<TModule,TPlugin>(){ Module = obj, Setting = set } );
                }
            }

            return result;
        }
    }

    public interface IModule
    {
    }

    public class MyModule : IModule
    {
    }


}
