using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Verse;

namespace RSA.Core
{
    /// <summary>
    /// Allow on-demand filtering of <see cref="TreeNode_ThingCategory" /> in <see cref="Listing_TreeThingFilter.DoCategoryChildren"/> (as of A17 
    /// <see cref="ThingFilterUI.DoThingFilterConfigWindow"/> is currently the only invoker of that method).
    /// </summary>
    [HarmonyPatch(typeof(Listing_TreeThingFilter), nameof(Listing_TreeThingFilter.DoCategoryChildren))]
    static class ThingFilter_InjectFilter {
		private static readonly Queue<Func<TreeNode_ThingCategory, TreeNode_ThingCategory>> projections = new Queue<Func<TreeNode_ThingCategory, TreeNode_ThingCategory>>();

	    internal static Queue<Func<TreeNode_ThingCategory, TreeNode_ThingCategory>> Projections => projections;

		/// <summary>
		/// Cache for filtered <see cref="TreeNode_ThingCategory" /> nodes.
		/// </summary>
		private static readonly IDictionary<TreeNode_ThingCategory, TreeNode_ThingCategory> FilteredNodesCache = new Dictionary<TreeNode_ThingCategory, TreeNode_ThingCategory>();

		/// <summary>
		/// Manually update filtered nodes by clearing cache.
		/// </summary>
		public static void ClearFilteredNodesCache()
        {
			FilteredNodesCache.Clear();
			interfaceTickCounter = 0;
		}

		private static uint interfaceTickCounter = 0; 

		[HarmonyPrefix]
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Harmony patch method")]
		public static void Before_DoCategoryChildren(ref TreeNode_ThingCategory node) {
		    if (projections.Count == 0)
		        return;

			// Note! Filter function must be obtained from queue even if it will not be applied
			var func = projections.Dequeue();

			// Clear nodes cache every InterfaceTicksPerUpdateFilteredNodes interface ticks.
			interfaceTickCounter++;
			if (interfaceTickCounter >= ModOptions.InterfaceTicksPerUpdateFilteredNodes.value)
			{
				interfaceTickCounter = 0;
				FilteredNodesCache.Clear();
			}


			if (FilteredNodesCache.ContainsKey(node))
            {
				// Get node from cache
				node = FilteredNodesCache[node];
			}
			else
            {
				// Apply filter function and update cache
				var filteredNode = func(node);
				FilteredNodesCache[node] = filteredNode;
				node = filteredNode;
			}
		}
	}
}
