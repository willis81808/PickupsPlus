using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System;
using BepInEx.Configuration;

namespace PickupsPlus
{
    public static class PickupsConfiguration
    {
        private static List<PickupData> pickupData = new List<PickupData>();

        public static PickupData AddPickup(GameObject prefab)
        {
            PickupData data = GetPickupData(prefab);
            if (data == null)
            {
                data = new PickupData(prefab);
                pickupData.Add(data);
            }
            return data;
        }

        public static PickupData[] GetAllPickups()
        {
            return pickupData.ToArray();
        }

        public static PickupData GetPickupData(GameObject prefab)
        {
            return pickupData.Where(p => p.Prefab == prefab).FirstOrDefault();
        }

        public static void SetPickupWeight(GameObject prefab, float weight)
        {
            if (GetPickupData(prefab) is PickupData data)
            {
                data.Weight = weight;
            }
        }

        public static void SetPickupEnabled(GameObject prefab, bool enabled)
        {
            if (GetPickupData(prefab) is PickupData data)
            {
                data.Enabled = enabled;
            }
        }

        public static GameObject GetRandomPickup()
        {
            var rand = UnityEngine.Random.Range(0f, 1f);
            var weightedSum = pickupData.Sum(d => d.Weight);
            
            var selected = pickupData.Where(pd => pd.Enabled).RandomElementByWeight(e => e.Weight);

            if (selected == null)
            {
                return null;
            }
            else
            {
                return selected.Prefab;
            }
        }

        private static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            if (sequence.Count() == 0) return default(T);

            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = UnityEngine.Random.value * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex > itemWeightIndex)
                    return item.Value;
            }

            return default(T);
        }
    }

    public class PickupData
    {
        public bool Enabled
        {
            get
            {
                return enabledConfig.Value;
            }
            set
            {
                enabledConfig.Value = value;
            }
        }
        public float Weight
        {
            get
            {
                return weightConfig.Value;
            }
            set
            {
                weightConfig.Value = value;
            }
        }
        public GameObject Prefab { get; set; }

        private ConfigEntry<bool> enabledConfig;
        private ConfigEntry<float> weightConfig;

        public PickupData(GameObject prefab)
        {
            this.Prefab = prefab;

            enabledConfig = PickupsPlusPlugin.Instance.Config.Bind(PickupsPlusPlugin.CompatabilityModName, $"{prefab.name}_enabled", true, $"Is {prefab.name} enabled?");
            weightConfig = PickupsPlusPlugin.Instance.Config.Bind(PickupsPlusPlugin.CompatabilityModName, $"{prefab.name}_weight", 1f, $"{prefab.name} random spawn weight");
        }
    }
}
