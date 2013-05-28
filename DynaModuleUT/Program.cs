#define _NET_4_
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
            DynaModuleASettingPool sp = new DynaModuleASettingPool();
            sp.Settings.Add(sa);
            sp.Settings.Add(sb);
            sp.Settings.Add(sc);

            XmlHelper.Save("TmpSetting.xml", sp);

            DynaModuleManager manager = new DynaModuleManager();
            manager.OnError += new Action<IDynaModule, string>(manager_OnError);
            PluginInfo pi1 = new PluginInfo() { AssemblyFile = "DynaModuleImpl.dll", CreateType = "DynaModuleImpl.DynaModuleA", SettingLoaderType = "DynaModuleImpl.DynaModuleALoader", SettingFile = "TmpSetting.xml" };
#if _NET_4_
            List<DynaModuleA> result = manager.Load<DynaModuleA,DynaModuleSettingA>(pi1);
#else
            List<DynaModuleA> result = manager.Load<DynaModuleA>(pi1);
#endif
        }

        static void manager_OnError(IDynaModule module, string msg)
        {
            Console.WriteLine("Load IDynaModule [{0}] failed with {1}", module, msg);
        }
    }
}

