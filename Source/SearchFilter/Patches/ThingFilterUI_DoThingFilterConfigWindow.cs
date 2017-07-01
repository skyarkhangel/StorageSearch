using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Harmony;
using UnityEngine;
using Verse;

namespace SearchFilter {

    [HarmonyPatch(typeof(ThingFilterUI), nameof(ThingFilterUI.DoThingFilterConfigWindow))]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Harmony patch class")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony patch class")]
    class ThingFilterUI_DoThingFilterConfigWindow
    {

        private const float SearchDefaultHeight = 29f;
        private const float SearchClearTopFactor = 15f/58f;
        private const float SeachClearHeightFactor = 14f/29f;

        private const float OverheadControlsHeight = 35f;

        internal static volatile int showSearchCount;

        internal static Queue<SearchOptions> searchOptions = new Queue<SearchOptions>();

        [HarmonyPrefix]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Harmony patch method")]
        public static void Before_DoThingFilterConfigWindow(ref object __state, ref Rect rect) {
            bool showSearch = showSearchCount-- > 0 && searchOptions.Count != 0;
            showSearchCount = Math.Max(0, showSearchCount);

            if (showSearch)
            {
                var options = searchOptions.Dequeue();
                DoSearchBlock(
                    new Rect(rect.x, rect.y - OverheadControlsHeight, rect.width, rect.height + OverheadControlsHeight), 
                    options.Term, 
                    options.Offset, 
                    options.Resize, 
                    options.Watermark);
                Listing_TreeThingFilter_DoCategoryChildren.Projections.Enqueue(options.Term.FilterNodes);
            }           
        }

        private static void DoSearchBlock(Rect position, SearchTerm term, Vector2? offset = null, Vector2? resize = null, string defaultWatermark = null) {
            if (offset == null)
                offset = Vector2.zero;

            if (resize == null)
                resize = Vector2.zero;

            float height = SearchDefaultHeight + resize.Value.y;
            float clearSize = height*SeachClearHeightFactor;

            Rect clearSearchRect = new Rect(position.xMax - 33f + offset.Value.x, position.y + offset.Value.y + height * SearchClearTopFactor, clearSize, clearSize);
            bool shouldClearSearch = Widgets.ButtonImage(clearSearchRect, Widgets.CheckboxOffTex);

            Rect searchRect = new Rect(position.x + 165f + offset.Value.x, position.y + offset.Value.y, position.width - 160f - 20f + resize.Value.x, SearchDefaultHeight + resize.Value.y);
            string watermark = (term.Value != string.Empty || term.Focused) ? term.Value :  (defaultWatermark ?? GeneratedDefs.SearchFilterKeys.SearchFilter_SearchWatermark.Translate()) ;

            bool escPressed = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
            bool clickedOutside = !Mouse.IsOver(searchRect) && Event.current.type == EventType.MouseDown;

            if (!term.Focused) {
                GUI.color = new Color(1f, 1f, 1f, 0.6f);
            }

            GUI.SetNextControlName(term.ControlName);
            string searchInput = Widgets.TextField(searchRect, watermark);
            GUI.color = Color.white;

            if (term.Focused) {
                term.Value = searchInput;
            }

            if ((GUI.GetNameOfFocusedControl() == term.ControlName || term.Focused) && (escPressed || clickedOutside)) {
                GUIUtility.keyboardControl = 0;
                term.Focused = false;
            } else if (GUI.GetNameOfFocusedControl() == term.ControlName && !term.Focused) {
                term.Focused = true;
            }

            if (shouldClearSearch) {
                term.Value = string.Empty;
            }
        }
    }
}
