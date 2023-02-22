using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using ModsPlus;
using BepInEx.Configuration;
using UnboundLib.Utils.UI;

namespace PickupsPlus
{
    public class AmmoPickup : Pickup
    {
        public EffectCounter counter;

        private static ConfigEntry<float> duration = null;
        private StatChanges ammoBuff = new StatChanges
        {
            MaxAmmo = 5,
        };

        public override void SetupExtraMenuOptions(GameObject menu)
        {
            if (duration == null)
            {
                duration = PickupsPlusPlugin.Instance.Config.Bind(PickupsPlusPlugin.CompatabilityModName, "PickupsPlus_AmmoEffect_Duration", 5f, "Duration of effect");
            }
            duration.CreateSlider(menu, "Duration (seconds)", 1f, 15f);
        }

        protected override void OnPickup(Player player)
        {
            var buff = StatManager.Apply(player, ammoBuff);
            Unbound.Instance.ExecuteAfterSeconds(duration.Value, () =>
            {
                StatManager.Remove(buff);
            });

            // show effect counter
            if (player.data.view.IsMine)
            {
                Instantiate(counter, PickupsPlusPlugin.activeEffectsBar.transform).StartCountdown(duration.Value);
            }
        }
    }
}
