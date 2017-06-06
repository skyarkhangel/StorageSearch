using System;
using System.Collections.Generic;

using RimWorld;

namespace HaulingHysteresis
{
    internal class StorageSettings_Mapping
    {
        private static Dictionary<StorageSettings, StorageSettings_Hysteresis> mapping = new Dictionary<StorageSettings, StorageSettings_Hysteresis>();

        public static StorageSettings_Hysteresis Get(StorageSettings storage)
        {
            bool flag = StorageSettings_Mapping.mapping.ContainsKey(storage);
            StorageSettings_Hysteresis result;
            if (flag)
            {
                result = StorageSettings_Mapping.mapping[storage];
            }
            else
            {
                result = new StorageSettings_Hysteresis();
            }

            return result;
        }

        public static void Set(StorageSettings storage, StorageSettings_Hysteresis value)
        {
            bool flag = StorageSettings_Mapping.mapping.ContainsKey(storage);
            if (flag)
            {
                StorageSettings_Mapping.mapping[storage] = value;
            }
            else
            {
                StorageSettings_Mapping.mapping.Add(storage, value);
            }
        }
    }
}
