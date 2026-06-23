using UnityEngine;
using System.Collections.Generic;

namespace JJ26.Gameplay
{
    public class MatchWaveHeight : MonoBehaviour
    {
		private WaveSystem _waveSystem;

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

			transform.SetPositionAndRotation(
				new Vector3
					(transform.position.x,
					_waveSystem.SampleHeight(transform.position),
					transform.position.z),
				Quaternion.identity);
		}
	}
}