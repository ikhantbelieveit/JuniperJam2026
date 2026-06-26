using UnityEngine;
using Mirror;
using System.Collections.Generic;
using JJ26.Network;

namespace JJ26.Gameplay
{
    public class CollectTreasure : NetworkBehaviour
    {
		[SerializeField] float _scoreValue;
		[SerializeField] BoxCollider _boxCollision;
		[SerializeField] GameObject _visualsGO;
		[SerializeField] GameObject _beamGO;
		[SerializeField] AudioSource _collectAudio;

		public void OnCollisionEnter(Collision collision)
		{
			var playerBoat = collision.gameObject.GetComponent<PlayerBoatController>();
			if (!playerBoat) return;
			Collect(playerBoat.connectionToClient);
		}

		public void OnTriggerEnter(Collider other)
		{
			var playerBoat = other.gameObject.GetComponent<PlayerBoatController>();
			if (!playerBoat) return;
			Collect(playerBoat.connectionToClient);
		}

		public void Collect(NetworkConnectionToClient conn)
		{
			List<PlayerGameData> gameData = GameNetworkManager.Instance.GamePlayers;

			if(GameNetworkManager.Instance.GameState?.CurrentState == EGameState.Gameplay)
			{
				foreach (var data in gameData)
				{
					if (data.connectionToClient == conn)
					{
						data.AddToScore(_scoreValue);
					}
				}
			}

			_boxCollision.enabled = false;
			_visualsGO.SetActive(false);
			_beamGO.SetActive(false);
			_collectAudio.Play();
		}
	}
}