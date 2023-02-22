using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using Photon.Pun;

namespace PickupsPlus
{
    public abstract class Pickup : MonoBehaviour
    {
        public GameObject pickupEffect, pickupBurst;

        public float touchRadius = 0.5f;

        private PhotonView view;
        private bool ready = true;

        private void Awake()
        {
            view = GetComponent<PhotonView>();
        }

        protected virtual void FixedUpdate()
        {
            if (!ready) return;

            foreach (var collider in Physics2D.OverlapCircleAll(transform.position, touchRadius))
            {
                if (!collider.CompareTag("Player")) continue;

                var player = collider.GetComponentInChildren<Player>();
                view.RPC(nameof(RPC_OnPickup), RpcTarget.All, player.playerID);
                ready = false;
                break;
            }
        }
        

        [PunRPC]
        public void RPC_OnPickup(int playerId)
        {
            if (!ready) return;
            ready = false;

            var player = PlayerManager.instance.players.Where(p => p.playerID == playerId).First();

            OnPickup(player);
            SpawnActivateEffect(player.transform.position);
            SpawnPickupAura();

            PhotonNetwork.Destroy(gameObject);
        }

        private void SpawnActivateEffect(Vector3 spawnPoint)
        {
            Instantiate(pickupBurst, spawnPoint, pickupBurst.transform.rotation);
        }

        private void SpawnPickupAura()
        {
            Vector3 spawnPoint;

            var floor = Physics2D.Raycast(transform.position, Vector2.down, touchRadius * 3, PickupsPlusPlugin.floorMask);
            if (floor.collider == null)
            {
                spawnPoint = transform.position - (Vector3.down * touchRadius);
            }
            else
            {
                spawnPoint = floor.point;
                spawnPoint.z = Camera.main.nearClipPlane;
            }

            Instantiate(pickupEffect, spawnPoint, pickupEffect.transform.rotation);
        }

        public virtual void SetupExtraMenuOptions(GameObject menu) { }

        protected abstract void OnPickup(Player player);
    }
}
