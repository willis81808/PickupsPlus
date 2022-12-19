using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using ModsPlus;

namespace PickupsPlus
{
    public class HealthPickup : Pickup
    {
        protected override void OnPickup(Player player)
        {
            UnityEngine.Debug.Log($"Health Pickup touched by: {player.name}");
            player.data.healthHandler.Heal(float.MaxValue);
        }
    }
}
