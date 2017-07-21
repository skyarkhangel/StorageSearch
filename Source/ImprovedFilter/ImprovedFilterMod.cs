using System;
using System.Linq;
using System.Reflection;
using Harmony;
using UnityEngine;
using Verse;

namespace ImprovedFilter
{
    public class ImprovedFilterMod : Mod
    {
        public ImprovedFilterMod(ModContentPack content) : base(content)
        {
            this.GetSettings<Settings>();
        }

        public override string SettingsCategory() {
            return GeneratedDefs.SearchFilterKeys.ImprovedFilter.Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect) {
            Settings.DoSettingsWindowContents(inRect);
        }


        [StaticConstructorOnStartup]
        public class Injector
        {
            static Injector()
            {
                HarmonyInstance harmonyInstance = HarmonyInstance.Create("ImprovedFilter");
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());              // just use all [HarmonyPatch] decorated classes       

                Log.Message($"ImprovedFilter injected ...");               
            }
        }
    }
}
