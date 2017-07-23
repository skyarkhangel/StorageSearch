using System;
using System.ComponentModel;
using UnityEngine;
using Verse;

namespace ImprovedFilter {
    public class Settings : ModSettings {

        public static bool IncludeParentCategory = true;

        public override void ExposeData() {
            base.ExposeData();

            Scribe_Values.Look(ref IncludeParentCategory, nameof(IncludeParentCategory), false);
        }
    }
}
