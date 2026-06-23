using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Linq;
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

		public List<PlayerLobbyData> LobbyPlayers { get; } = new();
		public List<PlayerGameData> GamePlayers { get; } = new();

		public delegate void ClientConnectEvent();
        public event ClientConnectEvent OnClientConnected;

        public delegate void ClientDisconnectEvent();
        public event ClientDisconnectEvent OnClientDisconnected;

		public delegate void ServerReadyEvent(NetworkConnectionToClient conn);
		public static event ServerReadyEvent OnServerReadied;

		public delegate void StartServerEvent();
		public event StartServerEvent OnServerStarted;

		public delegate void StopServerEvent();
		public event StopServerEvent OnServerStopped;

		public delegate void LevelEvent();
		public static event LevelEvent Broadcast_OnLevelStarted;
		public static event LevelEvent Broadcast_OnLevelExited;

		public delegate void GameDataEvent();
		public event GameDataEvent OnAllGameDataReady;

		public static string CurrentSceneName => SceneManager.GetActiveScene().name;

		public static GameNetworkManager Instance => singleton as GameNetworkManager;

		public override void OnClientConnect()
		{
			base.OnClientConnect();

            OnClientConnected?.Invoke();
		}

		public override void OnClientDisconnect()
		{
			base.OnClientDisconnect();

			GamePlayers.Clear();
			LobbyPlayers.Clear();

			UIStateSystem.EnterScreen(UIStateSystem.EUIState.PressStart);
			SceneManager.LoadScene("GameScene");

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

		public override void OnStartServer()
		{
			base.OnStartServer();
			OnServerStarted?.Invoke();
		}

		public override void OnStopServer()
		{
			base.OnStopServer();
			OnServerStopped?.Invoke();
			Broadcast_OnLevelExited?.Invoke();
			LobbyPlayers.Clear();
			GamePlayers.Clear();
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
				var lobbyPlayer = conn.identity.GetComponent<PlayerLobbyData>();
				if(null != lobbyPlayer)
				{
					LobbyPlayers.Remove(lobbyPlayer);
					UpdatePlayersOfReadyStatus();
				}

				var gamePlayer = conn.identity.GetComponent<PlayerGameData>();
				if(null != gamePlayer)
				{
					GamePlayers.Remove(gamePlayer);
				}
			}

			base.OnServerDisconnect(conn);
		}

		public bool IsGameReady()
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

		public override void OnStopClient()
		{
			base.OnStopClient();
			LobbyPlayers.Clear();
			GamePlayers.Clear();
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

		public PlayerGameData GetLocalGameData()
		{
			PlayerGameData foundPlayer = null;
			foreach (var player in GamePlayers)
			{
				if (player.isLocalPlayer) { foundPlayer = player; }
			}
			return foundPlayer;
		}

		public bool IsHostingInLobby()
		{
			var player = GetLocalPlayerData();
			return null == player ? false : player.IsLeader;
		}

		public bool IsHostingInGame()
		{
			var player = GetLocalGameData();
			return null == player ? false : player.IsLeader;
		}

		public void StartGame(bool forceStart = false)
		{
			if(SceneManager.GetActiveScene().path == _gameScene)
			{
				if(!IsGameReady() && !forceStart) { return; }

				UIStateSystem.EnterScreen(UIStateSystem.EUIState.Gameplay);
				ServerChangeScene("Level_Map_01");
			}
		}

		[Server]
		public void ExitGame()
		{
			StopHost();
			
		}

		public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
		{
			base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
			if(IsGameScene(newSceneName))
			{
				UIStateSystem.EnterScreen(UIStateSystem.EUIState.Gameplay);
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
					gamePlayerInstance.SetIsLeader(player.IsLeader);

					NetworkServer.Destroy(connection.identity.gameObject);
					NetworkServer.ReplacePlayerForConnection(connection, gamePlayerInstance.gameObject, ReplacePlayerOptions.KeepAuthority);
				}
			}

			base.ServerChangeScene(newSceneName);
		}

		public override void OnServerSceneChanged(string sceneName)
		{
			base.OnServerSceneChanged(sceneName);

			if(CurrentlyInGameScene())
			{
				Broadcast_OnLevelStarted?.Invoke();
			}
		}

		public override void OnServerReady(NetworkConnectionToClient conn)
		{
			base.OnServerReady(conn);

			OnServerReadied?.Invoke(conn);
			if(CurrentlyInGameScene())
			{
				(FindAnyObjectByType<Gameplay.PlayerSpawnSystem>()).SpawnPlayer(conn);
			}
		}

		private bool IsGameScene(string sceneName)
		{
			return sceneName.Contains("Level_Map");
		}

		private bool CurrentlyInGameScene()
		{
			return IsGameScene(CurrentSceneName);
		}

		public void OnPlayerGameDataStart(PlayerGameData gameData)
		{
			GamePlayers.Add(gameData);

			if(GamePlayers.Count >= numPlayers)
			{
				OnAllGameDataReady?.Invoke();
			}
		}
	}
}