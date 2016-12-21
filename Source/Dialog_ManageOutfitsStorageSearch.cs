using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using RimWorld;
using UnityEngine;
using Verse;

namespace StorageSearch
{
    public class Dialog_ManageOutfitsStorageSearch : Window
    {
        private const float TopAreaHeight = 40f;
        private const float TopButtonHeight = 35f;
        private const float TopButtonWidth = 150f;

        private static ThingFilter _apparelGlobalFilter;

        private static readonly Regex ValidNameRegex = new Regex("^[a-zA-Z0-9 '\\-]*$");

        private Vector2 _scrollPosition;
        private Outfit _selOutfitInt;

        public Dialog_ManageOutfitsStorageSearch(Outfit selectedOutfit)
        {
            forcePause = true;
            doCloseX = true;
            closeOnEscapeKey = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            if (_apparelGlobalFilter == null)
            {
                _apparelGlobalFilter = new ThingFilter();
                _apparelGlobalFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
            }
            SelectedOutfit = selectedOutfit;
        }

        private Outfit SelectedOutfit
        {
            get { return _selOutfitInt; }
            set
            {
                CheckSelectedOutfitHasName();
                _selOutfitInt = value;
            }
        }

        public override Vector2 InitialSize
        {
            get { return new Vector2(700f, 700f); }
        }

        private void CheckSelectedOutfitHasName()
        {
            if (SelectedOutfit != null && SelectedOutfit.label.NullOrEmpty())
            {
                SelectedOutfit.label = "Unnamed";
            }
        }
        //StorageSearch
        private string searchText = "";
        private bool isFocused;

        [Detour(typeof(Dialog_ManageOutfits), bindingFlags = (BindingFlags.Instance | BindingFlags.Public))]
        public override void DoWindowContents(Rect inRect)
        {
            var num = 0f;
            var rect = new Rect(0f, 0f, 150f, 35f);
            num += 150f;
            if (Widgets.ButtonText(rect, "SelectOutfit".Translate(), true, false))
            {
                var list = new List<FloatMenuOption>();
                foreach (var current in Current.Game.outfitDatabase.AllOutfits)
                {
                    var localOut = current;
                    list.Add(new FloatMenuOption(localOut.label, delegate { SelectedOutfit = localOut; },
                        MenuOptionPriority.Default, null, null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            num += 10f;
            var rect2 = new Rect(num, 0f, 150f, 35f);
            num += 150f;
            if (Widgets.ButtonText(rect2, "NewOutfit".Translate(), true, false))
            {
                SelectedOutfit = Current.Game.outfitDatabase.MakeNewOutfit();
            }
            num += 10f;
            var rect3 = new Rect(num, 0f, 150f, 35f);
            num += 150f;
            if (Widgets.ButtonText(rect3, "DeleteOutfit".Translate(), true, false, true))
            {
                List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                foreach (Outfit current2 in Current.Game.outfitDatabase.AllOutfits)
                {
                    Outfit localOut = current2;
                    list2.Add(new FloatMenuOption(localOut.label, delegate
                    {
                        AcceptanceReport acceptanceReport = Current.Game.outfitDatabase.TryDelete(localOut);
                        if (!acceptanceReport.Accepted)
                        {
                            Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
                        }
                        else if (localOut == SelectedOutfit)
                        {
                            SelectedOutfit = null;
                        }
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(list2));
            }
            var rect4 = new Rect(0f, 40f, 300f, inRect.height - 40f - CloseButSize.y).ContractedBy(10f);
            if (SelectedOutfit == null)
            {
                GUI.color = Color.grey;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect4, "NoOutfitSelected".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
                return;
            }
            GUI.BeginGroup(rect4);
            var rect5 = new Rect(0f, 0f, 180f, 30f);
            DoNameInputRect(rect5, ref SelectedOutfit.label, 30);

            #region Storage Search

            var clearSearchRect = new Rect(rect4.width - 20f, (29f - 14f) / 2f, 14f, 14f);
            var shouldClearSearch = (Widgets.ButtonImage(clearSearchRect, Widgets.CheckboxOffTex));

            var searchRect = new Rect(rect5.width + 10f, 0f, rect4.width - rect5.width - 10f, 29f);
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
            UIHighlighter.HighlightOpportunity(rect, "StoragePriority");


            //        if (_apparelGlobalFilter != null)
            //        {
            //            parentFilter = _apparelGlobalFilter;
            //        }
            //         Rect rect5a = new Rect(0f, 35f, rect4.width, rect4.height - 35f);
            //         HelperThingFilterUI.DoThingFilterConfigWindow(rect5a, ref this._scrollPosition, SelectedOutfit.filter, parentFilter, 8, searchText);

            #endregion


            var rect6 = new Rect(0f, 40f, rect4.width, rect4.height - 45f - 10f);

            // fix for the filter

            if (_apparelGlobalFilter == null)
            {
                _apparelGlobalFilter = new ThingFilter();
                _apparelGlobalFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
            }
            var parentFilter = _apparelGlobalFilter;

            //

            HelperThingFilterUI.DoThingFilterConfigWindow(rect6, ref _scrollPosition, SelectedOutfit.filter, parentFilter, 8, null,null, searchText);

            //ThingFilterUI.DoThingFilterConfigWindow(rect6, ref _scrollPosition, SelectedOutfit.filter, _apparelGlobalFilter, 16);
            GUI.EndGroup();

            rect4 = new Rect(300f, 40f, inRect.width - 300f, inRect.height - 40f - CloseButSize.y).ContractedBy(10f);
            GUI.BeginGroup(rect4);

            rect6 = new Rect(0f, 40f, rect4.width, rect4.height - 45f - 10f);
            //     DoStatsInput(rect6, ref _scrollPositionStats, saveout.Stats);
            GUI.EndGroup();
        }

        public override void PreClose()
        {
            base.PreClose();
            CheckSelectedOutfitHasName();
        }

        private static void DoNameInputRect(Rect rect, ref string name, int maxLength)
        {
            var text = Widgets.TextField(rect, name);
            if (text.Length <= maxLength && ValidNameRegex.IsMatch(text))
            {
                name = text;
            }
        }
    }
}