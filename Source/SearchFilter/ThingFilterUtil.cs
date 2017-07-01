using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace SearchFilter {
    public static class ThingFilterUtil {
        /// <summary>
        /// Queue up a projection on the displayed <see cref="TreeNode_ThingCategory"/> for the next invocation of 
        /// <see cref="Listing_TreeThingFilter.DoCategoryChildren"/>.
        /// </summary>
        /// <param name="projection">projection to apply to <see cref="Listing_TreeThingFilter.DoCategoryChildren"/>'s <c>node</c> partameter before execution.</param>
        /// <remarks>This projection is 'consumed' on invocation. Any subsequent incovations will need their own queued projections if desired.</remarks>
        public static void QueueNextInvocationFilter([NotNull]Func<TreeNode_ThingCategory, TreeNode_ThingCategory> projection) {
            Listing_TreeThingFilter_DoCategoryChildren.Projections.Enqueue(projection);
        }

        /// <summary>
        /// Queue up an embedding of a search input filter for the next invocation of <see cref="ThingFilterUI.DoThingFilterConfigWindow"/>.
        /// </summary>
        /// <param name="term"><see cref="SearchTerm"/> to use.</param>
        /// <param name="searchOffset">Use this parameter to move the search box from it's default position.</param>
        /// <param name="seachResize">Use this parameter to change the search boxes default size.</param>
        /// <param name="watermark">Use this parameter to change the search boxes default watermark/seach hint</param>
        /// <remarks>
        /// By default the search box is displayed at <see cref="ThingFilterUI.DoThingFilterConfigWindow"/>'s <c>rect</c> param's (<see cref="Rect.x"/>+165/<see cref="Rect.y"/>-35) 
        /// and is (<see cref="Rect.width"/>-180, 29) large.
        /// </remarks>
        public static void QueueNextInvocationSearch([NotNull] SearchTerm term, Vector2? searchOffset = null, Vector2? seachResize = null, string watermark = null) {
            ThingFilterUI_DoThingFilterConfigWindow.searchOptions.Enqueue(
                new SearchOptions
                {
                    Offset = searchOffset,
                    Resize = seachResize,
                    Term = term,
                    Watermark = watermark
                });
            ThingFilterUI_DoThingFilterConfigWindow.showSearchCount++;
        }
    }
}
