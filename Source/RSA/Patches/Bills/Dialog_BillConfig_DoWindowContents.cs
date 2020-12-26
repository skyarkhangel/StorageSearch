using HarmonyLib;
using RimWorld;
using RSA.Core;
using RSA.Core.Model;
using RSA.Core.Util;
using System;
using UnityEngine;
using Verse;

namespace RSA {
    [HarmonyPatch(typeof(Dialog_BillConfig), nameof(Dialog_BillConfig.DoWindowContents))]
    class Dialog_BillConfig_DoWindowContents
    {
        private static Func<Dialog_BillConfig, Bill_Production> GetBill = Access.GetFieldGetter<Dialog_BillConfig, Bill_Production>("bill");

        [HarmonyPrefix]
        public static void Before_DoWindowContents(Dialog_BillConfig __instance, Rect inRect) {
            if (!Settings.EnableCraftingFilter)
                return;

            if (ReferenceEquals(__instance.GetType().Assembly, typeof(ITab_Storage).Assembly)) {
                var bill = GetBill(__instance);
                ThingFilterCache.Set(SearchCategories.CategoryID_Bill, bill.ingredientFilter);
            }
        }
    }
}
