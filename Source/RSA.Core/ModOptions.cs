using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RSA.Core
{
    [DefOf]
    public static class ModOptions
    {
        /// <summary>
        /// Clear nodes cache every InterfaceTicksPerClearFilteredNodesCache times.
		/// The <see cref="TreeNode_ThingCategory" /> nodes cache will be cleared every Interface ticks count.
        /// Interface is updated 60 times per second. Thus nodes will be filtered by search string (60/InterfaceTicksPerClearFilteredNodesCache) times per second.
        /// 
        /// For example, if value equals 15 then search string will be applied 4 times per second.
		/// </summary>
        public static IntOption InterfaceTicksPerClearFilteredNodesCache;

        static ModOptions()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ModOptions));
        }
    }

    public class IntOption : Def
    {
        public int value;
    }
}
