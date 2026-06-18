using UnityEngine;
using JJ26.Network;

namespace JJ26.Gameplay
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
		private void Awake()
		{
			PlayerSpawnSystem.AddSpawnPoint(transform);
		}

		private void OnDestroy()
		{
			PlayerSpawnSystem.RemoveSpawnPoint(transform);
		}
	}
}