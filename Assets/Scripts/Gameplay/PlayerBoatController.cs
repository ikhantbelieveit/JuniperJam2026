using UnityEngine;
using Mirror;
using JJ26.Input;
using JJ26.Network;
using JJ26.UI;

namespace JJ26.Gameplay
{
    public class PlayerBoatController : NetworkBehaviour
    {
        [SerializeField] Rigidbody _rb;
		[SerializeField] float _speedMult = 1f;
		[SerializeField] float _steeringPower = 15f;

		private DirectionWheel _steeringWheel;
		private DirectionWheel _powerWheel;

		private void Start()
		{
			GameplayUIController gameUI = FindAnyObjectByType(typeof(GameplayUIController)) as GameplayUIController;
			_steeringWheel = gameUI.SteeringWheel;
			_powerWheel = gameUI.PowerWheel;
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
	}
}