using Harmony;
using RimWorld;

namespace StorageSearch {
    [HarmonyPatch(typeof(Dialog_BillConfig), nameof(Dialog_BillConfig.DoWindowContents))]
    class Dialog_BillConfig_DoWindowContents
    {

        [HarmonyPrefix]
        public static void Before_DoWindowContents() {
            ThingFilterUI_DoThingFilterConfigWindow.showSearchCount++;
        }
    }
}
