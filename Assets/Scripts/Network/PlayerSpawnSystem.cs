using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections.Generic;
using JJ26.Framework;
using JJ26.Network;

namespace JJ26.Gameplay
{
    public class PlayerSpawnSystem : BaseGameSystem
    {
        [SerializeField] GameObject _playerPrefab;

        private static List<Transform> _spawnPoints = new();
        private static int _nextSpawnIndex = 0;

        public static void AddSpawnPoint(Transform transform)
		{
			_spawnPoints.Add(transform);
			_spawnPoints = _spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
		}

		public override void SetCallbacks()
		{
			base.SetCallbacks();

			GameNetworkManager.Broadcast_OnLevelStarted -= OnLevelStarted;
			GameNetworkManager.Broadcast_OnLevelStarted += OnLevelStarted;

			GameNetworkManager.Broadcast_OnLevelExited -= OnLevelExited;
			GameNetworkManager.Broadcast_OnLevelExited += OnLevelExited;
		}

		public override void ResetSystem()
		{
			base.ResetSystem();

			_spawnPoints.Clear();
			_nextSpawnIndex = 0;
		}

		public void OnLevelStarted()
		{
			PlayerSpawnPoint[] spawnPoints = FindObjectsByType<PlayerSpawnPoint>();
			foreach(var spawnPoint in spawnPoints)
			{
				AddSpawnPoint(spawnPoint.transform);
			}
		}

		public void OnLevelExited()
		{
			ResetSystem();
		}

		public void SpawnPlayer(NetworkConnectionToClient conn)
		{
			Transform spawnPoint = _spawnPoints.ElementAtOrDefault(_nextSpawnIndex);

			if (null == spawnPoint)
			{
				Debug.LogError("Missing spawn point for player " + _nextSpawnIndex);
				return;
			}

			GameObject newPlayer = Instantiate(_playerPrefab, spawnPoint.position, spawnPoint.rotation);
			NetworkServer.Spawn(newPlayer, conn);
			_nextSpawnIndex++;
		}
	}
}