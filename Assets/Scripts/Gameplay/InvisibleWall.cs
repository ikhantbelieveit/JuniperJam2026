using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.purple;
		Gizmos.DrawSphere(transform.position, transform.localScale.x * 0.5f);
	}
}
