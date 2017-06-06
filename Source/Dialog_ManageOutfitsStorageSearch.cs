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
            get
            {
                return _selOutfitInt;
            }

            set
            {
                CheckSelectedOutfitHasName();
                _selOutfitInt = value;
            }
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(700f, 700f);
            }
        }

        private void CheckSelectedOutfitHasName()
        {
            if (SelectedOutfit != null && SelectedOutfit.label.NullOrEmpty())
            {
                SelectedOutfit.label = "Unnamed";
            }
        }

        // StorageSearch
        private string searchText = string.Empty;

        private bool isFocused;

        [Detour(typeof(Dialog_ManageOutfits), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public override void DoWindowContents(Rect inRect)
        {
            float num = 0f;
            Rect rect = new Rect(0f, 0f, 150f, 35f);
            num += 150f;
            if (Widgets.ButtonText(rect, "SelectOutfit".Translate(), true, false, true))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (Outfit current in Current.Game.outfitDatabase.AllOutfits)
                {
                    Outfit localOut = current;
                    list.Add(
                        new FloatMenuOption(
                            localOut.label,
                            delegate { this.SelectedOutfit = localOut; },
                            MenuOptionPriority.Default,
                            null,
                            null,
                            0f,
                            null,
                            null));
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }

            num += 10f;
            Rect rect2 = new Rect(num, 0f, 150f, 35f);
            num += 150f;
            if (Widgets.ButtonText(rect2, "NewOutfit".Translate(), true, false, true))
            {
                this.SelectedOutfit = Current.Game.outfitDatabase.MakeNewOutfit();
            }

            num += 10f;
            Rect rect3 = new Rect(num, 0f, 150f, 35f);
            num += 150f;
            if (Widgets.ButtonText(rect3, "DeleteOutfit".Translate(), true, false, true))
            {
                List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                foreach (Outfit current2 in Current.Game.outfitDatabase.AllOutfits)
                {
                    Outfit localOut = current2;
                    list2.Add(
                        new FloatMenuOption(
                            localOut.label,
                            delegate
                                {
                                    AcceptanceReport acceptanceReport = Current.Game.outfitDatabase.TryDelete(localOut);
                                    if (!acceptanceReport.Accepted)
                                    {
                                        Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
                                    }
                                    else if (localOut == this.SelectedOutfit)
                                    {
                                        this.SelectedOutfit = null;
                                    }
                                },
                            MenuOptionPriority.Default,
                            null,
                            null,
                            0f,
                            null,
                            null));
                }

                Find.WindowStack.Add(new FloatMenu(list2));
            }

            Rect rect4 = new Rect(0f, 40f, inRect.width, inRect.height - 40f - this.CloseButSize.y).ContractedBy(10f);
            if (this.SelectedOutfit == null)
            {
                GUI.color = Color.grey;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect4, "NoOutfitSelected".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
                return;
            }

            GUI.BeginGroup(rect4);
            Rect rect5 = new Rect(0f, 0f, 200f, 30f);
            Dialog_ManageOutfits.DoNameInputRect(rect5, ref this.SelectedOutfit.label);

            Rect clearSearchRect = new Rect(rect4.width - 20f, (29f - 14f) / 2f, 14f, 14f);
            bool shouldClearSearch = Widgets.ButtonImage(clearSearchRect, Widgets.CheckboxOffTex);

            Rect searchRect = new Rect(rect5.width + 10f, 0f, rect4.width - rect5.width - 10f, 29f);
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

            // if (_apparelGlobalFilter != null)
            // {
            // parentFilter = _apparelGlobalFilter;
            // }
            // Rect rect5a = new Rect(0f, 35f, rect4.width, rect4.height - 35f);
            // HelperThingFilterUI.DoThingFilterConfigWindow(rect5a, ref this._scrollPosition, SelectedOutfit.filter, parentFilter, 8, searchText);
            Rect rect6 = new Rect(0f, 40f, 300f, rect4.height - 45f - 10f);
            IEnumerable<SpecialThingFilterDef> forceHiddenFilters = this.HiddenSpecialThingFilters();

            // Storage Search
            // fix for the filter
            if (_apparelGlobalFilter == null)
            {
                _apparelGlobalFilter = new ThingFilter();
                _apparelGlobalFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
            }

            ThingFilter parentFilter = _apparelGlobalFilter;

            HelperThingFilterUI.DoThingFilterConfigWindow(
                rect6,
                ref _scrollPosition,
                this.SelectedOutfit.filter,
                parentFilter,
                16,
                null,
                forceHiddenFilters,
                null,
                searchText);

            // ThingFilterUI.DoThingFilterConfigWindow(rect6, ref _scrollPosition, this.SelectedOutfit.filter, _apparelGlobalFilter, 16, null, forceHiddenFilters, null);
            GUI.EndGroup();
        }

        private IEnumerable<SpecialThingFilterDef> HiddenSpecialThingFilters()
        {
            yield return SpecialThingFilterDefOf.AllowNonDeadmansApparel;
        }

        public override void PreClose()
        {
            base.PreClose();
            CheckSelectedOutfitHasName();
        }

        // private static void DoNameInputRect(Rect rect, ref string name, int maxLength)
        // {
        // var text = Widgets.TextField(rect, name);
        // if (text.Length <= maxLength && ValidNameRegex.IsMatch(text))
        // {
        // name = text;
        // }
        // }
    }
}