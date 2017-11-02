namespace RSA.Core
{
    using Harmony;
    using RSA.Languages;
    using System.Reflection;
    using UnityEngine;
    using Verse;

    public class RSACoreMod : Mod
    {
        public static bool Debug = false;

        private static readonly ThingFilter filter = new ThingFilter(); // dummy filter for ui settings preview

        private static Vector2 scrollPosition;

        public RSACoreMod(ModContentPack content)
            : base(content)
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("RSA.Core");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly()); // just use all [HarmonyPatch] decorated classes

            Log.Message("RSA Core loaded...");

            this.GetSettings<Settings>();
        }

        public bool SupressSettings { get; set; }

        public static void DoPreview(Listing_Standard list)
        {
            list.Gap();

            const float previewHeight = 250f;

            Rect rect = list.GetRect(previewHeight);
            Text.Font = GameFont.Tiny;
            Widgets.Label(
                new Rect(rect.x, rect.y, rect.width * 0.5f, Text.LineHeight),
                RSACoreKeys.RSACore_Preview.Translate());

            ThingFilterUtil.QueueNextInvocationSearch(SearchCategories.TermFor("Settings"));
            ThingFilterUI.DoThingFilterConfigWindow(
                new Rect(rect.width - 300f, rect.y, 300f, previewHeight),
                ref scrollPosition,
                filter);
        }

        public static void DoSettingsContents(Listing_Standard list)
        {
            list.CheckboxLabeled(
                RSACoreKeys.RSACore_IncludeParentCategory.Translate(),
                ref Settings.IncludeParentCategory,
                RSACoreKeys.RSACore_IncludeParentCategoryTip.Translate());

            const float sliderHeight = 30f;

            Rect rect = list.GetRect(sliderHeight);
            Widgets.Label(
                new Rect(rect.position, new Vector2(rect.width * 0.5f, sliderHeight)),
                RSACoreKeys.RSACore_SearchWidth.Translate());
            Settings.SearchWidth = Widgets.HorizontalSlider(
                new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.5f, sliderHeight),
                Settings.SearchWidth,
                0.25f,
                1f,
                false,
                $"{Settings.SearchWidth:p0}",
                null,
                null,
                -1f);
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard(GameFont.Small) { ColumnWidth = inRect.width };
            list.Begin(inRect);

            DoSettingsContents(list);
            DoPreview(list);

            list.End();
        }

        public override string SettingsCategory()
        {
            if (this.SupressSettings)
            {
                return string.Empty;
            }

            return Debug
                       ? RSACoreKeys.RSACore_Debug.Translate(Assembly.GetExecutingAssembly().GetName().Version)
                       : RSACoreKeys.RSACore.Translate();
        }
    }
}