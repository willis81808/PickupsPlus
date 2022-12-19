using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using ModsPlus;

namespace PickupsPlus
{
    public class AmmoPickup : Pickup
    {
        public EffectCounter counter;

        private float duration = 5f;
        private StatChanges ammoBuff = new StatChanges
        {
            MaxAmmo = 5,
        };

        protected override void OnPickup(Player player)
        {
            var buff = StatManager.Apply(player, ammoBuff);
            Unbound.Instance.ExecuteAfterSeconds(duration, () =>
            {
                StatManager.Remove(buff);
            });

            // show effect counter
            if (player.data.view.IsMine)
            {
                Instantiate(counter, PickupsPlusPlugin.activeEffectsBar.transform).StartCountdown(duration);
            }
        }
    }
}
