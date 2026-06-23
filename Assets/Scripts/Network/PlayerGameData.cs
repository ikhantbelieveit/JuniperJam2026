using UnityEngine;
using Mirror;
using JJ26.UI;

namespace JJ26.Network
{
    public class PlayerGameData : NetworkBehaviour
    {
        [SyncVar]
        private string _displayName = "Loading...";
		public string DisplayName => _displayName;

		[SyncVar]
		private bool _isLeader;
		public bool IsLeader => _isLeader;

		[SyncVar]
		private float _score;
		public float Score => _score;

		public override void OnStartClient()
		{
			base.OnStartClient();

			DontDestroyOnLoad(gameObject);
			GameNetworkManager.Instance.OnPlayerGameDataStart(this);
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

		[Server]
		public void SetScore(float score)
		{
			_score = score;
		}

		[Server]
		public void AddToScore(float addScore)
		{
			SetScore(_score + addScore);
		}

		[Command]
		public void CmdExitGame()
		{
			GameNetworkManager.Instance.ExitGame();
		}
	}
}
