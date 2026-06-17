using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;

namespace JJ26.Network
{
    public class GameNetworkManager : NetworkManager
    {
		[SerializeField] private int _minPlayers = 2;

        [Scene] [SerializeField] private string _gameScene = string.Empty;

        [SerializeField] private PlayerRoomData _playerRoomDataPrefab;

		public List<PlayerRoomData> RoomPlayers { get; } = new();

        public delegate void ClientConnectEvent();
        public event ClientConnectEvent OnClientConnected;

        public delegate void ClientDisconnectEvent();
        public event ClientDisconnectEvent OnClientDisconnected;

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
			bool isLeader = RoomPlayers.Count == 0;

			PlayerRoomData roomData = Instantiate(_playerRoomDataPrefab);
			roomData.IsLeader = isLeader;
			NetworkServer.AddPlayerForConnection(conn, roomData.gameObject);
		}

		public override void OnServerDisconnect(NetworkConnectionToClient conn)
		{
			if(conn.identity != null)
			{
				var player = conn.identity.GetComponent<PlayerRoomData>();
				RoomPlayers.Remove(player);
				UpdatePlayersOfReadyStatus();
			}

			base.OnServerDisconnect(conn);
		}

		private bool IsGameReady()
		{
			if(numPlayers < _minPlayers) { return false; }

			foreach(var player in RoomPlayers)
			{
				if(!player.IsReady) { return false; }
			}

			return true;
		}

		public void UpdatePlayersOfReadyStatus()
		{
			bool isGameReady = IsGameReady();
			foreach(PlayerRoomData roomData in RoomPlayers)
			{
				roomData.UpdateGameReady(isGameReady);
			}
		}

		public override void OnStopServer()
		{
			base.OnStopServer();
			RoomPlayers.Clear();
		}

		public PlayerRoomData GetLocalPlayerData()
		{
			PlayerRoomData foundPlayer = null;
			foreach(var player in RoomPlayers)
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
	}
}