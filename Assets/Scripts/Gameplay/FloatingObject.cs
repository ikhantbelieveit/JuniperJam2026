using UnityEngine;

namespace JJ26.Gameplay
{
    public class FloatingObject : MonoBehaviour
    {
		[SerializeField] Rigidbody _rigidbody;
		public float DepthBeforeSubmerged = 1f;
		public float DisplacementAmount = 3f;
		public int FloatPointCount = 1;
		public float WaterDrag = 0.99f;
		public float WaterAngularDrag = 0.5f;
		public float GravityScale = 1;

		private void FixedUpdate()
		{
			_rigidbody.AddForceAtPosition(Physics.gravity / FloatPointCount * GravityScale, transform.position, ForceMode.Acceleration);

			if(transform.position.y < 0f)
			{
				float displacementMult = Mathf.Clamp01(-transform.position.y / DepthBeforeSubmerged) * DisplacementAmount;
				_rigidbody.AddForce(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMult, 0f), ForceMode.Acceleration);
				_rigidbody.AddForce(displacementMult * -_rigidbody.linearVelocity * WaterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
				_rigidbody.AddTorque(displacementMult * -_rigidbody.angularVelocity * WaterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
			}
		}
	}
}