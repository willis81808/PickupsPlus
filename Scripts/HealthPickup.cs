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
            var data = player.data;
            data.healthHandler.Heal(data.maxHealth - data.health);
        }
    }
}
