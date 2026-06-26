using UnityEngine;
using JJ26.Framework;

namespace JJ26.Audio
{
    public class AudioSystem : BaseGameSystem
    {
        [SerializeField] private AudioSource _audio;

		public override void Awake()
		{
			base.Awake();

		}
	}
}
