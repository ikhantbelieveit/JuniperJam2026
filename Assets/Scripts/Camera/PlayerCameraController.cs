using UnityEngine;
using Mirror;
using JJ26.Network;

namespace JJ26.Gameplay
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [SerializeField] Transform _cameraTarget;
		[SerializeField] Transform _playerTransform;

		public override void OnStartAuthority()
		{
			base.OnStartAuthority();

			FollowCamera cam = Camera.main.GetComponent<FollowCamera>();
			cam.SetTarget(_cameraTarget);
			cam.SetPlayer(_playerTransform);
		}
	}
}