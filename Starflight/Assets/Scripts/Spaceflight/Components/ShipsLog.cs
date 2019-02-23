
using UnityEngine;
using UnityEngine.UI;

public class ShipsLog : MonoBehaviour
{
	public GameObject m_selectionXform;

	public Image m_upArrowImage;
	public Image m_downArrowImage;

	// unity update
	void Update()
	{
		// if the player hits cancel (esc) or submit (enter) close the ships log
		if ( InputController.m_instance.m_cancel || InputController.m_instance.m_submit )
		{
			Hide();

			InputController.m_instance.Debounce();

			SpaceflightController.m_instance.m_buttonController.DeactivateButton();
		}
		else
		{
			// get the controller stick position
			float controllerY = InputController.m_instance.m_y;

			// check if we want to change entries
			if ( ( controllerY < -0.5f ) || ( controllerY > 0.5f ) )
			{
				// yes
			}
		}
	}

	// hide the ships log
	public void Hide()
	{
		// unpause the game
		SpaceflightController.m_instance.m_gameIsPaused = false;

		// make this game object not active
		gameObject.SetActive( false );
	}

	// show the ships log
	public void Show()
	{
		// pause the game
		SpaceflightController.m_instance.m_gameIsPaused = true;

		// make this game object active
		gameObject.SetActive( true );

		// play a sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Beep );

		// change the viewport label
		SpaceflightController.m_instance.m_viewport.UpdateLabel( "Ships Log" );
	}

	// returns true if the ships log is open
	public bool IsOpen()
	{
		return gameObject.activeInHierarchy;
	}
}
