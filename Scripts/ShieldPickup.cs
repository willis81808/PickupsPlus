using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using ModsPlus;

namespace PickupsPlus
{
    public class ShieldPickup : Pickup
    {
        protected override void OnPickup(Player player)
        {
            var block = player.data.block;
            block.counter = block.cooldown;
        }
    }
}
