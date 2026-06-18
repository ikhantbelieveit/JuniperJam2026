using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections.Generic;

namespace JJ26.Network
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        [SerializeField] GameObject _playerPrefab;

        private static List<Transform> _spawnPoints = new();
        private int _nextSpawnIndex = 0;

        public static void AddSpawnPoint(Transform transform)
		{
			_spawnPoints.Add(transform);
			_spawnPoints = _spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
		}

		public static void RemoveSpawnPoint(Transform transform)
		{
			_spawnPoints.Remove(transform);
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			GameNetworkManager.OnServerReadied += SpawnPlayer;
		}

		[ServerCallback]

		private void OnDestroy()
		{
			GameNetworkManager.OnServerReadied -= SpawnPlayer;
		}

		private void SpawnPlayer(NetworkConnectionToClient conn)
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