using System;
using System.Linq;
using Verse;

namespace SearchFilter {
    public class SearchTerm
    {
        private readonly string _category;

        internal string Value = string.Empty;

        internal bool Focused = false;
        internal readonly string ControlName;
        internal string Category => _category;

        internal SearchTerm(string category) {
            _category = category;
            ControlName = $"SearchFilterInput_{category}";
        }

        public void Reset() {
            Value = String.Empty;
        }

        internal TreeNode_ThingCategory FilterNodes(TreeNode_ThingCategory node) {
            if (!string.IsNullOrEmpty(Value)) {
                TreeNode_ThingCategory rootNode = new TreeNode_ThingCategory(new ThingCategoryDef());

                foreach (ThingDef currentThing in node.catDef.DescendantThingDefs.Where(td => td.label.IndexOf(Value, StringComparison.CurrentCultureIgnoreCase) != -1)) {

                    rootNode.catDef.childThingDefs.Add(currentThing);

                    if (Settings.IncludeParentCategory) {
                        if (!rootNode.catDef.childCategories.Contains(currentThing.FirstThingCategory)) {
                            rootNode.catDef.childCategories.Add(currentThing.FirstThingCategory);
                        }
                    }
                }

                node = rootNode;
            }
            return node;
        }
    }
}
