namespace RSA
{
    using Harmony;

    using RimWorld;

    using RSA.HaulingHysterisis;

    using Verse;

    [HarmonyPatch(typeof(StorageSettings), nameof(StorageSettings.ExposeData))]
    public class StorageSettings_ExposeData
    {
        [HarmonyPostfix]
        public static void ExposeData(StorageSettings __instance)
        {
            StorageSettings_Hysteresis storageSettings_Hysteresis = StorageSettings_Mapping.Get(__instance);
            Scribe_Deep.Look(ref storageSettings_Hysteresis, "hysteresis");
            bool flag = storageSettings_Hysteresis != null;
            if (flag)
            {
                StorageSettings_Mapping.Set(__instance, storageSettings_Hysteresis);
            }
        }
    }
}