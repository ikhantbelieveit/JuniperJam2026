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
		public float WaterDrag = 0.99f;
		public float WaterAngularDrag = 0.5f;
		public float GravityScale = 1;
		public float RightingTorqueStrength = 20f;

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

			foreach(Transform point in _floatPoints)
			{
				float pointHeight = _waveSystem.SampleHeight(point.position);
				_rigidbody.AddForceAtPosition(Physics.gravity / _floatPoints.Count * GravityScale, point.position, ForceMode.Acceleration);

				if (point.position.y < pointHeight)
				{
					float displacementMult = Mathf.Clamp01((pointHeight - point.position.y) / DepthBeforeSubmerged) * DisplacementAmount;
					_rigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y / _floatPoints.Count) * displacementMult, 0f), point.position, ForceMode.Acceleration);
					_rigidbody.AddForce(-_rigidbody.linearVelocity * WaterDrag * displacementMult, ForceMode.Acceleration);
					_rigidbody.AddTorque(-_rigidbody.angularVelocity * WaterAngularDrag, ForceMode.Acceleration);

					Vector3 normal = _waveSystem.SampleNormal(point.position);
					Vector3 torqueAxis = Vector3.Cross(transform.up, normal);
					_rigidbody.AddTorque(torqueAxis * RightingTorqueStrength * displacementMult, ForceMode.Acceleration);

				}
			}
		}
	}
}