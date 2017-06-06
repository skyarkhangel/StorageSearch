using System;
using System.Collections.Generic;

using RimWorld;

using Verse;

namespace HaulingHysteresis
{
    internal class StoreUtility_Detour
    {
        public static bool NoStorageBlockersIn(IntVec3 c, Map map, Thing thing)
        {
            float num = 100f;
            bool flag = c.GetSlotGroup(map) != null && c.GetSlotGroup(map).Settings != null;
            if (flag)
            {
                num = StorageSettings_Mapping.Get(c.GetSlotGroup(map).Settings).FillPercent;
            }

            List<Thing> list = map.thingGrid.ThingsListAt(c); int i = 0;
            bool result;
            while (i < list.Count)
            {
                Thing thing2 = list[i];
                bool everStoreable = thing2.def.EverStoreable;
                if (everStoreable)
                {
                    bool flag2 = thing2.def != thing.def;
                    if (flag2)
                    {
                        result = false;
                        return result;
                    }

                    bool flag3 = thing2.stackCount >= thing.def.stackLimit || (float)thing2.stackCount >= (float)thing.def.stackLimit * (num / 100f);
                    if (flag3)
                    {
                        result = false;
                        return result;
                    }
                }

                bool flag4 = thing2.def.entityDefToBuild != null && thing2.def.entityDefToBuild.passability > Traversability.Standable;
                if (flag4)
                {
                    result = false;
                }
                else
                {
                    bool flag5 = thing2.def.surfaceType == SurfaceType.None && thing2.def.passability > Traversability.Standable;
                    if (!flag5)
                    {
                        i++;
                        continue;
                    }

                    result = false;
                }

                return result;
            }

            result = true;
            return result;
        }
    }
}
