using UnityEngine;
using Mirror;

namespace JJ26.Network
{
    public class GameNetworkManager : NetworkManager
    {
        [Scene] [SerializeField] private string _gameScene = string.Empty;

        [SerializeField] private PlayerRoomData _playerRoomDataPrefab;

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
			PlayerRoomData roomData = Instantiate(_playerRoomDataPrefab);

			NetworkServer.AddPlayerForConnection(conn, roomData.gameObject);
		}
	}
}