using System;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RSA.Core;
using RSA.Core.Model;
using RSA.Core.Util;
using UnityEngine;

namespace RSA {
    [HarmonyPatch(typeof(Dialog_ManageOutfits), nameof(Dialog_ManageOutfits.DoWindowContents))]
    class DialogManageOutfits_DoWindowContents
    {

        private static Func<Dialog_ManageOutfits, Outfit> GetSelectedOutfit;

        static DialogManageOutfits_DoWindowContents() {
            GetSelectedOutfit = Access.GetPropertyGetter<Dialog_ManageOutfits, Outfit>("SelectedOutfit");
        }

        [HarmonyPrefix]
        public static void Before_DoWindowContents(Dialog_ManageOutfits __instance, Rect inRect) {
            Outfit outfit = GetSelectedOutfit(__instance);
            if (outfit == null)
                return;

            if (!Settings.EnableOutfitFilter)
                return;

            if (__instance.GetType().Assembly == typeof(Dialog_ManageOutfits).Assembly)
                ThingFilterCache.Set(SearchCategories.CategoryID_Outfit, outfit.filter);
        }
    }
}
