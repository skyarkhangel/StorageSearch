using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using Verse;

namespace StorageSearch
{
    public class StorageSearchMod : Mod
    {
        public StorageSearchMod(ModContentPack content) : base(content)
        {

        }

        [StaticConstructorOnStartup]
        public class Injector {
            static Injector()
            {
                var harmonyInstance = HarmonyInstance.Create("StorageSearch");
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());              // just use all [HarmonyPatch] decorated classes                
            }
        }
    }
}
