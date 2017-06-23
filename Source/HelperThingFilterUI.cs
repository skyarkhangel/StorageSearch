using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace StorageSearch
{
    using System.Collections.Generic;

    public static class HelperThingFilterUI
    {
        private const float ExtraViewHeight = 90f;

        private const float RangeLabelTab = 10f;

        private const float RangeLabelHeight = 19f;

        private const float SliderHeight = 26f;

        private const float SliderTab = 20f;

        private static float viewHeight;



        // Verse.ThingFilterUI
        public static void DoThingFilterConfigWindow(Rect rect, ref Vector2 scrollPosition, ThingFilter filter, ThingFilter parentFilter = null, int openMask = 1, IEnumerable<ThingDef> forceHiddenDefs = null, IEnumerable<SpecialThingFilterDef> forceHiddenFilters = null, List<ThingDef> suppressSmallVolumeTags = null, string filterText = null)
        {
            Widgets.DrawMenuSection(rect, true);
            Text.Font = GameFont.Tiny;
            float num = rect.width - 2f;
            Rect rect2 = new Rect(rect.x + 1f, rect.y + 1f, num / 2f, 24f);
            if (Widgets.ButtonText(rect2, "ClearAll".Translate(), true, false, true))
            {
                filter.SetDisallowAll(forceHiddenDefs, forceHiddenFilters);
            }

            Rect rect3 = new Rect(rect2.xMax + 1f, rect2.y, rect.xMax - 1f - (rect2.xMax + 1f), 24f);
            if (Widgets.ButtonText(rect3, "AllowAll".Translate(), true, false, true))
            {
                filter.SetAllowAll(parentFilter);
            }

            Text.Font = GameFont.Small;
            rect.yMin = rect2.yMax;
            Rect viewRect = new Rect(0f, 0f, rect.width - 16f, viewHeight);
            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect, true);
            float num2 = 2f;
            DrawHitPointsFilterConfig(ref num2, viewRect.width, filter);
            DrawQualityFilterConfig(ref num2, viewRect.width, filter);
            float num3 = num2;
            Rect rect4 = new Rect(0f, num2, viewRect.width, 9999f);
            Listing_TreeThingFilter listing_TreeThingFilter = new Listing_TreeThingFilter(filter, parentFilter, forceHiddenDefs, forceHiddenFilters, suppressSmallVolumeTags);
            listing_TreeThingFilter.Begin(rect4);
            TreeNode_ThingCategory node = ThingCategoryNodeDatabase.RootNode;
            if (parentFilter != null)
            {
                node = parentFilter.DisplayRootCategory;
            }

            if (!string.IsNullOrEmpty(filterText))
            {
                TreeNode_ThingCategory rootNode = new TreeNode_ThingCategory(new ThingCategoryDef());

                node.catDef.DescendantThingDefs.Where(td => td.label.ToLower().Contains(filterText.ToLower()));

                foreach (ThingDef currentThing in node.catDef.DescendantThingDefs)
                {
                    if (currentThing.label.ToLower().Contains(filterText.ToLower()))
                    {
                        rootNode.catDef.childThingDefs.Add(currentThing);
                        if (!rootNode.catDef.childCategories.Contains(currentThing.FirstThingCategory))
                            rootNode.catDef.childCategories.Add(currentThing.FirstThingCategory);
                    }
                }

                node = rootNode;
            }

            listing_TreeThingFilter.DoCategoryChildren(node, 0, openMask, true);
            listing_TreeThingFilter.End();
            if (Event.current.type == EventType.Layout)
            {
                viewHeight = num3 + listing_TreeThingFilter.CurHeight + ExtraViewHeight;
            }

            Widgets.EndScrollView();
        }

        private static void DrawHitPointsFilterConfig(ref float y, float width, ThingFilter filter)
        {
            if (!filter.allowedHitPointsConfigurable)
            {
                return;
            }

            Rect rect = new Rect(SliderTab, y, width - SliderTab, SliderHeight);
            FloatRange allowedHitPointsPercents = filter.AllowedHitPointsPercents;
            Widgets.FloatRange(rect, 1, ref allowedHitPointsPercents, 0f, 1f, "HitPoints", ToStringStyle.PercentZero);
            filter.AllowedHitPointsPercents = allowedHitPointsPercents;
            y += SliderHeight;
            y += 5f;
            Text.Font = GameFont.Small;
        }

        private static void DrawQualityFilterConfig(ref float y, float width, ThingFilter filter)
        {
            if (!filter.allowedQualitiesConfigurable)
            {
                return;
            }

            Rect rect = new Rect(SliderTab, y, width - SliderTab, SliderHeight);
            QualityRange allowedQualityLevels = filter.AllowedQualityLevels;
            Widgets.QualityRange(rect, 2, ref allowedQualityLevels);
            filter.AllowedQualityLevels = allowedQualityLevels;
            y += SliderHeight;
            y += 5f;
            Text.Font = GameFont.Small;
        }
    }
}
