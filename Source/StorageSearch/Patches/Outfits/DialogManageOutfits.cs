using System;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using StorageSearch.Util;
using UnityEngine;

namespace StorageSearch {
    [HarmonyPatch(typeof(Dialog_ManageOutfits), nameof(Dialog_ManageOutfits.DoWindowContents))]
    class DialogManageOutfits_DoWindowContents
    {

        private static Func<Dialog_ManageOutfits, Outfit> GetSelectedOutfit;

        static DialogManageOutfits_DoWindowContents() {
            GetSelectedOutfit = Access.GetPropertyGetter<Dialog_ManageOutfits, Outfit>("SelectedOutfit");
        }

        [HarmonyPrefix]
        public static void Before_DoWindowContents(Rect inRect, Dialog_ManageOutfits __instance) {
            if (GetSelectedOutfit(__instance) == null)
                return;

            if (!Settings.EnableOutfitFilter)
                return;
            
            ThingFilterUtil.QueueNextInvocationSearch(SearchCategories.Outfit);
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
