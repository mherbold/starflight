
using UnityEngine;

public class DockingBay : MonoBehaviour
{
	// particle systems
	public ParticleSystem m_decompressionParticleSystem;

	// the animator
	Animator m_animator;

	// unity awake
	void Awake()
	{
		// get the animator
		m_animator = GetComponent<Animator>();
	}

	// call this to hide the docking bay
	public void Hide()
	{
		if ( !gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the docking bay location." );

		// hide the docking bay
		gameObject.SetActive( false );
	}

	// call this to switch to the docking bay
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the docking bay location." );

		// show the docking bay
		gameObject.SetActive( true );

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// put us in the right spot for the docking bay launch sequence
		SpaceflightController.m_instance.m_playerShip.transform.position = playerData.m_general.m_coordinates = new Vector3( 0.0f, 0.0f, 0.0f );

		// freeze the player
		SpaceflightController.m_instance.m_playerShip.Freeze();

		// play the in space camera animation
		SpaceflightController.m_instance.m_playerCamera.StartAnimation( "In Space" );

		// reset the buttons
		SpaceflightController.m_instance.m_buttonController.RestoreBridgeButtons();

		// fade in the map
		SpaceflightController.m_instance.m_viewport.StartFade( 1.0f, 2.0f );

		// make sure we have the status display up
		SpaceflightController.m_instance.m_displayController.ChangeDisplay( SpaceflightController.m_instance.m_displayController.m_statusDisplay );

		// play the docking bay music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.DockingBay );
	}

	// call this to start the launch animation
	public void StartLaunchAnimation()
	{
		m_animator.Play( "Docking Bay Launch" );

		// play the launch sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Launch );

		// play the docking bay door open sound
		SoundController.m_instance.PlaySound( SoundController.Sound.DockingBayDoorOpen );

		// play the decompression sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Decompression );

		// run the launching camera animation
		SpaceflightController.m_instance.m_playerCamera.StartAnimation( "Launching (Docking Bay)" );

		// fire up the particle system
		m_decompressionParticleSystem.Play();
	}

	// call this to start the landing animation
	public void StartLandingAnimation()
	{
		m_animator.Play( "Docking Bay Landing" );

		// play the docking bay door open sound
		SoundController.m_instance.PlaySound( SoundController.Sound.DockingBayDoorClose );

		// run the landing camera animation
		SpaceflightController.m_instance.m_playerCamera.StartAnimation( "Landing (Docking Bay)" );
	}
}
