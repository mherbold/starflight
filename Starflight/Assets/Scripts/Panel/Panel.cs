
using UnityEngine;
using UnityEngine.EventSystems;

abstract public class Panel : MonoBehaviour
{
	// the animator for this panel (to open and close it)
	public Animator m_panelAnimator;

	// panel tick
	public virtual void Tick()
	{
	}

	// start the animation to show this panel
	public virtual bool Open()
	{
		// unhide the panel
		gameObject.SetActive( true );

		// tell the panel to start animating to the "open" position
		m_panelAnimator.SetBool( "Open", true );

		// make sure nothing is selected
		EventSystem.current.SetSelectedGameObject( null );

		// play the compressed air sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Open );

		// return true to let the panel controller know we have opened this panel
		return true;
	}

	// start the animation to hide this panel
	public virtual void Close()
	{
		// tell the panel to start animating to the "closed" position
		m_panelAnimator.SetBool( "Open", false );

		// play the escaping air sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Close );
	}

	// this is called when the opening animation is done
	public virtual void Opened()
	{
	}

	// this is called when the closing animation is done
	public virtual void Closed()
	{
		// hide the game object
		gameObject.SetActive( false );
	}
}
