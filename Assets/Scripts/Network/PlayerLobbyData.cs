using JJ26.UI;
using Mirror;

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

			GameNetworkManager.Instance.LobbyPlayers.Add(this);
		}

		public void OnDestroy()
		{
			GameNetworkManager.Instance?.LobbyPlayers.Remove(this);
			RefreshLobbyDisplay();
		}

		public void RefreshLobbyDisplay()
		{
			if(!authority)
			{
				foreach(var player in GameNetworkManager.Instance.LobbyPlayers)
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
			GameNetworkManager.Instance.UpdatePlayersOfReadyStatus();
		}

		[Command]
		public void CmdStartGame()
		{
			if(GameNetworkManager.Instance.LobbyPlayers[0].connectionToClient != connectionToClient) { return; }
			GameNetworkManager.Instance.StartGame();
		}

		[Command]
		public void CmdForceStartGame()
		{
			GameNetworkManager.Instance.StartGame(true);
		}
	}
}
