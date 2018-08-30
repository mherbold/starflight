
using UnityEngine;

public class Hyperspace : MonoBehaviour
{
	// our template star (that we will duplicate all over the place)
	public GameObject m_template;

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
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// make copies of the star template
		foreach ( Star star in gameData.m_starList )
		{
			// clone the star
			GameObject clonedStar = Instantiate( m_template, star.m_worldCoordinates, Quaternion.identity, transform ) as GameObject;

			// get to the star quad mesh object
			Transform starQuad = clonedStar.transform.Find( "Star Quad" );

			// calculate the scale of the star quad
			float scale = ( 1.0f + star.m_scale / 0.6f ) * 128.0f;

			// scale the star image based on the class of the star system
			starQuad.localScale = new Vector3( scale, scale, 1.0f );
		}

		// hide the star template
		m_template.SetActive( false );
	}

	// unity update
	void Update()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// go through each star in the game
		foreach ( Star star in gameData.m_starList )
		{
			// did we breach it?
			float distance = Vector3.Distance( playerData.m_starflight.m_hyperspaceCoordinates, star.m_worldCoordinates );

			if ( distance < star.GetBreachDistance() )
			{
				Debug.Log( "Entering star system at " + star.m_xCoordinate + " x " + star.m_yCoordinate + ", distance = " + distance );

				// change the system
				playerData.m_starflight.m_currentStarId = star.m_id;

				// update the player location
				playerData.m_starflight.m_location = Starflight.Location.StarSystem;

				// set the position of the player inside this system
				Vector3 starToShip = playerData.m_starflight.m_hyperspaceCoordinates - (Vector3) star.m_worldCoordinates;
				starToShip.Normalize();
				playerData.m_starflight.m_systemCoordinates = starToShip * ( 8192.0f - 16.0f );

				// switch to the star system mode
				m_spaceflightController.SwitchMode();
			}
		}
	}

	// call this to hide the hyperspace stuff
	public void Hide()
	{
		// hide the hyperspace objects
		gameObject.SetActive( false );
	}

	// call this to show the hyperspace stuff
	public void Show()
	{
		// show the hyperspace objects
		gameObject.SetActive( true );

		// show the player (ship)
		m_spaceflightController.m_player.Show();

		// make sure the camera is at the right height above the zero plane
		m_spaceflightController.m_player.DollyCamera( 1024.0f );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// move the ship to where we are in hyperspace
		m_spaceflightController.m_player.SetPosition( playerData.m_starflight.m_hyperspaceCoordinates );

		// show the status display
		m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_statusDisplay );

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Hyperspace );
	}
}
