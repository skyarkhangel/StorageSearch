﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RSA.Core;
using RSA.Core.Model;
using RSA.Core.Util;
using StorageSearch;
using UnityEngine;
using Verse;

namespace RSA
{
    [HarmonyPatch(typeof(ITab_Storage), "FillTab")]
    public class ITab_Storage_FillTab {

        private static Func<RimWorld.ITab_Storage, IStoreSettingsParent> GetSelStoreSettingsParent;


        static ITab_Storage_FillTab() {
            GetSelStoreSettingsParent = Access.GetPropertyGetter<RimWorld.ITab_Storage, IStoreSettingsParent>("SelStoreSettingsParent");
        }


        [HarmonyPrefix]
        public static void Before_ITab_Storage_FillTab(ITab_Storage __instance) {
            if (ReferenceEquals(__instance.GetType().Assembly, typeof(ITab_Storage).Assembly))
            {
                // only show hysteresis option for non derived (non-custom) storage(s)

                StorageSettings storeSettings = GetSelStoreSettingsParent(__instance).GetStoreSettings();
                ThingFilterCache.Set(SearchCategories.CategoryID_Storage, storeSettings.filter);
                HaulingHysteresis_InjectControls.SettingsQueue.Enqueue(storeSettings);
            }
        }
    }
}
