namespace RSA
{
    using Verse;

    public class Settings : Core.Settings
    {
        public static bool EnableCraftingFilter = true;

        public static bool EnableOutfitFilter = true;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref EnableOutfitFilter, nameof(EnableOutfitFilter), true);
            Scribe_Values.Look(ref EnableCraftingFilter, nameof(EnableCraftingFilter), true);
        }
    }
}