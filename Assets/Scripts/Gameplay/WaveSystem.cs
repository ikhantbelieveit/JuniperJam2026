using UnityEngine;
using JJ26.Framework;

namespace JJ26.Gameplay
{
	public class WaveSystem : BaseGameSystem
	{
		[System.Serializable]
		public struct Wave //X, Y = direction. Z = steepness. W = wavelength
		{
			[Tooltip("X, Y = direction. Z = steepness. W = wavelength.")]
			public Vector4 Parameters;
		}

		[SerializeField]
		public Wave WaveA;
		public Wave WaveB;

		public const float OCEAN_LEVEL = 0f;

		const float _gravity = 9.81f;
		const float _pi = 3.14159265f;

		public const float OCEAN_SCALE_X = 50f;
		public const float OCEAN_SCALE_Z = 50f;

		public Vector3 SampleDisplacement(Vector3 worldPos)
		{
			Vector3 displacement = Vector3.zero;

			displacement += GerstnerWave(WaveA.Parameters, worldPos);
			displacement += GerstnerWave(WaveB.Parameters, worldPos);

			return displacement;
		}

		public float SampleHeight(Vector3 worldPos)
		{
			return SampleDisplacement(worldPos).y;
		}

		public Vector3 GetWavePosition(Vector3 worldPos)
		{
			return worldPos + SampleDisplacement(worldPos);
		}

		private Vector3 GerstnerWave(Vector4 wave, Vector3 worldPos)
		{
			float steepness = wave.z;
			float wavelength = wave.w;

			float k = 2.0f * _pi / wavelength;
			float c = Mathf.Sqrt(_gravity / k);
			Vector2 d = new Vector2(wave.x, wave.y).normalized;

			Vector3 p = worldPos;

			p.x /= OCEAN_SCALE_X;
			p.z /= OCEAN_SCALE_Z;

			float f = k * (Vector2.Dot(
					d,
					new Vector2(p.x, p.z)
				) - c * Time.time);

			float amplitude = steepness / k;

			return new Vector3(
				d.x * (amplitude * Mathf.Cos(f)),
				amplitude * Mathf.Sin(f),
				d.y * (amplitude * Mathf.Cos(f)));
		}
	}
}