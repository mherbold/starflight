
using UnityEngine;

public class InOrbit : MonoBehaviour
{
	// the nebula overlay
	public GameObject m_nebula;

	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
	}

	// call this to hide the in orbit objects
	public void Hide()
	{
		// hide the starsystem
		gameObject.SetActive( false );
	}

	// call this to show the in orbit objects
	public void Show()
	{
		// if we are already active then don't do it again
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the in orbit scene." );

		// show the in orbit objects
		gameObject.SetActive( true );

		// freeze the player
		m_spaceflightController.m_player.Freeze();

		// reset the buttons
		m_spaceflightController.m_buttonController.RestoreBridgeButtons();

		// play the docking bay music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.InOrbit );
	}
}
