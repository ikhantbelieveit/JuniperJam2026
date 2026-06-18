using UnityEngine;
using JJ26.Network;

namespace JJ26.Gameplay
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(transform.position, 1f);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
		}
	}
}