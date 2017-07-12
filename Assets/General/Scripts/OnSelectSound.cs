
using UnityEngine;
using UnityEngine.EventSystems;

public class OnSelectSound : MonoBehaviour, ISelectHandler
{
	// this is called by unity when this game object is selected
	public void OnSelect( BaseEventData eventData )
	{
		UISoundController uiSoundController = SceneControllersInstance.m_instance.GetComponent<UISoundController>();

		uiSoundController.Play( UISoundController.UISound.Click );
	}
}
