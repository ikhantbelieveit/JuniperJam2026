using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Collections.Generic;
using JJ26.UI;

namespace JJ26.Network
{
    public class GameNetworkManager : NetworkManager
    {
		[SerializeField] private int _minPlayers = 2;

        [Scene] [SerializeField] private string _gameScene = string.Empty;

        [SerializeField] private PlayerLobbyData _playerLobbyDataPrefab;
		[SerializeField] private PlayerGameData _playerGameDataPrefab;
		[SerializeField] private PlayerSpawnSystem _playerSpawnSystemPrefab;

		public List<PlayerLobbyData> LobbyPlayers { get; } = new();
		public List<PlayerGameData> GamePlayers { get; } = new();

		public delegate void ClientConnectEvent();
        public event ClientConnectEvent OnClientConnected;

        public delegate void ClientDisconnectEvent();
        public event ClientDisconnectEvent OnClientDisconnected;

		public delegate void ServerReadyEvent(NetworkConnectionToClient conn);
		public static event ServerReadyEvent OnServerReadied;

		public override void OnClientConnect()
		{
			base.OnClientConnect();

            OnClientConnected?.Invoke();
		}

		public override void OnClientDisconnect()
		{
			base.OnClientDisconnect();

			OnClientDisconnected?.Invoke();
		}

		public override void OnServerConnect(NetworkConnectionToClient conn)
		{
			if(numPlayers >= maxConnections)
			{
				conn.Disconnect();
				return;
			}
			base.OnServerConnect(conn);
		}

		public override void OnServerAddPlayer(NetworkConnectionToClient conn)
		{
			bool isLeader = LobbyPlayers.Count == 0;

			PlayerLobbyData roomData = Instantiate(_playerLobbyDataPrefab);
			roomData.IsLeader = isLeader;
			NetworkServer.AddPlayerForConnection(conn, roomData.gameObject);
		}

		public override void OnServerDisconnect(NetworkConnectionToClient conn)
		{
			if(conn.identity != null)
			{
				var player = conn.identity.GetComponent<PlayerLobbyData>();
				LobbyPlayers.Remove(player);
				UpdatePlayersOfReadyStatus();
			}

			base.OnServerDisconnect(conn);
		}

		private bool IsGameReady()
		{
			if(numPlayers < _minPlayers) { return false; }

			foreach(var player in LobbyPlayers)
			{
				if(!player.IsReady) { return false; }
			}

			return true;
		}

		public void UpdatePlayersOfReadyStatus()
		{
			bool isGameReady = IsGameReady();
			foreach(PlayerLobbyData roomData in LobbyPlayers)
			{
				roomData.UpdateGameReady(isGameReady);
			}
		}

		public override void OnStopServer()
		{
			base.OnStopServer();
			LobbyPlayers.Clear();
		}

		public PlayerLobbyData GetLocalPlayerData()
		{
			PlayerLobbyData foundPlayer = null;
			foreach(var player in LobbyPlayers)
			{
				if (player.isLocalPlayer) { foundPlayer = player; }
			}
			return foundPlayer;
		}

		public bool IsHosting()
		{
			var player = GetLocalPlayerData();
			return null == player ? false : player.IsLeader;
		}

		public void StartGame()
		{
			if(SceneManager.GetActiveScene().path == _gameScene)
			{
				if(!IsGameReady()) { return; }

				ServerChangeScene("Level_Map_01");
			}
		}

		public override void ServerChangeScene(string newSceneName)
		{
			//if going from menu to game
			if(SceneManager.GetActiveScene().path == _gameScene && newSceneName.StartsWith("Level_Map"))
			{
				foreach(var player in LobbyPlayers)
				{
					var connection = player.connectionToClient;
					var gamePlayerInstance = Instantiate(_playerGameDataPrefab);
					gamePlayerInstance.SetDisplayName(player.DisplayName);

					NetworkServer.Destroy(connection.identity.gameObject);
					NetworkServer.ReplacePlayerForConnection(connection, gamePlayerInstance.gameObject, ReplacePlayerOptions.KeepAuthority);
				}
			}

			base.ServerChangeScene(newSceneName);
		}

		public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
		{
			base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);

			UIStateSystem uiSystem = FindAnyObjectByType(typeof(UIStateSystem)) as UIStateSystem;

			if(uiSystem?.CurrentState == UIStateSystem.EUIState.Lobby)
			{
				//If entering level, update UI to gameplay UI
				if (IsGameScene("Level_Map"))
				{
					UIStateSystem.EnterScreen(UIStateSystem.EUIState.Gameplay);
				}
			}
		}

		public override void OnServerSceneChanged(string sceneName)
		{
			base.OnServerSceneChanged(sceneName);

			if(IsGameScene("Level_Map"))
			{
				PlayerSpawnSystem spawnSystemInstance = Instantiate(_playerSpawnSystemPrefab);
				NetworkServer.Spawn(spawnSystemInstance.gameObject);
			}
		}

		public override void OnServerReady(NetworkConnectionToClient conn)
		{
			base.OnServerReady(conn);

			OnServerReadied?.Invoke(conn);
		}

		private bool IsGameScene(string sceneName)
		{
			return sceneName.StartsWith("Level_Map");
		}
	}
}