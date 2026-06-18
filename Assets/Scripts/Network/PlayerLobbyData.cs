using UnityEngine;
using Mirror;
using JJ26.UI;

namespace JJ26.Network
{
    public class PlayerLobbyData : NetworkBehaviour
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
            }
		}

		private GameNetworkManager _networkManager => NetworkManager.singleton as GameNetworkManager;

		public void HandleReadyStatusChanged(bool oldValue, bool newValue) => RefreshLobbyDisplay();
		public void HandleDisplayNameChanged(string oldValue, string newValue) => RefreshLobbyDisplay();

		public void UpdateGameReady(bool isReady)
		{
			if(!_isLeader) { return; }

			LobbyUISystem.SetStartButtonActive(isReady);
		}

		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			string name = MainMenuUISystem.GetDisplayName();
			CmdSetDisplayName(name);
		}

		public override void OnStartClient()
		{
			base.OnStartClient();

			_networkManager.LobbyPlayers.Add(this);
		}

		public void OnDestroy()
		{
			if(!_networkManager) { return; }

			_networkManager.LobbyPlayers.Remove(this);
			RefreshLobbyDisplay();
		}

		public void RefreshLobbyDisplay()
		{
			if(!authority)
			{
				foreach(var player in _networkManager.LobbyPlayers)
				{
					if(player.authority)
					{
						player.RefreshLobbyDisplay();
						break;
					}
				}
			}
			LobbyUISystem.RefreshDisplay();
		}

		[Command]
		private void CmdSetDisplayName(string displayName)
		{
			DisplayName = displayName;
		}

		[Command]
		public void CmdSetReady(bool isReady)
		{
			IsReady = isReady;
			_networkManager.UpdatePlayersOfReadyStatus();
		}

		[Command]
		public void CmdStartGame()
		{
			if(_networkManager.LobbyPlayers[0].connectionToClient != connectionToClient) { return; }
			_networkManager.StartGame();
		}
	}
}
