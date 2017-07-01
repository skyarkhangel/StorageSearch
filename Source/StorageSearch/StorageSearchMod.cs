using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using UnityEngine;
using Verse;

namespace StorageSearch
{
    public class StorageSearchMod : Mod
    {
        public StorageSearchMod(ModContentPack content) : base(content)
        {
            this.GetSettings<Settings>();
        }

        public override string SettingsCategory() {
            return GeneratedDefs.StorageSearchKeys.StorageSearch.Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect) {
            Settings.DoSettingsWindowContents(inRect);
        }


        [StaticConstructorOnStartup]
        public class Injector
        {
            static Injector()
            {
                HarmonyInstance harmonyInstance = HarmonyInstance.Create("StorageSearch");
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());              // just use all [HarmonyPatch] decorated classes       

                bool modifiedExtendedCrafting = TryDetourExtendedCrafting(harmonyInstance);               

                Log.Message($"StorageSearch injected {(modifiedExtendedCrafting ? "(ExtendedCrafting detected)" : String.Empty)} ...");         
            }

            private static bool TryDetourExtendedCrafting(HarmonyInstance harmony) {
                var ecAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "AcEnhancedCrafting");
                if (ecAssembly != null)
                {
                    MethodInfo mi = ecAssembly.GetType("AlcoholV.Overriding.Dialog_BillConfig").GetMethod(nameof(RimWorld.Dialog_BillConfig.DoWindowContents));
                    harmony.Patch(mi, new HarmonyMethod(typeof(Dialog_BillConfig_DoWindowContents), nameof(Dialog_BillConfig_DoWindowContents.Before_DoWindowContents)), null, null);
                    return true;
                }
                return false;
            }
        }
    }
}
