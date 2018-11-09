
using UnityEngine;

public class Encounter : MonoBehaviour
{
	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// alien ship models (need 8)
	public GameObject[] m_alienShipList;

	// template models that we will clone as needed (need 23)
	public GameObject[] m_alienShipTemplate;

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
		m_spaceflightController.m_player.DollyCamera( 1024.0f );

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

		// add the alien ships to the encounter
		AddAlienShips();

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Encounter );

		// play the alarm sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Alarm );
	}

	// adds a number of alien ships to the encounter - up to the maximum allowed by the encounter
	void AddAlienShips()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the current encounter id
		var encounterId = playerData.m_general.m_currentEncounterId;

		// get to the list of alien ships
		var alienShipList = playerData.m_encounterList[ encounterId ].GetAlienShipList();

		// get the encounter game data
		var encounter = gameData.m_encounterList[ encounterId ];

		// we currently have no ships in this encounter
		var alienShipCount = 0;

		// go through all of the alien ships in the encounter and add them, up to the maximum allowed
		foreach ( var alienShip in alienShipList )
		{
			// is this ship dead?
			if ( alienShip.m_isDead )
			{
				// yes - skip it
				continue;
			}

			// LEFT OFF HERE

			// up the ship count
			alienShipCount++;

			// are we at the limit?
			if ( alienShipCount == encounter.m_maxNumShipsAtOnce )
			{
				// yes - ok stop now
				break;
			}
		}
	}
}
