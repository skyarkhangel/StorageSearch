using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace StorageSearch {

    [HarmonyPatch(typeof(ThingFilterUI), nameof(ThingFilterUI.DoThingFilterConfigWindow))]
    public class ThingFilterUI_DoThingFilterConfigWindow
    {

        private const string ControlNameStorageSearch = @"StorageSearchInput";
        private const float HysteresisHeight = 30f;
        private const float HysteresisBlockHeight = 35f;
        private const float OverheadControlsHeight = 35f;


        private static string searchText = string.Empty;
        private static bool searchFocused;

        public static volatile int showSearchCount;
        public static volatile int showHysteresisCount;

        private static Queue<StorageSettings> _settingsQueue = new Queue<StorageSettings>();

        public static Queue<StorageSettings> SettingsQueue => _settingsQueue;


        [HarmonyPrefix]
        public static void Before_DoThingFilterConfigWindow(ref object __state, ref Rect rect) {
            bool showSearch = showSearchCount-- > 0;
            showSearchCount = Math.Max(0, showSearchCount);

            bool showHysteresis = (showHysteresisCount-- > 0) && _settingsQueue.Count != 0;
            showHysteresisCount = Math.Max(0, showHysteresisCount);

            if (showSearch) {
                DoSearchBlock(new Rect(rect.x, rect.y - OverheadControlsHeight, rect.width, rect.height + OverheadControlsHeight));
                Listing_TreeThingFilter_DoCategoryChildren.Projections.Enqueue(FilterNodes);
            }

            if (showHysteresis)
            {
                // original layout rect will get modified in called code, so keep a copy for postfix work
                __state = new Rect(rect);  
                rect= new Rect(rect.x, rect.y, rect.width, rect.height - HysteresisBlockHeight);            
            }
        }

        [HarmonyPostfix]
        public static void After_DoThingFilterConfigWindow(object __state, Rect rect) {
            if (__state == null)
                return;

            Rect original = (Rect) __state;
           
            DoHysteresisBlock(new Rect(0f, original.yMax - HysteresisHeight, original.width, HysteresisHeight), _settingsQueue.Dequeue());          
        }


        private static TreeNode_ThingCategory FilterNodes(TreeNode_ThingCategory node) {
            if (!string.IsNullOrEmpty(searchText)) {
                TreeNode_ThingCategory rootNode = new TreeNode_ThingCategory(new ThingCategoryDef());

                foreach (ThingDef currentThing in node.catDef.DescendantThingDefs.Where(td => td.label.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) != -1))
                {

                    rootNode.catDef.childThingDefs.Add(currentThing);

                    if (Settings.IncludeParentCategory)
                    {
                        if (!rootNode.catDef.childCategories.Contains(currentThing.FirstThingCategory))
                        {
                            rootNode.catDef.childCategories.Add(currentThing.FirstThingCategory);
                        }
                    }
                }

                node = rootNode;
            }
            return node;
        }


        private static void DoHysteresisBlock(Rect rect, StorageSettings settings) {

            StorageSettings_Hysteresis storageSettings_Hysteresis = StorageSettings_Mapping.Get(settings) ?? new StorageSettings_Hysteresis();

            storageSettings_Hysteresis.FillPercent = Widgets.HorizontalSlider(rect.LeftPart(0.8f), storageSettings_Hysteresis.FillPercent, 0f, 100f, false, GeneratedDefs.Keys.HaulingHysteresis_RefillCellsLabel.Translate());
            Widgets.Label(rect.RightPart(0.2f), storageSettings_Hysteresis.FillPercent.ToString("N0") + "%");

            StorageSettings_Mapping.Set(settings, storageSettings_Hysteresis);
        }

        private static void DoSearchBlock(Rect position) {
            Rect clearSearchRect = new Rect(position.xMax - 33f, position.y + (29f - 14f) / 2f, 14f, 14f);
            bool shouldClearSearch = Widgets.ButtonImage(clearSearchRect, Widgets.CheckboxOffTex);

            Rect searchRect = new Rect(position.x + 165f, position.y, position.width - 160f - 20f, 29f);
            string watermark = (searchText != string.Empty || searchFocused) ? searchText : GeneratedDefs.Keys.StorageSearch_SearchWatermark.Translate();

            bool escPressed = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
            bool clickedOutside = !Mouse.IsOver(searchRect) && Event.current.type == EventType.MouseDown;

            if (!searchFocused) {
                GUI.color = new Color(1f, 1f, 1f, 0.6f);
            }

            GUI.SetNextControlName(ControlNameStorageSearch);
            string searchInput = Widgets.TextField(searchRect, watermark);
            GUI.color = Color.white;

            if (searchFocused) {
                searchText = searchInput;
            }

            if ((GUI.GetNameOfFocusedControl() == ControlNameStorageSearch || searchFocused) && (escPressed || clickedOutside)) {
                GUIUtility.keyboardControl = 0;
                searchFocused = false;
            } else if (GUI.GetNameOfFocusedControl() == ControlNameStorageSearch && !searchFocused) {
                searchFocused = true;
            }

            if (shouldClearSearch) {
                searchText = string.Empty;
            }
        }
    }
}
