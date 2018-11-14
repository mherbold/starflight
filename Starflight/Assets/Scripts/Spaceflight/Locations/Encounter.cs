
using UnityEngine;

public class Encounter : MonoBehaviour
{
	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// the speed the alien ships move at
	public float m_alienShipSpeed;

	// the rate the alien ships turn at
	public float m_alienShipTurnRate;

	// alien ship models (need 8)
	public GameObject[] m_alienShipModelList;

	// template models that we will clone as needed (need 23)
	public GameObject[] m_alienShipModelTemplate;

	// the current encounter data (both player and game)
	public PD_Encounter m_pdEncounter;
	public GD_Encounter m_gdEncounter;

	// alien ship data
	PD_AlienShip[] m_alienShipList;

	// current dolly distance
	float m_currentDollyDistance;

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
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// update the position and rotation of the active alien ship models
		for ( var i = 0; i < m_alienShipModelList.Length; i++ )
		{
			var alienShipModel = m_alienShipModelList[ i ];

			if ( alienShipModel.activeInHierarchy )
			{
				var alienShip = m_alienShipList[ i ];

				var encounterType = gameData.m_encounterTypeList[ alienShip.m_encounterTypeId ];

				switch ( encounterType.m_scriptId )
				{
					case 3:
						MechanUpdate( alienShip, alienShipModel );
						break;

					default:
						break;
				}
			}
		}

		// remember the extents
		var xExtent = 0.0f;
		var zExtent = 0.0f;

		// update the position and rotation of the active alien ship models
		for ( var i = 0; i < m_alienShipModelList.Length; i++ )
		{
			var alienShipModel = m_alienShipModelList[ i ];

			if ( alienShipModel.activeInHierarchy )
			{
				var alienShip = m_alienShipList[ i ];

				alienShipModel.transform.SetPositionAndRotation( alienShip.m_coordinates, Quaternion.LookRotation( alienShip.m_direction, Vector3.up ) );

				var playerToShip = alienShip.m_coordinates - playerData.m_general.m_coordinates;

				xExtent = Mathf.Max( xExtent, Mathf.Abs( playerToShip.x ) );
				zExtent = Mathf.Max( zExtent, Mathf.Abs( playerToShip.z ) );
			}
		}

		// add some space around the extents
		xExtent += 192.0f;
		zExtent += 192.0f;

		// recalculate what the camera distance from the zero plane should be
		var verticalFieldOfView = m_spaceflightController.m_map.m_playerCamera.fieldOfView * Mathf.Deg2Rad;
		var horizontalFieldOfView = 2.0f * Mathf.Atan( Mathf.Tan( verticalFieldOfView * 0.5f ) * m_spaceflightController.m_map.m_playerCamera.aspect );
		var horizontalAngle = Mathf.Deg2Rad * ( 180.0f - 90.0f - horizontalFieldOfView * Mathf.Rad2Deg * 0.5f );
		var verticalAngle = Mathf.Deg2Rad * ( 180.0f - 90.0f - verticalFieldOfView * Mathf.Rad2Deg * 0.5f );
		var tanHorizontalAngle = Mathf.Tan( horizontalAngle );
		var tanVerticalAngle = Mathf.Tan( verticalAngle );

		var targetDollyDistance = Mathf.Max( xExtent * tanHorizontalAngle, zExtent * tanVerticalAngle, 1024.0f );

		// slowly dolly the camera
		m_currentDollyDistance = Mathf.Lerp( m_currentDollyDistance, targetDollyDistance, Time.deltaTime * 0.25f );

