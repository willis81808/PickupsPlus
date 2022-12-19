using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using UnboundLib.Cards;
using PickupsPlus.Cards;
using Photon.Pun;
using UnboundLib.GameModes;
using UnboundLib;

namespace PickupsPlus
{

    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willis.rounds.modsplus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, ModVersion)]
    [BepInProcess("Rounds.exe")]
    public class PickupsPlusPlugin : BaseUnityPlugin
    {
        private const string ModId = "com.willis.rounds.pickupsplus";
        private const string ModName = "Pickups Plus";
        private const string ModVersion = "0.0.1";
        private const string CompatabilityModName = "PickupsPlus";

        private static List<GameObject> pickups = new List<GameObject>();

        internal static GameObject activeEffectsBar;
        internal static LayerMask playerMask;
        internal static LayerMask floorMask;

        private Coroutine spawningCoroutine = null;

        private void Awake()
        {
            floorMask = LayerMask.GetMask(new string[] { "Default", "IgnorePlayer" });
            playerMask = LayerMask.GetMask(new string[] { "Player" });
        }

        private void Start()
        {
            //CustomCard.BuildCard<PickupsTestCard>();

            foreach (var pickup in Assets.Pickups)
            {
                PhotonNetwork.PrefabPool.RegisterPrefab(pickup.name, pickup);
            }

            activeEffectsBar = Instantiate(Assets.ActiveEffectsBar, Unbound.Instance.canvas.transform);

            GameModeManager.AddHook(GameModeHooks.HookBattleStart, OnBattleStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, OnBattleEnd);
        }
        
        public IEnumerator OnBattleStart(IGameModeHandler gameModeHandler)
        {
            if (!PhotonNetwork.IsMasterClient) yield break;

            if (spawningCoroutine != null)
            {
                StopCoroutine(spawningCoroutine);
            }
            spawningCoroutine = StartCoroutine(SpawnPickups());
            yield break;
        }

        public IEnumerator OnBattleEnd(IGameModeHandler gameModeHandler)
        {
            foreach (Transform effectCounter in activeEffectsBar.transform)
            {
                Destroy(effectCounter.gameObject);
            }

            if (!PhotonNetwork.IsMasterClient) yield break;

            StopCoroutine(spawningCoroutine);
            spawningCoroutine = null;
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
                        if (Physics2D.Raycast(pos, Vector3.down, dy * 3, floorMask).collider == null) continue;
                        if (pickups.Where(p => Vector3.Distance(p.transform.position, pos) <= crowdingMargin).Count() > 0) continue;
                        results.Add(pos);
                    }
                }
            }

            return results.ToArray();
        }
    }
}
