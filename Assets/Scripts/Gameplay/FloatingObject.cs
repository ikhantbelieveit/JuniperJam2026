using UnityEngine;
using System.Collections.Generic;

namespace JJ26.Gameplay
{
    public class FloatingObject : MonoBehaviour
    {
		private WaveSystem _waveSystem;

		[SerializeField] Rigidbody _rigidbody;
		[SerializeField] List<Transform> _floatPoints;

		public float DepthBeforeSubmerged = 1f;
		public float DisplacementAmount = 3f;
		//public int FloatPointCount = 1;
		public float WaterDrag = 0.99f;
		public float WaterAngularDrag = 0.5f;
		public float GravityScale = 1;

		public void Start()
		{
			_waveSystem = FindAnyObjectByType(typeof(WaveSystem)) as WaveSystem;
		}

		private void FixedUpdate()
		{
			if(!_waveSystem)
			{
				_waveSystem = FindAnyObjectByType(typeof(WaveSystem)) as WaveSystem;
			}
			float waveHeight = _waveSystem.SampleHeight(transform.position);

			foreach(Transform point in _floatPoints)
			{
				_rigidbody.AddForceAtPosition(Physics.gravity / _floatPoints.Count * GravityScale, point.position, ForceMode.Acceleration);

				if (point.position.y < waveHeight)
				{
					float displacementMult = Mathf.Clamp01((waveHeight - point.position.y) / DepthBeforeSubmerged) * DisplacementAmount;
					_rigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y / _floatPoints.Count) * displacementMult, 0f), point.position, ForceMode.Acceleration);
					_rigidbody.AddForce(displacementMult * -_rigidbody.linearVelocity * WaterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
					_rigidbody.AddTorque(displacementMult * -_rigidbody.angularVelocity * WaterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
				}
			}

			
		}
	}
}