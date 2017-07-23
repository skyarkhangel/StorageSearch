using System;
using System.Linq;
using System.Reflection;
using Harmony;
using ImprovedFilter.Util;
using UnityEngine;
using Verse;

namespace ImprovedFilter
{
    public class FilterSearchBaseMod : Mod
    {

        public static bool Debug = false;

        public FilterSearchBaseMod(ModContentPack content) : base(content) {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("FilterSearch");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());              // just use all [HarmonyPatch] decorated classes       

            Log.Message($"Filter search basics injected ...");

            this.GetSettings<Settings>();

            
        }

        public bool SupressSettings { get; set; }

        public override string SettingsCategory() {
            if (SupressSettings)
                return String.Empty;

            return Debug
                ? GeneratedDefs.SearchFilterKeys.ImprovedFilter_Debug.Translate(Assembly.GetExecutingAssembly().GetName().Version)
                : GeneratedDefs.SearchFilterKeys.ImprovedFilter.Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect) {
            Listing_Standard list = new Listing_Standard(GameFont.Small) {
                                        ColumnWidth = inRect.width
                                    };
            list.Begin(inRect);

            DoSettingsContents(list);

            list.End();
        }

        public static void DoSettingsContents(Listing_Standard list) {
            list.CheckboxLabeled(
                GeneratedDefs.SearchFilterKeys.ImprovedFilter_IncludeParentCategory.Translate(),
                ref Settings.IncludeParentCategory,
                GeneratedDefs.SearchFilterKeys.ImprovedFilter_IncludeParentCategoryTip.Translate());
        }
    }
}
