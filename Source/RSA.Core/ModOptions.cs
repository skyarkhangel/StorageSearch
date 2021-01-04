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
        /// Clear filtered <see cref="TreeNode_ThingCategory" /> nodes cache every InterfaceTicksPerUpdateFilteredNodes times.
        /// Interface is updated 60 times per second. Thus nodes will be filtered by search string (60/InterfaceTicksPerUpdateFilteredNodes) times per second.
        /// 
        /// For example, if value equals 15 then search string will be applied 4 times per second.
		/// </summary>
        public static IntOption InterfaceTicksPerUpdateFilteredNodes;

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
