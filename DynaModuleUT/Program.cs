using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KGI.IT.Der.DynaModuleBase;
using DynaModuleImpl;

namespace DynaModuleUT
{
    class Program
    {
        static void Main(string[] args)
        {
            UT_DynaModule();
        }

        public static void UT_DynaModule()
        {
            // Setting File
            DynaModuleSettingA sa = new DynaModuleSettingA() { ServiceCode = "SettingA" };
            DynaModuleSettingA sb = new DynaModuleSettingA() { ServiceCode = "SettingB" };
            DynaModuleSettingA sc = new DynaModuleSettingA() { ServiceCode = "SettingC" };
            //DynaModuleASettingPool sp = new DynaModuleASettingPool();
            DynaModuleSettingPool<DynaModuleSettingA> sp = new DynaModuleSettingPool<DynaModuleSettingA>(); 
            sp.Settings.Add(sa);
            sp.Settings.Add(sb);
            sp.Settings.Add(sc);

            if (XmlHelper.Save("TmpSetting.xml", sp) == false)
            {
                Console.WriteLine("Save setting file failed"); 
            }

            DynaModuleManager<DynaModuleSettingA> manager = new DynaModuleManager<DynaModuleSettingA>();
            manager.OnError += new Action<IDynaModule<DynaModuleSettingA> , string>(manager_OnError);
            PluginInfo pi1 = new PluginInfo() { AssemblyFile = "DynaModuleImpl.dll", CreateType = "DynaModuleImpl.DynaModuleA", SettingLoaderType = "DynaModuleImpl.DynaModuleALoader", SettingFile = "TmpSetting.xml" };

            List<DynaModuleA> result = manager.Load<DynaModuleA>(pi1);
        }

        static void manager_OnError(IDynaModule<DynaModuleSettingA> module, string msg)
        {
            Console.WriteLine("Load IDynaModule [{0}] failed with {1}", module, msg);
        }
    }
}

