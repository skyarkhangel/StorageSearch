#if NoCCL
using System.Reflection;
using RimWorld;
using StorageSearch.NoCCL;
#else
using CommunityCoreLibrary;
#endif
using Verse;

namespace StorageSearch
{
    public class Injector_StorageSearch : SpecialInjector
    {
        ITab_Storage_Enhanced tabStorage;

        public Injector_StorageSearch()
        {
            tabStorage = new ITab_Storage_Enhanced();
        }

        private static Assembly Assembly { get { return Assembly.GetAssembly(typeof(Injector_StorageSearch)); } }

        private static readonly BindingFlags[] bindingFlagCombos = {
            BindingFlags.Instance | BindingFlags.Public, BindingFlags.Static | BindingFlags.Public,
            BindingFlags.Instance | BindingFlags.NonPublic, BindingFlags.Static | BindingFlags.NonPublic
        };

        public override bool Inject()
        {

            #region Automatic hookup
            // Loop through all detour attributes and try to hook them up
            foreach (var targetType in Assembly.GetTypes())
            {
                foreach (var bindingFlags in bindingFlagCombos)
                {
                    foreach (var targetMethod in targetType.GetMethods(bindingFlags))
                    {
                        foreach (DetourAttribute detour in targetMethod.GetCustomAttributes(typeof(DetourAttribute), true))
                        {
                            var flags = detour.bindingFlags != default(BindingFlags) ? detour.bindingFlags : bindingFlags;
                            var sourceMethod = detour.source.GetMethod(targetMethod.Name, flags);
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
            #endregion


            FieldInfo field = typeof(Zone_Stockpile).GetField("StorageTab", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, tabStorage);

            foreach (ThingDef current in DefDatabase<ThingDef>.AllDefs)
            {
                if (current.inspectorTabsResolved != null)
                {
                    bool flag = false;
                    ITab tabToReplace = null;
                    foreach (ITab tab in current.inspectorTabsResolved)
                    {
                        if (tab.GetType() == typeof(ITab_Storage))
                        {
                            flag = true;
                            tabToReplace = tab;
                            break;
                        }
                    }

                    if (flag)
                    {
                        int index = current.inspectorTabsResolved.IndexOf(tabToReplace);
                        current.inspectorTabsResolved.RemoveAt(index);
                        current.inspectorTabsResolved.Insert(index, tabStorage);
                    }
                }

            }
            Log.Message("Injector_StorageSearch : Injected");

            return true;
        }
        
    }
}
