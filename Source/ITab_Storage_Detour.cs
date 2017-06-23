using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HaulingHysteresis;
using StorageSearch;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class ITab_Storage_Detour
    {

        private static PropertyInfo SelStoreSettingsParent;

        private static FieldInfo ScrollPosition;

        private static readonly Vector2 WinSize = new Vector2(300f, 480f);

        public static void Init()
        {
            ITab_Storage_Detour.SelStoreSettingsParent = typeof(ITab_Storage).GetProperty("SelStoreSettingsParent", BindingFlags.Instance | BindingFlags.NonPublic);
            ITab_Storage_Detour.ScrollPosition = typeof(ITab_Storage).GetField("scrollPosition", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private const float TopAreaHeight = 35f;


        // private static readonly Vector2 WinSize = new Vector2(300f, 480f);

        // private IStoreSettingsParent SelStoreSettingsParent
        // {
        // get
        // {
        // return (IStoreSettingsParent)SelObject;
        // }
        // }

        // public override bool IsVisible
        // {
        // get
        // {
        // return SelStoreSettingsParent.StorageTabVisible;
        // }
        // }

        // StorageSearch
        private static string searchText = string.Empty;

        private static bool isFocused;



        public static void FillTab(ITab_Storage tab)
        {
            IStoreSettingsParent storeSettingsParent = (IStoreSettingsParent)ITab_Storage_Detour.SelStoreSettingsParent.GetValue(tab, null);
            StorageSettings settings = storeSettingsParent.GetStoreSettings();
            Rect position = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            GUI.BeginGroup(position);
            Text.Font = GameFont.Small;
            Rect rect = new Rect(0f, 0f, 160f, 29f);
            if (Widgets.ButtonText(rect, "Priority".Translate() + ": " + settings.Priority.Label(), true, false, true))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (object current in Enum.GetValues(typeof(StoragePriority)))
                {
                    bool flag2 = (StoragePriority)current > StoragePriority.Unstored;
                    if (flag2)
                    {
                        StoragePriority localPr = (StoragePriority)current;
                        list.Add(
                            new FloatMenuOption(
                                localPr.Label().CapitalizeFirst(),
                                delegate { settings.Priority = localPr; },
                                MenuOptionPriority.Default,
                                null,
                                null,
                                0f,
                                null));
                    }
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }

            Rect clearSearchRect = new Rect(position.width - 33f, (29f - 14f) / 2f, 14f, 14f);
            bool shouldClearSearch = Widgets.ButtonImage(clearSearchRect, Widgets.CheckboxOffTex);

            Rect searchRect = new Rect(165f, 0f, position.width - 160f - 20f, 29f);
            string watermark = (searchText != string.Empty || isFocused) ? searchText : "Search";

            bool escPressed = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
            bool clickedOutside = !Mouse.IsOver(searchRect) && Event.current.type == EventType.MouseDown;

            if (!isFocused)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.6f);
            }

            GUI.SetNextControlName("StorageSearchInput");
            string searchInput = Widgets.TextField(searchRect, watermark);
            GUI.color = Color.white;

            if (isFocused)
            {
                searchText = searchInput;
            }

            if ((GUI.GetNameOfFocusedControl() == "StorageSearchInput" || isFocused) && (escPressed || clickedOutside))
            {
                GUIUtility.keyboardControl = 0;
                isFocused = false;
            }
            else if (GUI.GetNameOfFocusedControl() == "StorageSearchInput" && !isFocused)
            {
                isFocused = true;
            }

            if (shouldClearSearch)
            {
                searchText = string.Empty;
            }



            UIHighlighter.HighlightOpportunity(rect, "StoragePriority");
            ThingFilter parentFilter = null;
            if (storeSettingsParent.GetParentStoreSettings() != null)
            {
                parentFilter = storeSettingsParent.GetParentStoreSettings().filter;
            }

            Rect rect2 = new Rect(0f, 35f, position.width, position.height - 70f);

            Vector2 vector = (Vector2)ITab_Storage_Detour.ScrollPosition.GetValue(tab);
            HelperThingFilterUI.DoThingFilterConfigWindow(rect2, ref vector, settings.filter, parentFilter, 8, null, null, null, searchText);
            ITab_Storage_Detour.ScrollPosition.SetValue(tab, vector);

            // from Hauling Hysterisis
            Rect rect3 = new Rect(0f, position.height - 30f, position.width, 30f);
            StorageSettings_Hysteresis storageSettings_Hysteresis = StorageSettings_Mapping.Get(settings);
            bool flag4 = storageSettings_Hysteresis == null;
            if (flag4)
            {
                storageSettings_Hysteresis = new StorageSettings_Hysteresis();
            }

            storageSettings_Hysteresis.FillPercent = Widgets.HorizontalSlider(rect3.LeftPart(0.8f), storageSettings_Hysteresis.FillPercent, 0f, 100f, false, "Refill cells less then");
            Widgets.Label(rect3.RightPart(0.2f), storageSettings_Hysteresis.FillPercent.ToString("N0") + "%");
            StorageSettings_Mapping.Set(settings, storageSettings_Hysteresis);
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.FrameDisplayed);


            GUI.EndGroup();
        }
    }
}
