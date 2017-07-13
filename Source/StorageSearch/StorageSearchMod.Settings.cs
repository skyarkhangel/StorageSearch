using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace StorageSearch {
    public class Settings : ModSettings {

        public static bool IncludeParentCategory = true;
        public static bool EnableOutfitFilter = true;
        public static bool EnableCraftingFilter = true;

        public override void ExposeData() {
            base.ExposeData();

            Scribe_Values.Look(ref IncludeParentCategory, nameof(IncludeParentCategory), false);
            Scribe_Values.Look(ref EnableOutfitFilter, nameof(EnableOutfitFilter), true);
            Scribe_Values.Look(ref EnableCraftingFilter, nameof(EnableCraftingFilter), true);
        }

        public static void DoSettingsWindowContents(Rect rect) {
            Listing_Standard list = new Listing_Standard(GameFont.Small) {
                ColumnWidth = rect.width
            };
            list.Begin(rect);

            list.CheckboxLabeled(
                GeneratedDefs.SearchFilterKeys.ImprovedFilter_IncludeParentCategory.Translate(),
                ref IncludeParentCategory,
                GeneratedDefs.SearchFilterKeys.ImprovedFilter_IncludeParentCategoryTip.Translate());

            list.CheckboxLabeled(GeneratedDefs.StorageSearchKeys.StorageSearch_ForOutfits.Translate(), ref EnableOutfitFilter, GeneratedDefs.StorageSearchKeys.StorageSearch_ForOutfitsTip.Translate());
            list.CheckboxLabeled(GeneratedDefs.StorageSearchKeys.StorageSearch_ForCrafting.Translate(), ref EnableCraftingFilter, GeneratedDefs.StorageSearchKeys.StorageSearch_ForCraftingTip.Translate());

            list.End();
        }
    }

}
