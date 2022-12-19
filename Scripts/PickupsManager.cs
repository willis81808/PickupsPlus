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
            StartCoroutine(SpawnPickups());
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

        private IEnumerator SpawnPickups()
        {
            while (true)
            {
                yield return null;

                pickups.RemoveAll(p => p == null);
                var spawnPoints = GenerateScreenGrid(Camera.main, 50, 1, 3, 5, 1f);
                if (spawnPoints.Length > 0)
                {
                    var point = spawnPoints.GetRandom<Vector3>();
                    var pickup = Assets.Pickups.GetRandom<GameObject>();

                    var spawned = PhotonNetwork.Instantiate(pickup.name, point, pickup.transform.rotation);
                    pickups.Add(spawned);
                }

                yield return new WaitForSeconds(5);
            }
        }

        public Vector3[] GenerateScreenGrid(Camera camera, int segments, int marginX, int marginY, int crowdingMargin, float radius)
        {
            var topLeft = camera.ViewportToWorldPoint(new Vector3(0, 1, camera.nearClipPlane));
            var bottomRight = camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane));
            var topRight = new Vector3(bottomRight.x, topLeft.y, topLeft.z);
            var bottomLeft = new Vector3(topLeft.x, bottomRight.y, bottomRight.z);

            var width = topRight.x - topLeft.x;
            var height = topLeft.y - bottomLeft.y;

            var dx = width / segments;
            var dy = height / segments;

            var results = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                if (i < marginX || i > segments - marginX) continue;
                float x = topLeft.x + (i * dx);
                for (int j = 0; j <= segments; j++)
                {
                    if (j < marginY || j > segments - marginY) continue;
                    float y = bottomLeft.y + (j * dy);

                    var pos = new Vector3(x, y, camera.nearClipPlane);
                    if (Physics2D.OverlapCircle(pos, radius) == null && Physics2D.OverlapCircle(pos, crowdingMargin, playerMask) == null)
                    {
                        if (Physics2D.Raycast(pos, Vector3.down, dy * 3, spawnableAreaMask).collider == null) continue;
                        if (pickups.Where(p => Vector3.Distance(p.transform.position, pos) <= crowdingMargin).Count() > 0) continue;
                        results.Add(pos);
                    }
                }
            }

            return results.ToArray();
        }
    }
}
