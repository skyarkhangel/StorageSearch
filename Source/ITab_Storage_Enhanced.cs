using System;
using System.Collections;
using System.Collections.Generic;
using StorageSearch;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class ITab_Storage_Enhanced : ITab
    {
        private const float TopAreaHeight = 35f;

        private Vector2 scrollPosition = default(Vector2);

        private static readonly Vector2 WinSize = new Vector2(300f, 480f);

        private IStoreSettingsParent SelStoreSettingsParent
        {
            get
            {
                return (IStoreSettingsParent)base.SelObject;
            }
        }

        public override bool IsVisible
        {
            get
            {
                return this.SelStoreSettingsParent.StorageTabVisible;
            }
        }

        //StorageSearch
        private string searchText = "";

        private bool isFocused;

        //


        public ITab_Storage_Enhanced()
        {
            this.size = ITab_Storage_Enhanced.WinSize;
            this.labelKey = "TabStorage";
            this.tutorHighlightTag = "TabStorage";
        }

        protected override void FillTab()
        {
            ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.GuiFrame);
            ConceptDecider.TeachOpportunity(ConceptDefOf.StorageTabCategories, OpportunityType.GuiFrame);
            ConceptDecider.TeachOpportunity(ConceptDefOf.StoragePriority, OpportunityType.GuiFrame);
            IStoreSettingsParent selStoreSettingsParent = this.SelStoreSettingsParent;
            StorageSettings settings = selStoreSettingsParent.GetStoreSettings();
            Rect position = new Rect(0f, 0f, ITab_Storage_Enhanced.WinSize.x, ITab_Storage_Enhanced.WinSize.y).ContractedBy(10f);
            GUI.BeginGroup(position);
            Text.Font = GameFont.Small;
            Rect rect = new Rect(0f, 0f, 160f, 29f);
            if (Widgets.ButtonText(rect, "Priority".Translate() + ": " + settings.Priority.Label(), true, false, true))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                IEnumerator enumerator = Enum.GetValues(typeof(StoragePriority)).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        StoragePriority storagePriority = (StoragePriority)((byte)enumerator.Current);
                        if (storagePriority != StoragePriority.Unstored)
                        {
                            StoragePriority localPr = storagePriority;
                            list.Add(new FloatMenuOption(localPr.Label().CapitalizeFirst(), delegate
                            {
                                settings.Priority = localPr;
                                ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.StoragePriority, KnowledgeAmount.Total);
                            }, MenuOptionPriority.Medium, null, null, 0f, null));
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }

            #region StorageSearch
            var clearSearchRect = new Rect(position.width - 33f, (29f - 14f) / 2f, 14f, 14f);
            var shouldClearSearch = (Widgets.ButtonImage(clearSearchRect, Widgets.CheckboxOffTex));

            var searchRect = new Rect(165f, 0f, position.width - 160f - 20f, 29f);
            var watermark = (searchText != string.Empty || isFocused) ? searchText : "Search";

            var escPressed = (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape);
            var clickedOutside = (!Mouse.IsOver(searchRect) && Event.current.type == EventType.MouseDown);

            if (!isFocused)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.6f);
            }

            GUI.SetNextControlName("StorageSearchInput");
            var searchInput = Widgets.TextField(searchRect, watermark);
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
            #endregion


            TutorUIHighlighter.HighlightOpportunity("StoragePriority", rect);
            ThingFilter parentFilter = null;
            if (selStoreSettingsParent.GetParentStoreSettings() != null)
            {
                parentFilter = selStoreSettingsParent.GetParentStoreSettings().filter;
            }
            Rect rect2 = new Rect(0f, 35f, position.width, position.height - 35f);
            HelperThingFilterUI.DoThingFilterConfigWindow(rect2, ref this.scrollPosition, settings.filter, parentFilter, 8, searchText);
            GUI.EndGroup();
        }
    }
}
