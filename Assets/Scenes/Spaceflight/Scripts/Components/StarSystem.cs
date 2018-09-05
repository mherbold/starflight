
using UnityEngine;

public class StarSystem : MonoBehaviour
{
	// convenient access to the spaceflight controller
	SpaceflightController m_spaceflightController;

	// unity awake
	private void Awake()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();
	}

	// unity start
	void Start()
	{

	}

	// unity update
	void Update()
	{
		// configure the infinite starfield system to become fully visible at higher speeds
		m_spaceflightController.m_player.SetStarfieldFullyVisibleSpeed( 15.0f );
	}

	// call this to hide the starsystem objects
	public void Hide()
	{
		// hide the starsystem
		gameObject.SetActive( false );
	}

	// call this to show the starsystem objects
	public void Show()
	{
		// show the starsystem
		gameObject.SetActive( true );

		// show the player (ship)
		m_spaceflightController.m_player.Show();

		// make sure the camera is at the right height above the zero plane
		m_spaceflightController.m_player.DollyCamera( 1024.0f );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// move the player object
		m_spaceflightController.m_player.SetPosition( playerData.m_starflight.m_systemCoordinates );

		// update the system controller
		m_spaceflightController.m_systemController.EnterSystem();

		// show the status display
		m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_systemDisplay );

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.StarSystem );
	}
}
