namespace RSA.Core
{
    using System;
    using System.Linq;

    using Verse;

    public class SearchTerm
    {
        internal readonly string ControlName;

        internal bool Focused = false;

        internal string Value = string.Empty;

        private readonly string _categoryKey;

        internal SearchTerm(string categoryKey)
        {
            this._categoryKey = categoryKey;
            this.ControlName = $"SearchFilterInput_{categoryKey}";
        }

        public string CategoryKey
        {
            get
            {
                return this._categoryKey;
            }
        }

        public void Reset()
        {
            this.Value = string.Empty;
        }

        internal TreeNode_ThingCategory FilterNodes(TreeNode_ThingCategory node)
        {
            if (!string.IsNullOrEmpty(this.Value))
            {
                TreeNode_ThingCategory rootNode = new TreeNode_ThingCategory(new ThingCategoryDef());

                foreach (ThingDef currentThing in node.catDef.DescendantThingDefs.Where(
                    td => td.label.IndexOf(this.Value, StringComparison.CurrentCultureIgnoreCase) != -1))
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
    }
}