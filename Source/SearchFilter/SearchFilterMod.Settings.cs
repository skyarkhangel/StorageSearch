using UnityEngine;
using Verse;

namespace SearchFilter {
    public class Settings : ModSettings {

        public static bool IncludeParentCategory = true;

        public override void ExposeData() {
            base.ExposeData();

            Scribe_Values.Look(ref IncludeParentCategory, nameof(IncludeParentCategory), false);
        }

        public static void DoSettingsWindowContents(Rect rect) {
            Listing_Standard list = new Listing_Standard(GameFont.Small) {
                ColumnWidth = rect.width
            };
            list.Begin(rect);

            list.CheckboxLabeled(GeneratedDefs.SearchFilterKeys.SearchFilter_IncludeParentCategory.Translate(), ref IncludeParentCategory, GeneratedDefs.SearchFilterKeys.SearchFilter_IncludeParentCategoryTip.Translate());

            list.End();
        }
    }

}