		m_spaceflightController.m_player.DollyCamera( m_currentDollyDistance );
	}

	// call this to hide the encounter stuff
	public void Hide()
	{
		if ( !gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the encounter location." );

		// hide the hyperspace objects
		gameObject.SetActive( false );
	}

	// call this to show the encounter stuff
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the encounter location." );

		// show the hyperspace objects
		gameObject.SetActive( true );

		// show the player (ship)
		m_spaceflightController.m_player.Show();

		// make sure the camera is at the right height above the zero plane
		m_currentDollyDistance = 1024.0f;

		m_spaceflightController.m_player.DollyCamera( m_currentDollyDistance );

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// move the ship to where we are in the encounter
		m_spaceflightController.m_player.transform.position = playerData.m_general.m_coordinates = playerData.m_general.m_lastEncounterCoordinates;

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

		// hide the radar
		m_spaceflightController.m_radar.Hide();

		// reset the encounter
		Reset();

		// add the alien ships to the encounter
		AddAlienShips( true );

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Encounter );

		// play the alarm sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Alarm );
	}

	void Reset()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the current encounter id
		var encounterId = playerData.m_general.m_currentEncounterId;

		// get to the encounter game data
		m_gdEncounter = gameData.m_encounterList[ encounterId ];

		// find the encounter in the player data (the list is continually sorted by distance so we have to search)
		foreach ( var encounter in playerData.m_encounterList )
		{
			if ( encounter.m_encounterId == encounterId )
			{
				m_pdEncounter = encounter;
				break;
			}
		}

		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		// reset all of the alien ships
		foreach ( var alienShip in alienShipList )
		{
			// this alien ship has not been added yet
			alienShip.m_addedToEncounter = false;
		}

		// inactivate all of the alien ship models
		foreach ( var alienShip in m_alienShipModelList )
		{
			alienShip.SetActive( false );
		}

		// allocate array for alien ship list
		m_alienShipList = new PD_AlienShip[ m_alienShipModelList.Length ];
	}

	// adds a number of alien ships to the encounter - up to the maximum allowed by the encounter
	void AddAlienShips( bool justEnteredEncounter )
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		// go through alien ship slots (up to the maximum allowed at once)
		for ( var alienShipIndex = 0; alienShipIndex < m_gdEncounter.m_maxNumShipsAtOnce; alienShipIndex++ )
		{
			// is this slot active right now?
			if ( m_alienShipModelList[ alienShipIndex ].activeInHierarchy )
			{
				// yes - skip it
				continue;
			}

			// no - go through all of the alien ships in the encounter and add the next one
			foreach ( var alienShip in alienShipList )
			{
				// has this alien ship already been added to the encounter?
				if ( alienShip.m_addedToEncounter )
				{
					// yes - skip it
					continue;
				}

				// is this alien ship dead?
				if ( alienShip.m_isDead )
				{
					// yes - skip it
					continue;
				}

				// generate a random position inside of a unit circle
				var randomPosition = Random.insideUnitCircle;

				Vector3 coordinates;

				if ( justEnteredEncounter )
				{
					// put alien ship in area approximately in the correct direction of approach
					coordinates = new Vector3( randomPosition.x, 0.0f, randomPosition.y ) * 256.0f + Vector3.Normalize( m_pdEncounter.m_currentCoordinates - playerData.m_general.m_lastHyperspaceCoordinates ) * 2048.0f;
				}
				else
				{
					// put alien ship in a random position on a circle around the player
					coordinates = Vector3.Normalize( new Vector3( randomPosition.x, 0.0f, randomPosition.y ) ) * 2048.0f;
				}

				// make alien ship face the center of the encounter space
				var direction = -Vector3.Normalize( coordinates );

				// update the position and direction of the alien ship
				alienShip.m_coordinates = coordinates;
				alienShip.m_direction = direction;

				// set the target coordinates to be the player
				alienShip.m_targetCoordinates = Vector3.zero;

				// clone the model
				Instantiate( m_alienShipModelTemplate[ alienShip.m_encounterTypeId ], Vector3.zero, Quaternion.Euler( -90.0f, 0.0f, 0.0f ), m_alienShipModelList[ alienShipIndex ].transform );

				// show the model
				m_alienShipModelList[ alienShipIndex ].SetActive( true );

				// remember the alien ship associated with this model
				m_alienShipList[ alienShipIndex ] = alienShip;

				// we are done adding an alien ship to this slot
				alienShip.m_addedToEncounter = true;
				break;
			}
		}
	}

	void MechanUpdate( PD_AlienShip alienShip, GameObject alienShipModel )
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// 1 in 150 chance target coordinates will change
		var randomNumber = Random.Range( 1, 150 );

		if ( randomNumber == 25 )
		{
			var randomCoordinates = Random.insideUnitCircle;

			alienShip.m_targetCoordinates = playerData.m_general.m_coordinates + Vector3.Normalize( new Vector3( randomCoordinates.x, 0.0f, randomCoordinates.y ) ) * 256.0f;
		}

		// steer the alien ship towards the target coordinates
		var desiredDirection = Vector3.Normalize( alienShip.m_targetCoordinates - alienShip.m_coordinates );

		alienShip.m_direction = Vector3.Slerp( alienShip.m_direction, desiredDirection, Time.deltaTime * m_alienShipTurnRate );

		// move the alien ship forward
		alienShip.m_coordinates += alienShip.m_direction * Time.deltaTime * m_alienShipSpeed;
	}
}
