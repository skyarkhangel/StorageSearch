namespace RSA.HaulingHysterisis
{
    using Verse;

    public class StorageSettings_Hysteresis : IExposable
    {
        public float FillPercent = 100f;

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.FillPercent, "fillPercent", 100f, false);
        }
    }
}