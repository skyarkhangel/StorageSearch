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
            return GeneratedDefs.Keys.StorageSearch.Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect) {
            Settings.DoSettingsWindowContents(inRect);
        }


        [StaticConstructorOnStartup]
        public class Injector {
            static Injector()
            {
                HarmonyInstance harmonyInstance = HarmonyInstance.Create("StorageSearch");
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());              // just use all [HarmonyPatch] decorated classes       
                
                Log.Message("StorageSearch injected...");         
            }
        }
    }
}
