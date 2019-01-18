
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
	// the main camera (manual control)
	public Transform m_manualControlCamera;

	// the actual camera
	public Camera m_camera;

	// the ship
	public Transform m_ship;

	// the missile launcher
	public GameObject m_missileLauncher;

	// the laser cannon
	public GameObject m_laserCannon;

	// the cargo pods
	public GameObject[] m_cargoPods;

	// the infinite starfield
	public InfiniteStarfield m_infiniteStarfield;

	// the maximum speed of the player
	public float m_maximumSpeed;

	// the minimum time to reach the maximum speed
	public float m_minimumTimeToReachMaximumSpeed;

	// the time to slow down (coast) to a stop
	public float m_timeToStop;

	// set to true when the engines are on (to accelerate) or off (to brake)
	bool m_enginesAreOn;

	// set to true to prevent the player from moving (temporarily)
	bool m_freezePlayer;

	// keep track of the last direction (for banking the ship)
	Vector3 m_lastDirection;

	// keep track of the last banking angle (for interpolation)
	float m_currentBankingAngle;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// show only as many cargo pods as we have purchased
		for ( int cargoPodId = 0; cargoPodId < m_cargoPods.Length; cargoPodId++ )
		{
			m_cargoPods[ cargoPodId ].SetActive( cargoPodId < playerData.m_playerShip.m_numCargoPods );
		}

		// hide or show the missile launchers depending on if we have them
		m_missileLauncher.SetActive( playerData.m_playerShip.m_missileLauncherClass > 0 );

		// hide or show the missile launchers depending on if we have them
		m_laserCannon.SetActive( playerData.m_playerShip.m_laserCannonClass > 0 );

		// jump start the last direction
		m_lastDirection = playerData.m_general.m_currentDirection;
	}

	// unity update
	void Update()
	{
		// don't do anything if the game is paused
		if ( SpaceflightController.m_instance.m_gameIsPaused )
		{
			return;
		}

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the amount of enduruium remaining in storage
		var elementReference = playerData.m_playerShip.m_elementStorage.Find( 5 );

		// do this part only if we havent frozen the player (in flux travel)
		if ( !m_freezePlayer )
		{
			// are we out of fuel?
			if ( elementReference == null )
			{
				// yes - turn the engines off
				m_enginesAreOn = false;
			}

			// are the engines turned on?
			if ( m_enginesAreOn )
			{
				// update the current maximum speed of the player
				playerData.m_general.m_currentMaximumSpeed = m_maximumSpeed * ( ( playerData.m_general.m_location == PD_General.Location.StarSystem ) ? 4.0f : 1.0f );

				// calculate the acceleration
				var acceleration = Time.deltaTime * playerData.m_playerShip.m_acceleration / ( m_minimumTimeToReachMaximumSpeed * 25.0f );

				// increase the current speed
				playerData.m_general.m_currentSpeed = Mathf.Lerp( playerData.m_general.m_currentSpeed, playerData.m_general.m_currentMaximumSpeed, acceleration );

				// are we in hyperspace?
				if ( playerData.m_general.m_location == PD_General.Location.Hyperspace )
				{
					// get the engines
					var engines = playerData.m_playerShip.GetEngines();

					// calculate the amount of fuel used up
					playerData.m_playerShip.m_fuelUsed += ( playerData.m_general.m_currentSpeed * engines.m_fuelUsedPerCoordinate / 256.0f ) * Time.deltaTime;

					// have we used up more than 0.1 units?
					if ( playerData.m_playerShip.m_fuelUsed >= 0.1f )
					{
						// yes - deduct 0.1 unit from storage
						playerData.m_playerShip.m_elementStorage.Remove( 5, 1 );

						// recalculate the volume used up in the cargo bays
						playerData.m_playerShip.RecalculateVolumeUsed();

						// adjust fuel use
						playerData.m_playerShip.m_fuelUsed -= 0.1f;
					}
				}
			}
			else
			{
				// slow the ship to a stop
				playerData.m_general.m_currentSpeed = Mathf.Lerp( playerData.m_general.m_currentSpeed, 0.0f, Time.deltaTime / m_timeToStop );
			}

			// check if the ship is moving
			if ( playerData.m_general.m_currentSpeed >= 0.1f )
			{
				// calculate the new position of the player
				var newPosition = transform.position + (Vector3) playerData.m_general.m_currentDirection * playerData.m_general.m_currentSpeed * Time.deltaTime;

				// make sure the ship stays on the zero plane
				newPosition.y = 0.0f;

				// update the player position
				transform.position = newPosition;

				// update the player data (it will save out to disk eventually)
				playerData.m_general.m_coordinates = newPosition;

				// update the last coordinate based on the location
				switch ( playerData.m_general.m_location )
				{
					case PD_General.Location.Hyperspace:
						playerData.m_general.m_lastHyperspaceCoordinates = playerData.m_general.m_coordinates;
						break;

					case PD_General.Location.StarSystem:
						playerData.m_general.m_lastStarSystemCoordinates = playerData.m_general.m_coordinates;
						break;

					case PD_General.Location.Encounter:
						playerData.m_general.m_lastEncounterCoordinates = playerData.m_general.m_coordinates;
						break;
				}

				// update the map coordinates
				SpaceflightController.m_instance.m_viewport.UpdateCoordinates();
			}

			// set the rotation of the ship
			m_ship.rotation = Quaternion.LookRotation( playerData.m_general.m_currentDirection, Vector3.up );

			// get the number of degrees we are turning the ship (compared to the last frame)
			var bankingAngle = Vector3.SignedAngle( playerData.m_general.m_currentDirection, m_lastDirection, Vector3.up );

			// scale the angle enough so we actually see the ship banking (but max it out at 60 degrees in either direction)
			bankingAngle = Mathf.Max( -60.0f, Mathf.Min( 60.0f, bankingAngle * 12.0f ) );

			// interpolate towards the new banking angle
			m_currentBankingAngle = Mathf.Lerp( m_currentBankingAngle, bankingAngle, Time.deltaTime * 4.0f );

			// bank the ship based on the calculated angle
			m_ship.rotation = Quaternion.AngleAxis( m_currentBankingAngle, playerData.m_general.m_currentDirection ) * m_ship.rotation;

			// update the last direction
			m_lastDirection = playerData.m_general.m_currentDirection;
		}

		// get the current hyperspace coordinates (if in hyperspace get it from the player transform due to flux travel not updating m_hyperspaceCoordinates)
		var hyperspaceCoordinates = ( playerData.m_general.m_location == PD_General.Location.Hyperspace ) ? transform.position : playerData.m_general.m_lastHyperspaceCoordinates;

		// figure out how far we are from each nebula
		foreach ( var nebula in gameData.m_nebulaList )
		{
			nebula.Update( hyperspaceCoordinates );
		}

		// sort the results
		Array.Sort( gameData.m_nebulaList );

		// TODO: affect shields
	}

	// call this to show the player (camera and all)
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		gameObject.SetActive( true );

		Debug.Log( "Showing the player (parent)." );
	}

	// call this to hide the player (camera and all)
	public void Hide()
	{
		if ( !gameObject.activeInHierarchy )
		{
			return;
		}

		gameObject.SetActive( false );

		Debug.Log( "Hiding the player (parent)." );
	}

	// call this to show the ship
	public void ShowShip()
	{
		if ( m_ship.gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the player ship." );

		// show the ship
		m_ship.gameObject.SetActive( true );
	}

	// call this to hide the ship
	public void HideShip()
	{
		if ( !m_ship.gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the player ship." );

		// hide the ship
		m_ship.gameObject.SetActive( false );
	}

	// call this to turn on the engines (accelerate)
	public void TurnOnEngines()
	{
		m_enginesAreOn = true;
	}

	// call this to turn off the engines (brake)
	public void TurnOffEngines()
	{
		m_enginesAreOn = false;
	}

	// call this to change the height of the camera above the zero plane
	public void DollyCamera( float distance )
	{
		// subtract 1024 since that is the baseline position for the animated camera
		distance -= 1024.0f;

		// calculate the camera vector
		var cameraPosition = new Vector3( 0.0f, distance, 0.0f );

		// set it on the camera
		m_manualControlCamera.localPosition = cameraPosition;

		// update the clipping planes
		m_camera.farClipPlane = Mathf.Max( 2048.0f, distance + 1024.0f + 512.0f );
	}

	// call this to get the current position of the player
	public Vector3 GetPosition()
	{
		return transform.position;
	}

	// call this to temporarily stop the player from moving (e.g. while travelling in flux)
	public void Freeze()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// make sure the player has no momentum to start with
		playerData.m_general.m_currentSpeed = 0.0f;

		// freeze the player
		m_freezePlayer = true;
	}

	// call this to allow the player to move again
	public void Unfreeze()
	{
		// unfreeze the player
		m_freezePlayer = false;
	}

	// start the space warp effect
	public void StartSpaceWarp()
	{
		SpaceflightController.m_instance.m_playerCamera.EnterWarp();
	}

	// stop the space warp effect
	public void StopSpaceWarp()
	{
		SpaceflightController.m_instance.m_playerCamera.ExitWarp();
	}
}
