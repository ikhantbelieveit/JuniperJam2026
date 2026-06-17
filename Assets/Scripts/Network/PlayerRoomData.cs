using UnityEngine;
using Mirror;
using JJ26.UI;

namespace JJ26.Network
{
    public class PlayerRoomData : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string DisplayName = "Loading...";

        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool IsReady = false;

        private bool _isLeader = false;
        public bool IsLeader
		{
			get
			{
				return _isLeader;
			}
            set
			{
                _isLeader = value;
                (FindAnyObjectByType(typeof(LobbyUIController)) as LobbyUIController).OnPlayerLeaderStatusSet();
            }
		}

		private GameNetworkManager _room => NetworkManager.singleton as GameNetworkManager;

		public void HandleReadyStatusChanged(bool oldValue, bool newValue) => RefreshLobbyDisplay();
		public void HandleDisplayNameChanged(string oldValue, string newValue) => RefreshLobbyDisplay();

		public void UpdateGameReady(bool isReady)
		{
			if(!_isLeader) { return; }

			(FindAnyObjectByType(typeof(LobbyUIController)) as LobbyUIController).StartGameButton.gameObject.SetActive(isReady);
		}

		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			MainMenuUISystem mainMenuUISystem = (FindAnyObjectByType(typeof(MainMenuUISystem)) as MainMenuUISystem);
			string name = (mainMenuUISystem.Controller as MainMenuUIController).DisplayName;
			CmdSetDisplayName(name);
		}

		public override void OnStartClient()
		{
			base.OnStartClient();

			_room.RoomPlayers.Add(this);
		}

		public void OnDestroy()
		{
			if(!_room) { return; }

			_room.RoomPlayers.Remove(this);
			RefreshLobbyDisplay();
		}

		public void RefreshLobbyDisplay()
		{
			if(!authority)
			{
				foreach(var player in _room.RoomPlayers)
				{
					if(player.authority)
					{
						player.RefreshLobbyDisplay();
						break;
					}
				}
			}
			(FindAnyObjectByType(typeof(LobbyUIController)) as LobbyUIController).RefreshDisplay();
		}

		[Command]
		private void CmdSetDisplayName(string displayName)
		{
			DisplayName = displayName;
		}

		[Command]
		public void CmdSetReady(bool isReady)
		{
			Debug.Log("TOGGLE READY");
			IsReady = isReady;

			_room.UpdatePlayersOfReadyStatus();
		}

		[Command]
		public void CmdStartGame()
		{
			if(_room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }
			Debug.Log("START GAME");
		}
	}
}
