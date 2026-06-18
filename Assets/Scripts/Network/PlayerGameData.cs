using UnityEngine;
using Mirror;
using JJ26.UI;

namespace JJ26.Network
{
    public class PlayerGameData : NetworkBehaviour
    {
        [SyncVar]
        private string _displayName = "Loading...";

		private GameNetworkManager _networkManager => NetworkManager.singleton as GameNetworkManager;

		public override void OnStartClient()
		{
			base.OnStartClient();

			DontDestroyOnLoad(gameObject);
			_networkManager.GamePlayers.Add(this);
		}

		public void OnDestroy()
		{
			if (!_networkManager) { return; }
			_networkManager.GamePlayers.Remove(this);
		}

		[Server]
		public void SetDisplayName(string name)
		{
			_displayName = name;
		}
	}
}
