using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnboundLib;
using ModsPlus;
using UnboundLib.GameModes;
using Photon.Pun;

namespace PickupsPlus
{
    public class PickupsManager : CardEffect
    {
        private static List<GameObject> pickups = new List<GameObject>();

        private LayerMask playerMask;
        private LayerMask spawnableAreaMask;

        private void Awake()
        {
            spawnableAreaMask = LayerMask.GetMask(new string[] { "Default", "IgnorePlayer" });
            playerMask = LayerMask.GetMask(new string[] { "Player" });
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(PickupsPlusPlugin.Instance.SpawnPickups());
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            StopAllCoroutines();

            ClearPickups();
        }

        public override IEnumerator OnPointEnd(IGameModeHandler gameModeHandler)
        {
            ClearPickups();
            yield break;
        }

        private void ClearPickups()
        {
            foreach (var p in pickups)
            {
                if (p == null) continue;
                PhotonNetwork.Destroy(p);
            }
            pickups.Clear();
        }
    }
}
