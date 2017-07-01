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

        public override void ExposeData() {
            base.ExposeData();

            Scribe_Values.Look(ref IncludeParentCategory, nameof(IncludeParentCategory), false);
            Scribe_Values.Look(ref EnableOutfitFilter, nameof(EnableOutfitFilter), true);
        }

        public static void DoSettingsWindowContents(Rect rect) {
            Listing_Standard list = new Listing_Standard(GameFont.Small) {
                ColumnWidth = rect.width
            };
            list.Begin(rect);

            list.CheckboxLabeled(GeneratedDefs.Keys.StorageSearch_IncludeParentCategory.Translate(), ref IncludeParentCategory, GeneratedDefs.Keys.StorageSearch_IncludeParentCategoryTip.Translate());

            list.CheckboxLabeled(GeneratedDefs.Keys.StorageSearch_ForOutfits.Translate(), ref EnableOutfitFilter, GeneratedDefs.Keys.StorageSearch_ForOutfitsTip.Translate());

            list.End();
        }
    }

}
