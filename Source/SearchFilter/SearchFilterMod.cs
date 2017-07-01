using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using UnityEngine;
using Verse;

namespace SearchFilter {
    public class SearchFilterMod : Mod {
        public SearchFilterMod(ModContentPack content) : base(content) {
            this.GetSettings<Settings>();
        }

        public override string SettingsCategory() {
            return GeneratedDefs.SearchFilterKeys.SearchFilter.Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect) {
            Settings.DoSettingsWindowContents(inRect);
        }


        [StaticConstructorOnStartup]
        public class Injector {
            static Injector() {
                HarmonyInstance harmonyInstance = HarmonyInstance.Create("SearchFilter");
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());              // just use all [HarmonyPatch] decorated classes       

                // HACK: force static <see cref="SearchCategories" /> ctor access execution *now* - somehow Unity doesn't like it if it happens later during a detour
                Log.Message($"StorageSearch.SearchFilter injected - predefined categories: ({SearchCategories.Bill.Category}, {SearchCategories.Outfit.Category}, {SearchCategories.Storage.Category})...");
            }
            
        }
    }
}
