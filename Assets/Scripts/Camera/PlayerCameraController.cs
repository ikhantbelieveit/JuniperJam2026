using UnityEngine;
using Mirror;
using JJ26.Network;

namespace JJ26.Gameplay
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [SerializeField] Transform _cameraTarget;
		[SerializeField] Transform _playerTransform;

		public void Start()
		{
			PlayerGameData gameData = GameNetworkManager.Instance.GetLocalGameData();
			if(gameData.connectionToClient == connectionToClient)
			{
				FollowCamera cam = Camera.main.GetComponent<FollowCamera>();
				cam.SetTarget(_cameraTarget);
				cam.SetPlayer(_playerTransform);
			}
			//if(GameNetworkManager.Instance.islo)
		}

		//public override void OnStartLocalPlayer()
		//{
		//	Debug.Log("Local player start - try set camera target");
		//	base.OnStartLocalPlayer();

		//	Camera.main.GetComponent<FollowCamera>().SetTarget(_cameraTarget);
		//}
	}
}