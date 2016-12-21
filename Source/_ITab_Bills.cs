using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace StorageSearch
{
    class _ITab_Bills : ITab
    {
        protected Building_WorkTable SelTable
        {
            get
            {
                return (Building_WorkTable)SelThing;
            }
        }

        private Bill mouseoverBill;

        private static readonly Vector2 WinSize = new Vector2(370f, 480f);

        private Vector2 scrollPosition = default(Vector2);

        private float viewHeight = 1000f;

      //  [Detour(typeof(ITab_Bills), bindingFlags = (BindingFlags.Instance | BindingFlags.NonPublic))]
        // RimWorld.ITab_Bills
        protected override void FillTab()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.FrameDisplayed);
            Rect rect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            Func<List<FloatMenuOption>> recipeOptionsMaker = delegate
            {
                List<FloatMenuOption> list;
                if (SelTable.def.defName.Equals("HandTailoringBench") || SelTable.def.defName.Equals("ElectricTailoringBench"))
                {
                    list = SelTable.def.AllRecipes.OrderByDescending(x => x?.products[0]?.thingDef?.apparel?.bodyPartGroups?[0])
                    .ThenBy(x=>x?.LabelCap)
                    .Where(recipeDef => recipeDef != null && recipeDef.AvailableNow)
                    .Select(recipe => new FloatMenuOption(recipe.LabelCap, delegate
                    {
                        if (!this.SelTable.Map.mapPawns.FreeColonists.Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
                        {
                            Bill.CreateNoPawnsWithSkillDialog(recipe);
                        }
                        Bill bill = recipe.MakeNewBill();
                        SelTable.billStack.AddBill(bill);
                        if (recipe.conceptLearned != null)
                        {
                            PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
                        }
                        if (TutorSystem.TutorialMode)
                        {
                            TutorSystem.Notify_Event("AddBill-" + recipe.LabelCap);
                        }
                    }))
                    .ToList();

                }
                else
                {
                   
                list = SelTable.def.AllRecipes.Where(recipeDef => recipeDef.AvailableNow).Select(recipe => new FloatMenuOption(recipe.LabelCap, delegate
                {
                    if (!SelTable.Map.mapPawns.FreeColonists.Any(col => recipe.PawnSatisfiesSkillRequirements(col)))
                    {
                        Bill.CreateNoPawnsWithSkillDialog(recipe);
                    }
                    Bill bill = recipe.MakeNewBill();
                    SelTable.billStack.AddBill(bill);
                    if (recipe.conceptLearned != null)
                    {
                        PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
                    }
                    if (TutorSystem.TutorialMode)
                    {
                        TutorSystem.Notify_Event("AddBill-" + recipe.LabelCap);
                    }
                })).ToList();
                }

                if (!Enumerable.Any(list))
                {
                    list.Add(new FloatMenuOption("NoneBrackets".Translate(), null));
                }
                return list;
            };
            mouseoverBill = SelTable.billStack.DoListing(rect, recipeOptionsMaker, ref scrollPosition, ref viewHeight);
        }


        [Detour(typeof(ITab_Bills), bindingFlags = (BindingFlags.Instance | BindingFlags.Public))]
        public override void TabUpdate()
        {
            if (mouseoverBill != null)
            {
                mouseoverBill.TryDrawIngredientSearchRadiusOnMap(SelTable.Position);
                mouseoverBill = null;
            }
        }
    }

}

