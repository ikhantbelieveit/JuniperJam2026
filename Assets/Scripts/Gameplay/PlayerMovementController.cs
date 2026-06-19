using UnityEngine;
using Mirror;
using JJ26.Input;

namespace JJ26.Gameplay
{
    public class PlayerMovementController : NetworkBehaviour
    {
		public bool IsGrounded => _groundContactCount > 0;
		public bool IsOnSteepGround => _steepContactCount > 0;

		private int _jumpPhase = 0;
		private float _minGroundDotProduct;

		private Vector3 _groundContactNormal;
		private int _groundContactCount;

		private Vector3 _steepContactNormal;
		private int _steepContactCount;

		private int _stepsSinceLastGround = 0;
		private int _stepsSinceLastJump = 0;

		private Vector3 _currentVelocity;

		[SerializeField, Range(0f, 50f)] float _maxVelocity = 5;

		[SerializeField, Range(0f, 50f)] float _maxAcceleration = 5;
		[SerializeField, Range(0f, 50f)] float _maxAirAcceleration = 1;
		[SerializeField, Range(0f, 50f)] float _maxTurnAcceleration = 5;

		[SerializeField, Range(0f, 20f)] float _jumpHeight = 5;

		[SerializeField, Range(0f, 90f)] float _maxGroundAngle = 25f;

		[SerializeField, Range(0, 5)] int _maxMidairJumps = 1;

		[SerializeField, Range(0f, 100f)] float _maxGroundSnapSpeed = 75f;

		[SerializeField, Min(0f)] float _groundCheckDistance = 1f;

		[SerializeField] LayerMask _groundCheckMask = -1;

		[SerializeField] Rigidbody _rb;

		[SerializeField, Range(0f, 1f)] float _jumpBufferDuration = 0.15f;
		private float _jumpBufferTimeLeft = 0;

		[ClientCallback]

		private void Update()
		{
			if (InputSystem.JumpPressed)
			{
				_jumpBufferTimeLeft = _jumpBufferDuration;
			}
		}

		[ClientCallback]
		private void FixedUpdate()
		{
			_currentVelocity = _rb.linearVelocity;
			UpdateJumpState();
			UpdateWalkVelocity();

			if(_jumpBufferTimeLeft > 0)
			{
				Jump();
				_jumpBufferTimeLeft = 0;
			}

			_rb.linearVelocity = _currentVelocity;

			ClearState();
		}

		private void Jump()
		{
			Vector3 jumpDirection = GetJumpDirection();
			if (jumpDirection == Vector3.zero) { return; }
			jumpDirection = (jumpDirection + Vector3.up).normalized;
			UpdateJumpPhase();
			_stepsSinceLastJump = 0;
			float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * _jumpHeight);
			float alignedSpeed = Vector3.Dot(_currentVelocity, jumpDirection);
			if (alignedSpeed > 0f)
			{
				jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
			}
			_currentVelocity += jumpDirection * jumpSpeed;
		}

		private void UpdateJumpPhase()
		{
			if (!IsGrounded)
			{
				//Account for a midair jump after falling off the side of ground
				if (_maxMidairJumps > 0 && _jumpPhase <= _maxMidairJumps && _jumpPhase == 0)
				{
					_jumpPhase = 1;
				}
				//Reset midair jumps after wall jumping
				if (IsOnSteepGround)
				{
					_jumpPhase = 0;
				}
			}

			_jumpPhase++;
		}

		private Vector3 GetJumpDirection()
		{
			if (!IsGrounded)
			{
				if (IsOnSteepGround)
				{
					return _steepContactNormal;
				}
				if (_jumpPhase <= _maxMidairJumps)
				{
					return _groundContactNormal;
				}
				return Vector3.zero;
			}
			return _groundContactNormal;
		}

		private void ClearState()
		{
			_groundContactNormal = Vector3.zero;
			_groundContactCount = 0;
			_steepContactNormal = Vector3.zero;
			_steepContactCount = 0;
		}

		private void UpdateJumpState()
		{
			_jumpBufferTimeLeft -= Time.fixedDeltaTime;
			_jumpBufferTimeLeft = Mathf.Max(0, _jumpBufferTimeLeft);
			_stepsSinceLastJump++;
			if (IsGrounded)
			{
				_stepsSinceLastGround = 0;
				if (_stepsSinceLastJump > 1)
				{
					_jumpPhase = 0;
				}
				if (_groundContactCount > 1)
				{
					_groundContactNormal.Normalize();
				}
			}
			else
			{
				_stepsSinceLastGround++;
				_groundContactNormal = Vector3.up;
			}
		}

		/// <summary>
		/// Read walk input and alter current velocity
		/// Could be fancy and add stuff to make it work on sloped ground
		/// </summary>
		private void UpdateWalkVelocity()
		{
			Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_currentVelocity, Vector3.up);
			Vector3 desiredHorizontalVelocity = new Vector3(InputSystem.WalkValue.x, 0f, InputSystem.WalkValue.y) * _maxVelocity;

			float accel = _maxAcceleration;

			if(Vector3.Dot(horizontalVelocity, desiredHorizontalVelocity) < 0f)
			{
				accel = _maxTurnAcceleration;
			}

			horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, desiredHorizontalVelocity, accel * Time.deltaTime);

			_currentVelocity.x = horizontalVelocity.x;
			_currentVelocity.z = horizontalVelocity.z;
		}

		private void OnCollisionEnter(Collision collision)
		{
			CheckCollisionForGround(collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			CheckCollisionForGround(collision);
		}


		/// <summary>
		/// Evaluate surface collision to detect whether on ground or steep ground.
		/// Normal of flat ground = 1, vertical wall = 0, ceiling = -1
		/// </summary>
		private void CheckCollisionForGround(Collision collision)
		{
			for (int contactIndex = 0; contactIndex < collision.contactCount; ++contactIndex)
			{
				ContactPoint contact = collision.GetContact(contactIndex);
				Vector3 contactNormal = contact.normal;
				if (contactNormal.y >= _minGroundDotProduct)
				{
					_groundContactNormal += contactNormal;  //get average of all ground contacts (will be multiple if in a dip)
					_groundContactCount++;
				}
				else if (contactNormal.y > -0.01f)
				{
					_steepContactNormal += contactNormal;
					_steepContactCount++;
				}
			}
		}
	}
}

