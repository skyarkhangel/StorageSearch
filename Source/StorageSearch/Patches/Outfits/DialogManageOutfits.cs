using Harmony;
using RimWorld;
using UnityEngine;

namespace StorageSearch {
    [HarmonyPatch(typeof(Dialog_ManageOutfits), nameof(Dialog_ManageOutfits.DoWindowContents))]
    class DialogManageOutfits_DoWindowContents {

        [HarmonyPrefix]
        public static void Before_DoWindowContents(Rect inRect) {
            if (!Settings.EnableOutfitFilter)
                return;
            
            ThingFilterUI_DoThingFilterConfigWindow.showSearchCount++;
        }
    }


    [HarmonyPatch(typeof(Dialog_ManageOutfits), nameof(Dialog_ManageOutfits.DoNameInputRect))]
    class DialogManageOutfits_DoNameInputRect {

        [HarmonyPrefix]
        public static void Before_DoNameInputRect(ref Rect rect) {
            if (!Settings.EnableOutfitFilter)
                return;

            // make rename box somewhat smaller (has 35px overlap) + padding, and reduce height by 1 to align boxes, also shift box down
            rect = new Rect(rect.x, rect.y + 5, rect.width - 40f, rect.height -1);
        }

    }
}
