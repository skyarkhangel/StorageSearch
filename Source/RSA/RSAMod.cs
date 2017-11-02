namespace RSA
{
    using Harmony;
    using RSA.Core;
    using RSA.Languages;
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using Verse;

    public class RSAMod : Mod
    {
        public static bool EnableCraftingFilter = true;

        public static bool EnableOutfitFilter = true;

        private readonly RSACoreMod baseFilterSearchMod;

        public RSAMod(ModContentPack content)
            : base(content)
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("RSA");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly()); // just use all [HarmonyPatch] decorated classes

            bool modifiedExtendedCrafting = TryDetourExtendedCrafting(harmonyInstance);

            Log.Message($"RSA Main injected {(modifiedExtendedCrafting ? "(ExtendedCrafting detected)" : null)}...");

            // supress base mod (mini) settings, we're replicating then in our own extended object
            this.baseFilterSearchMod = LoadedModManager.GetMod<RSACoreMod>();
            if (this.baseFilterSearchMod == null)
            {
                Log.Warning("Base filter mod not found - wrong assembly load orders?");
            }
            else
            {
                this.baseFilterSearchMod.SupressSettings = true;
            }

            this.GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard(GameFont.Small) { ColumnWidth = inRect.width };
            list.Begin(inRect);

            if (this.baseFilterSearchMod != null)
            {
                RSACoreMod.DoSettingsContents(list);
            }

            list.CheckboxLabeled(
                RSAKeys.RSA_ForOutfits.Translate(),
                ref Settings.EnableOutfitFilter,
                RSAKeys.RSA_ForOutfitsTip.Translate());
            list.CheckboxLabeled(
                RSAKeys.RSA_ForCrafting.Translate(),
                ref Settings.EnableCraftingFilter,
                RSAKeys.RSA_ForCraftingTip.Translate());

            if (RSACoreMod.Debug)
            {
                list.GapLine();
                list.Label(RSACoreKeys.RSACore_Debug.Translate(typeof(RSACoreMod).Assembly.GetName().Version));
            }

            if (this.baseFilterSearchMod != null)
            {
                RSACoreMod.DoPreview(list);
            }

            list.End();
        }

        public override string SettingsCategory()
        {
            return RSAKeys.RSA.Translate();
        }

        private static bool TryDetourExtendedCrafting(HarmonyInstance harmony)
        {
            Assembly ecAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "AcEnhancedCrafting");
            if (ecAssembly != null)
            {
                MethodInfo mi = ecAssembly.GetType("AlcoholV.Overriding.Dialog_BillConfig")
                    .GetMethod(nameof(RimWorld.Dialog_BillConfig.DoWindowContents));
                harmony.Patch(
                    mi,
                    new HarmonyMethod(
                        typeof(Dialog_BillConfig_DoWindowContents),
                        nameof(Dialog_BillConfig_DoWindowContents.Before_DoWindowContents)),
                    null,
                    null);
                return true;
            }

            return false;
        }
    }
}