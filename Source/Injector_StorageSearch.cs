#if NoCCL
#else
using CommunityCoreLibrary;
#endif
using System.Reflection;

using HaulingHysteresis;

using RimWorld;

using StorageSearch.NoCCL;

using Verse;

namespace StorageSearch
{
    using System;

    public class Injector_StorageSearch : SpecialInjector
    {
        ITab_Storage_Detour tabStorage;

        public Injector_StorageSearch()
        {
            tabStorage = new ITab_Storage_Detour();
        }

        private static Assembly Assembly { get { return Assembly.GetAssembly(typeof(Injector_StorageSearch)); } }

        private static readonly BindingFlags[] bindingFlagCombos = {
            BindingFlags.Instance | BindingFlags.Public, BindingFlags.Static | BindingFlags.Public,
            BindingFlags.Instance | BindingFlags.NonPublic, BindingFlags.Static | BindingFlags.NonPublic
        };

        public override bool Inject()
        {

            
            // Loop through all detour attributes and try to hook them up
            foreach (Type targetType in Assembly.GetTypes())
            {
                foreach (BindingFlags bindingFlags in bindingFlagCombos)
                {
                    foreach (MethodInfo targetMethod in targetType.GetMethods(bindingFlags))
                    {
                        foreach (DetourAttribute detour in targetMethod.GetCustomAttributes(typeof(DetourAttribute), true))
                        {
                            BindingFlags flags = detour.bindingFlags != default(BindingFlags) ? detour.bindingFlags : bindingFlags;
                            MethodInfo sourceMethod = detour.source.GetMethod(targetMethod.Name, flags);
                            if (sourceMethod == null)
                            {
                                Log.Error(string.Format("StorageSearch :: Detours :: Can't find source method '{0} with bindingflags {1}", targetMethod.Name, flags));
                                return false;
                            }

                            if (!Detours.TryDetourFromTo(sourceMethod, targetMethod)) return false;
                        }
                    }
                }
            }
            
            // ToDo reactivate HaulingHysterisis
           MethodInfo method = typeof(StorageSettings).GetMethod("ExposeData", BindingFlags.Instance | BindingFlags.Public);
           MethodInfo method2 = typeof(StorageSettings_Enhanced).GetMethod("ExposeData", BindingFlags.Static | BindingFlags.Public);
           bool flag = !Detours.TryDetourFromTo(method, method2);
           if (!flag)
           {
               method = typeof(StoreUtility).GetMethod("NoStorageBlockersIn", BindingFlags.Static | BindingFlags.NonPublic);
               method2 = typeof(StoreUtility_Detour).GetMethod("NoStorageBlockersIn", BindingFlags.Static | BindingFlags.Public);
               bool flag2 = !Detours.TryDetourFromTo(method, method2);
               if (!flag2)
               {
                   ITab_Storage_Detour.Init();
                   method = typeof(ITab_Storage).GetMethod("FillTab", BindingFlags.Instance | BindingFlags.NonPublic);
                   method2 = typeof(ITab_Storage_Detour).GetMethod("FillTab", BindingFlags.Static | BindingFlags.Public);
                   bool flag3 = !Detours.TryDetourFromTo(method, method2);
                   if (flag3)
                   {
                   }
               }
           }


          // FieldInfo field = typeof(Zone_Stockpile).GetField("StorageTab", BindingFlags.Static | BindingFlags.NonPublic);
          // field.SetValue(null, tabStorage);
          // foreach (ThingDef current in DefDatabase<ThingDef>.AllDefs)
          // {
          // if (current.inspectorTabsResolved != null)
          // {
          // bool flag = false;
          // ITab tabToReplace = null;
          // foreach (ITab tab in current.inspectorTabsResolved)
          // {
          // if (tab.GetType() == typeof(ITab_Storage))
          // {
          // flag = true;
          // tabToReplace = tab;
          // break;
          // }
          // }
          // if (flag)
          // {
          // int index = current.inspectorTabsResolved.IndexOf(tabToReplace);
          // current.inspectorTabsResolved.RemoveAt(index);
          // current.inspectorTabsResolved.Insert(index, tabStorage);
          // }
          // }
          // }
            Log.Message("Injector_StorageSearch : Injected");

            return true;
        }
        
    }
}
