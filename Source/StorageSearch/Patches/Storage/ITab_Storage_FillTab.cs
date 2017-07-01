using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace StorageSearch
{
    [HarmonyPatch(typeof(ITab_Storage), "FillTab")]
    public class ITab_Storage_FillTab {

        private static readonly PropertyInfo piSelStoreSettingsParent;

        static ITab_Storage_FillTab() {
            // accessor for private field
            piSelStoreSettingsParent = typeof(RimWorld.ITab_Storage).GetProperty("SelStoreSettingsParent", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        // TODO: speed up access to private fields (Reflection is *slow*). Maybe wait for @pardeike to add field/property accessors to Harmony (see https://github.com/pardeike/Harmony/issues/20 discussion) ???
        private static IStoreSettingsParent GetSelStoreSettingsParent(RimWorld.ITab_Storage tab) {
            return (IStoreSettingsParent)piSelStoreSettingsParent.GetValue(tab, null);
        }

        [HarmonyPrefix]
        public static void Before_ITab_Storage_FillTab(ITab_Storage __instance) {
            ThingFilterUI_DoThingFilterConfigWindow.showSearchCount++;

            if (ReferenceEquals(__instance.GetType().Assembly, typeof(ITab_Storage).Assembly))
            {
                // only show hysteresis option for non derived (non-custom) storage(s)
                ThingFilterUI_DoThingFilterConfigWindow.showHysteresisCount++;

                IStoreSettingsParent selStoreSettingsParent =  GetSelStoreSettingsParent(__instance);
                ThingFilterUI_DoThingFilterConfigWindow.SettingsQueue.Enqueue(selStoreSettingsParent.GetStoreSettings());
            }
        }
    }
}
