using UnityEngine;
using Mirror;
using JJ26.UI;

namespace JJ26.Network
{
    public class PlayerGameData : NetworkBehaviour
    {
        [SyncVar]
        private string _displayName = "Loading...";

		[SyncVar]
		private bool _isLeader;
		public bool IsLeader => _isLeader;

		public override void OnStartClient()
		{
			base.OnStartClient();

			DontDestroyOnLoad(gameObject);
			GameNetworkManager.Instance.GamePlayers.Add(this);
		}

		public void OnDestroy()
		{
			GameNetworkManager.Instance.GamePlayers.Remove(this);
		}

		[Server]
		public void SetDisplayName(string name)
		{
			_displayName = name;
		}

		[Server]
		public void SetIsLeader(bool isLeader)
		{
			_isLeader = isLeader;
		}

		[Command]
		public void CmdExitGame()
		{
			GameNetworkManager.Instance.ExitGame();
		}
	}
}
