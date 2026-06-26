using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] AudioSource _audio;

    public void OnPointerDown(PointerEventData eventData)
	{
		PlayButtonSound();
	}

    public void PlayButtonSound()
	{
		_audio.Play();
	}
}
