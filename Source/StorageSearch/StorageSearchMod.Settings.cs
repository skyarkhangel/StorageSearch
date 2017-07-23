using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace StorageSearch {
    public class Settings : ImprovedFilter.Settings {

        public static bool EnableOutfitFilter = true;
        public static bool EnableCraftingFilter = true;

        public override void ExposeData() {
            base.ExposeData();

            Scribe_Values.Look(ref EnableOutfitFilter, nameof(EnableOutfitFilter), true);
            Scribe_Values.Look(ref EnableCraftingFilter, nameof(EnableCraftingFilter), true);
        }
    }

}
