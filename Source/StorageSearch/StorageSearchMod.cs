using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using ImprovedFilter;
using UnityEngine;
using Verse;

namespace StorageSearch
{
    public class StorageSearchMod : Mod
    {
        public static bool EnableOutfitFilter = true;
        public static bool EnableCraftingFilter = true;

        private FilterSearchBaseMod baseFilterSearchMod;

        public StorageSearchMod(ModContentPack content) : base(content)
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("StorageSearch");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());              // just use all [HarmonyPatch] decorated classes       

            bool modifiedExtendedCrafting = TryDetourExtendedCrafting(harmonyInstance);

            Log.Message($"Filter Search Main injected {(modifiedExtendedCrafting ? "(ExtendedCrafting detected)" : String.Empty)} ...");

            // supress base mod (mini) settings, we're replicating then in our own extended object
            baseFilterSearchMod = LoadedModManager.GetMod<FilterSearchBaseMod>();
            if (baseFilterSearchMod == null)
                Log.Warning("Base filter mod not found - wrong assembly load orders?");
            else
                baseFilterSearchMod.SupressSettings = true;

            this.GetSettings<Settings>();
        }

        public override string SettingsCategory() {
            return GeneratedDefs.StorageSearchKeys.StorageSearch.Translate();
        }


        public override void DoSettingsWindowContents(Rect inRect) {
            Listing_Standard list = new Listing_Standard(GameFont.Small) {
                                        ColumnWidth = inRect.width
                                    };
            list.Begin(inRect);

            if (baseFilterSearchMod != null) {
                FilterSearchBaseMod.DoSettingsContents(list);
            }

            list.CheckboxLabeled(GeneratedDefs.StorageSearchKeys.StorageSearch_ForOutfits.Translate(), ref Settings.EnableOutfitFilter, GeneratedDefs.StorageSearchKeys.StorageSearch_ForOutfitsTip.Translate());
            list.CheckboxLabeled(GeneratedDefs.StorageSearchKeys.StorageSearch_ForCrafting.Translate(), ref Settings.EnableCraftingFilter, GeneratedDefs.StorageSearchKeys.StorageSearch_ForCraftingTip.Translate());

            if (FilterSearchBaseMod.Debug) {
                list.GapLine();
                list.Label(ImprovedFilter.GeneratedDefs.SearchFilterKeys.ImprovedFilter_Debug.Translate(typeof(FilterSearchBaseMod).Assembly.GetName().Version));
            }

            list.End();
        }

        private static bool TryDetourExtendedCrafting(HarmonyInstance harmony) {
            var ecAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "AcEnhancedCrafting");
            if (ecAssembly != null) {
                MethodInfo mi = ecAssembly.GetType("AlcoholV.Overriding.Dialog_BillConfig").GetMethod(nameof(RimWorld.Dialog_BillConfig.DoWindowContents));
                harmony.Patch(mi, new HarmonyMethod(typeof(Dialog_BillConfig_DoWindowContents), nameof(Dialog_BillConfig_DoWindowContents.Before_DoWindowContents)), null, null);
                return true;
            }
            return false;
        }
        
    }
}
