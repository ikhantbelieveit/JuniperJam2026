using UnityEngine;
using JJ26.Framework;

namespace JJ26.Audio
{
    public class AudioSystem : BaseGameSystem
    {
		[SerializeField] private AudioSource _buttonClickAudio;

		public override void Awake()
		{
			base.Awake();

		}

		public void PlayButtonClickSound()
		{
			_buttonClickAudio.Play();
		}
	}
}
