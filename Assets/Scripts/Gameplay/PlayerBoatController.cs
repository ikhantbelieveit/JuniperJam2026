using UnityEngine;
using Mirror;
using JJ26.Input;
using JJ26.Network;
using JJ26.UI;
using UnityEngine.UI;
using TMPro;

namespace JJ26.Gameplay
{
    public class PlayerBoatController : NetworkBehaviour
    {
        [SerializeField] Rigidbody _rb;
		[SerializeField] float _speedMult = 1f;
		[SerializeField] float _steeringPower = 15f;
		[SerializeField] TMP_Text _nameText;

		private DirectionWheel _steeringWheel;
		private DirectionWheel _powerWheel;

		private void Start()
		{
			GameplayUIController gameUI = FindAnyObjectByType(typeof(GameplayUIController)) as GameplayUIController;
			_steeringWheel = gameUI.SteeringWheel;
			_powerWheel = gameUI.PowerWheel;

            foreach(var gamePlayer in GameNetworkManager.Instance.GamePlayers)
			{
				if(gamePlayer.connectionToClient == connectionToClient)
				{
					_nameText.text = gamePlayer.DisplayName;
					break;
				}
			}
        }

        public void FixedUpdate()
		{
			if (GameNetworkManager.Instance.GameState.CurrentState != EGameState.Gameplay) { return; }

			//continuous forward
			float forwardSpeed = _powerWheel.InputValue * _speedMult;
			_rb.AddForce(transform.forward * forwardSpeed, ForceMode.Acceleration);

			//left/right steering
			_rb.AddTorque(Vector3.up * -_steeringWheel.InputValue * _steeringPower, ForceMode.Acceleration);
		}

		public void LateUpdate()
		{
			if(Camera.main != null)
			{
				_nameText.transform.forward = Camera.main.transform.forward;
			}
		}
	}
}