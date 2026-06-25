using UnityEngine;
using Mirror;
using JJ26.Input;
using JJ26.Network;

namespace JJ26.Gameplay
{
    public class PlayerBoatController : NetworkBehaviour
    {
        [SerializeField] Rigidbody _rb;
		[SerializeField] float _speedMult = 40f;
		[SerializeField] float _steeringPower = 15f;

		public void FixedUpdate()
		{
			if (GameNetworkManager.Instance.GameState.CurrentState != EGameState.Gameplay) { return; }
			float speedMult = InputSystem.Jump ? _speedMult * 2 : _speedMult;

			//continuous forward
			_rb.AddForce(transform.forward * speedMult, ForceMode.Acceleration);

			//left/right steering
			_rb.AddTorque(Vector3.up * InputSystem.WalkValue.x * _steeringPower, ForceMode.Acceleration);
		}
	}
}