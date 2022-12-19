using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jotunn.Utils;
using UnityEngine;

namespace PickupsPlus
{
    public class Assets
    {
        private static AssetBundle Bundle = AssetUtils.LoadAssetBundleFromResources("pickupsplus", typeof(PickupsPlusPlugin).Assembly);
        internal static GameObject[] Pickups = new GameObject[]
        {
            Bundle.LoadAsset<GameObject>("Speed Pickup"),
            Bundle.LoadAsset<GameObject>("Damage Pickup"),
            Bundle.LoadAsset<GameObject>("Health Pickup"),
            Bundle.LoadAsset<GameObject>("Shield Pickup"),
            Bundle.LoadAsset<GameObject>("Ammo Pickup"),
        };
        internal static GameObject ActiveEffectsBar = Bundle.LoadAsset<GameObject>("Active Effects Bar");
    }
}
