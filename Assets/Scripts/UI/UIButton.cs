using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JJ26.Audio;

namespace JJ26.UI
{
	public class UIButton : MonoBehaviour, IPointerDownHandler
	{
		public void OnPointerDown(PointerEventData eventData)
		{
			PlayButtonSound();
		}

		public void PlayButtonSound()
		{
			var audioSystem = FindAnyObjectByType(typeof(AudioSystem)) as AudioSystem;
			audioSystem.PlayButtonClickSound();
		}
	}
}