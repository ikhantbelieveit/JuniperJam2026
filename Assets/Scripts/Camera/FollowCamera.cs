using UnityEngine;

namespace JJ26.Gameplay
{
    public class FollowCamera : MonoBehaviour
    {
        private Transform _cameraTarget;
		private Transform _playerTransform;

        private float _smoothSpeed = 10f;

		private void LateUpdate()
		{
			if(null == _cameraTarget ||
				null == _playerTransform) { return; }

			transform.position = Vector3.Lerp(
				transform.position,
				_cameraTarget.position,
				_smoothSpeed * Time.deltaTime);

			transform.LookAt(_playerTransform.position);
		}

		public void SetTarget(Transform newTarget)
		{
			_cameraTarget = newTarget;
			Debug.Log("Set new camera target");
		}

		public void SetPlayer(Transform newPlayer)
		{
			_playerTransform = newPlayer;
		}
	}
}