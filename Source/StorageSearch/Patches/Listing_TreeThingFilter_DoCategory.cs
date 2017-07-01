using System;
using System.Collections.Generic;
using Harmony;
using RimWorld;
using Verse;

namespace StorageSearch
{
    /// <summary>
    /// Allow on-demand filtering of <see cref="TreeNode_ThingCategory" /> in <see cref="Listing_TreeThingFilter.DoCategoryChildren"/> (as of A17 
    /// <see cref="ThingFilterUI.DoThingFilterConfigWindow"/> is currently the only invoker of that method).
    /// </summary>
    [HarmonyPatch(typeof(Listing_TreeThingFilter), nameof(Listing_TreeThingFilter.DoCategoryChildren))]
    class Listing_TreeThingFilter_DoCategoryChildren {
		private static Queue<Func<TreeNode_ThingCategory, TreeNode_ThingCategory>> _projections = new Queue<Func<TreeNode_ThingCategory, TreeNode_ThingCategory>>();

	    public static Queue<Func<TreeNode_ThingCategory, TreeNode_ThingCategory>> Projections => _projections;

		[HarmonyPrefix]
        public static void Before_DoCategoryChildren(ref TreeNode_ThingCategory node) {
		    if (_projections.Count == 0)
		        return;

		    node = _projections.Dequeue()(node);
		}
	}
}
