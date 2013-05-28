using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KGI.TW.Der.DynaModuleBase;
using DynaModuleImpl;

namespace DynaModuleUT
{
    class Program
    {
        static void Main(string[] args)
        {
            //DynaModuleALoader loader = new DynaModuleALoader();
            //bool isLoader = loader is IDynaModuleSettingLoader<DynaModuleSettingA>;  

            //UT_DynaModule();

            GeneralPlugin.SavePluginSet();
            List<DynaModuleInfo<IModule, MyPluginSet>> result = GeneralPlugin.LoadPluginSet<MyPluginSet, IModule>("MyPlugIn.xml");
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
                              
            PluginInfo pi1 = new PluginInfo() { AssemblyFile = "DynaModuleImpl.dll", CreateType = "DynaModuleImpl.DynaModuleA", SettingLoaderType = "DynaModuleImpl.DynaModuleALoader", SettingFile = "TmpSetting.xml" };

            string errMsg = "";
            List<DynaModuleInfo<DynaModuleA, DynaModuleSettingA>> result = DynaModuleManager.LoadModuleInfo<DynaModuleA, DynaModuleSettingA>(pi1, ref errMsg );   
        }       
    }
}

