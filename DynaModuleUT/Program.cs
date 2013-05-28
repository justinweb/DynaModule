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

            UT_DynaModule();

            //GeneralPlugin.SavePluginSet();
            //List<DynaModuleInfo<IModule, MyPluginSet>> result = GeneralPlugin.LoadPluginSet<MyPluginSet, IModule>("MyPlugIn.xml");
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
            // 如果能知道最終型別的話，就使用最終型別作為泛型參數來呼叫
            List<DynaModuleInfo<DynaModuleA, DynaModuleSettingA>> result = DynaModuleManager.LoadModuleInfo<DynaModuleA, DynaModuleSettingA>(pi1, ref errMsg );   

            // 如果不知最終型別的話(可能是PlugIn的外掛模組)，就用基底介面作為泛型參數來呼叫
            // 這時SettingLoaderType必須是繼承自IDynaModuleSettingLoader<TSetting>的
            List<DynaModuleInfo<IModule, ISetting>> resultBase = DynaModuleManager.LoadModuleInfo<IModule, ISetting>(pi1, ref errMsg);  
        }       
    }
}

