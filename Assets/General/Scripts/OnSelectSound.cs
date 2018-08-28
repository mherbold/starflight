
using UnityEngine;
using UnityEngine.EventSystems;

public class OnSelectSound : MonoBehaviour, ISelectHandler
{
	// this is called by unity when this game object is selected
	public void OnSelect( BaseEventData eventData )
	{
		SoundController.m_instance.PlaySound( SoundController.Sound.Click );
	}
}
