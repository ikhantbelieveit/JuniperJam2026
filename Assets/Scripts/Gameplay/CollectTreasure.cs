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
			foreach(var data in gameData)
			{
				if(data.connectionToClient == conn)
				{
					data.AddToScore(_scoreValue);
				}
			}

			_boxCollision.enabled = false;
			_visualsGO.SetActive(false);
		}
	}
}