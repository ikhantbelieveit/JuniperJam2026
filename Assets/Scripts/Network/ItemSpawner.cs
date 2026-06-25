using UnityEngine;
using Mirror;
using System.Collections.Generic;
using JJ26.Gameplay;

namespace JJ26.Network
{
    public class ItemSpawner : NetworkBehaviour
    {
        [SerializeField] float _spawnDuration_Min = 5f;
        [SerializeField] float _spawnDuration_Max = 10f;

        [SerializeField] float _spawnAreaRadius = 300f;

        [SerializeField] List<CollectTreasure> _treasurePrefabs;

        [SyncVar] public float SpawnTimeRemaining;

        private Vector2 _spawnCentre = Vector2.zero;

        [SerializeField] int _initialSpawnAmount = 4;


        public override void OnStartClient()
        {
            base.OnStartClient();

            if (null == GameNetworkManager.Instance.ItemSpawner)
            {
                GameNetworkManager.Instance.SetItemSpawner(this);
            }
        }

        [Server]
        public void ResetSpawnTime()
        {
            SpawnTimeRemaining = Random.Range(_spawnDuration_Min, _spawnDuration_Max);
        }

        [Server]
        public void Update()
        {
            switch (GameNetworkManager.Instance?.GameState.CurrentState)
            {
                case EGameState.Gameplay:
                    UpdateSpawnCountdown();
                    break;
            }
        }

        [Server]
        void UpdateSpawnCountdown()
        {
            if (SpawnTimeRemaining <= 0) { return; }
            SpawnTimeRemaining -= Time.deltaTime;
            if (SpawnTimeRemaining <= 0)
            {
                SpawnNewItem();
                ResetSpawnTime();
            }
        }

        [Server]
        void SpawnNewItem()
		{
            float r = _spawnAreaRadius * Mathf.Sqrt(Random.Range(0.0f, 1.0f));
            float theta = Random.Range(0.0f, 1.0f) * 2 * Mathf.PI;

            float xPos = _spawnCentre.x + r * Mathf.Cos(theta);
            float ZPos = _spawnCentre.x + r * Mathf.Sin(theta);

            int randomItemIndex = Random.Range(0, _treasurePrefabs.Count - 1);
            CollectTreasure prefab = _treasurePrefabs[randomItemIndex];
            CollectTreasure treasure = Instantiate(prefab, new Vector3(xPos, 0.0f, ZPos), Quaternion.identity);

            NetworkServer.Spawn(treasure.gameObject);

            Debug.Log("Spawn treasure at position X " + xPos.ToString() + " Y " + ZPos.ToString());
        }

        [Server]
        public void SpawnInitialItems()
		{
            for(int i = 0; i < _initialSpawnAmount; ++i)
			{
                SpawnNewItem();
			}
		}
    }
}
