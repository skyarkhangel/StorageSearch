using System;
using JetBrains.Annotations;
using Verse;

namespace RSA.Core {
    public static class ThingFilterUtil {
        /// <summary>
        /// Queue up a projection on the displayed <see cref="TreeNode_ThingCategory"/> for the next invocation of 
        /// <see cref="Listing_TreeThingFilter.DoCategoryChildren"/>.
        /// </summary>
        /// <param name="projection">projection to apply to <see cref="Listing_TreeThingFilter.DoCategoryChildren"/>'s <c>node</c> partameter before execution.</param>
        /// <remarks>This projection is 'consumed' on invocation. Any subsequent incovations will need their own queued projections if desired.</remarks>
        public static void QueueNextInvocationFilter([NotNull]Func<TreeNode_ThingCategory, TreeNode_ThingCategory> projection) {
            ThingFilter_InjectFilter.Projections.Enqueue(projection);
        }
    }
}
