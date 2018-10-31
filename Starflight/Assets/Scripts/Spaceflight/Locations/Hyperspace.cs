
using UnityEngine;

public class Hyperspace : MonoBehaviour
{
	// our star template (that we will duplicate all over the place)
	public GameObject m_starTemplate;

	// our flux template (that we will duplicate all over the place)
	public GameObject m_fluxTemplate;

	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// true if we are currently traveling through a flux
	bool m_travelingThroughFlux;

	// flux travel timer
	float m_timer;

	// flux travel duration
	float m_fluxTravelDuration;

	// the starting point
	Vector3 m_fluxTravelStartPosition;

	// the ending point
	Vector3 m_fluxTravelEndPosition;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// make copies of the star template
		foreach ( var star in gameData.m_starList )
		{
			// clone the star
			var clonedStar = Instantiate( m_starTemplate, star.GetWorldCoordinates(), Quaternion.identity, transform ) as GameObject;

			// activate the star
			clonedStar.SetActive( true );

			// get to the star quad mesh object
			var starQuad = clonedStar.transform.Find( "Star Quad" );

			// calculate the scale of the star quad
			float scale = star.GetBreachDistance() * 3.0f;

			// scale the star image based on the class of the star system
			starQuad.localScale = new Vector3( scale, scale, 1.0f );
		}

		// hide the star template
		m_starTemplate.SetActive( false );

		// make copies of the flux template
		foreach ( GD_Flux flux in gameData.m_fluxList )
		{
			// clone the flux
			var clonedFlux = Instantiate( m_fluxTemplate, flux.GetFrom(), Quaternion.identity, transform );

			// activate the flux
			clonedFlux.SetActive( true );
		}

		// hide the flux template
		m_fluxTemplate.SetActive( false );

		// we are not travelling through a flux now
		m_travelingThroughFlux = false;
	}

	// unity update
	void Update()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// are we travelling through a flux right now?
		if ( m_travelingThroughFlux )
		{
			// update the timer
			m_timer += Time.deltaTime;

			// calculate a smoothed curve for moving our position to the other end of the flux
			float t = Mathf.SmoothStep( 0.0f, 1.0f, m_timer / m_fluxTravelDuration );
			
			// update the position of the ship
			var newPosition = Vector3.Lerp( m_fluxTravelStartPosition, m_fluxTravelEndPosition, t );

			// update the ship position
			playerData.m_general.m_hyperspaceCoordinates = m_spaceflightController.m_player.transform.position = newPosition;

			// rotate the skybox in the direction of the flux travel
			var direction = Vector3.Normalize( m_fluxTravelStartPosition - m_fluxTravelEndPosition );
			m_spaceflightController.m_player.RotateSkybox( -direction, Time.deltaTime * 5.0f );

			// have we arrived?
			if ( m_timer >= m_fluxTravelDuration )
			{
				// end the space warp effect
				m_spaceflightController.m_player.StopSpaceWarp();

				// let the ship move again
				m_spaceflightController.m_player.Unfreeze();

				// update the map coordinates
				m_spaceflightController.m_map.UpdateCoordinates();

				// play the exit warp sound
				SoundController.m_instance.PlaySound( SoundController.Sound.ExitWarp );

				// not travelling through the flux any more
				m_travelingThroughFlux = false;
			}
		}
		else
		{
			// go through each star in the game
			foreach ( GD_Star star in gameData.m_starList )
			{
				// did we breach it?
				float distance = Vector3.Distance( playerData.m_general.m_hyperspaceCoordinates, star.GetWorldCoordinates() );

				if ( distance < star.GetBreachDistance() )
				{
					Debug.Log( "Entering star system at " + star.m_xCoordinate + " x " + star.m_yCoordinate + " - switching to the star system location." );

					// change the system
					playerData.m_general.m_currentStarId = star.m_id;

					// set the position of the player inside this system
					var starToShip = playerData.m_general.m_hyperspaceCoordinates - star.GetWorldCoordinates();
					starToShip.Normalize();
					playerData.m_general.m_starSystemCoordinates = starToShip * ( 8192.0f - 16.0f );

					// scale the speed of the player
					playerData.m_general.m_currentSpeed *= 4.0f;
					playerData.m_general.m_currentMaximumSpeed *= 4.0f;

					// switch to the star system location
					m_spaceflightController.SwitchLocation( PD_General.Location.StarSystem );
				}
			}

			// go through each flux in the game
			foreach ( GD_Flux flux in gameData.m_fluxList )
			{
				// did we breach it?
				var distance = Vector3.Distance( playerData.m_general.m_hyperspaceCoordinates, flux.GetFrom() );

				if ( distance < flux.GetBreachDistance() )
				{
					Debug.Log( "Entering flux at " + flux.m_x1 + " x " + flux.m_y1 + ", distance = " + distance );

					// prevent the player from maneuvering
					m_spaceflightController.m_player.Freeze();

					// reset the timer
					m_timer = 0.0f;

					// figure out how long we should take to travel through this flux
					m_fluxTravelDuration = Mathf.Max( 2.0f, Vector3.Distance( flux.GetFrom(), flux.GetTo() ) / 2048.0f );

					// compute the starting and ending point of the flux travel
					m_fluxTravelStartPosition = playerData.m_general.m_hyperspaceCoordinates;
					m_fluxTravelEndPosition = flux.GetTo() + (Vector3) playerData.m_general.m_currentDirection * ( flux.GetBreachDistance() + 16.0f );

					// start the warp cinematics
					m_travelingThroughFlux = true;

					// start the warp effect
					m_spaceflightController.m_player.StartSpaceWarp();

					// play the enter warp sound
					SoundController.m_instance.PlaySound( SoundController.Sound.EnterWarp );
				}
			}
		}
	}

	// call this to hide the hyperspace stuff
	public void Hide()
	{
		if ( !gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the hyperspace location." );

		// hide the hyperspace objects
		gameObject.SetActive( false );
	}

	// call this to show the hyperspace stuff
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the hyperspace location." );

		// show the hyperspace objects
		gameObject.SetActive( true );

		// show the player (ship)
		m_spaceflightController.m_player.Show();

		// make sure the camera is at the right height above the zero plane
		m_spaceflightController.m_player.DollyCamera( 1024.0f );

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// move the ship to where we are in hyperspace
		m_spaceflightController.m_player.transform.position = playerData.m_general.m_hyperspaceCoordinates;

		// calculate the new rotation of the player
		var newRotation = Quaternion.LookRotation( playerData.m_general.m_currentDirection, Vector3.up );

		// update the player rotation
		m_spaceflightController.m_player.m_ship.rotation = newRotation;

		// unfreeze the player
		m_spaceflightController.m_player.Unfreeze();

		// fade in the map
		m_spaceflightController.m_map.StartFade( 1.0f, 2.0f );

		// show the status display
		m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_statusDisplay );

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Hyperspace );
	}
}
