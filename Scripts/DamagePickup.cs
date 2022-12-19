using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using ModsPlus;

namespace PickupsPlus
{
    public class DamagePickup : Pickup
    {
        public EffectCounter counter;

        private static float duration = 5f;
        private static StatChanges speedBuff = new StatChanges
        {
            Damage = 2f
        };

        protected override void OnPickup(Player player)
        {
            var buff = StatManager.Apply(player, speedBuff);
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
