
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SpaceflightController : MonoBehaviour
{
	// the different locations
	public DockingBay m_dockingBay;
	public StarSystem m_starSystem;
	public InOrbit m_inOrbit;
	public Planetside m_planetside;
	public Disembarked m_disembarked;
	public Hyperspace m_hyperspace;
	public Encounter m_encounter;

	// controllers
	public ButtonController m_buttonController;
	public DisplayController m_displayController;

	// components
	public PlayerShip m_playerShip;
	public PlayerCamera m_playerCamera;
	public Viewport m_viewport;
	public Countdown m_countdown;
	public Radar m_radar;
	public Scanner m_scanner;
	public Starmap m_starmap;
	public Messages m_messages;
	public TerrainVehicle m_terrainVehicle;

	// some settings
	public float m_alienHyperspaceRadarDistance;
	public float m_alienStarSystemRadarDistance;
	public float m_encounterRange;
	public float m_planetRotationSpeed;

	// true if the game is paused
	public bool m_gameIsPaused;

	// save game timer
	float m_timer;

	// remember whether or not we have faded in the scene already
	bool m_alreadyFadedIn;

	// static instance to this spaceflight controller
	static public SpaceflightController m_instance;

	// constructor
	SpaceflightController()
	{
		// make me accessible to everyone
		m_instance = this;
	}

	// unity awake
	void Awake()
	{
		// check if we loaded the persistent scene
		if ( DataController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			DataController.m_sceneToLoad = "Spaceflight";

			// debug info
			Debug.Log( "Loading scene Persistent" );

			// load the persistent scene
			SceneManager.LoadScene( "Persistent" );
		}
	}

	// unity start
	void Start()
	{
		// hide everything
		HideEverything();

		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// show the player in case we had it hidden in the editor
		m_playerShip.Show();

		// reset the buttons to default
		m_buttonController.SetBridgeButtons();

		// switch to the current location
		SwitchLocation( playerData.m_general.m_location );

		// make sure the scene is blacked out
		SceneFadeController.m_instance.BlackOut();

		// reset the save game timer
		m_timer = 0.0f;

		// connect the persistent ui canvas to the main camera
		var persistentUI = GameObject.FindWithTag( "Persistent UI" );
		var uiCamera = GameObject.FindWithTag( "UI Camera" );
		var canvas = persistentUI.GetComponent<Canvas>();
		var camera = uiCamera.GetComponent<Camera>();
		canvas.worldCamera = camera;
		canvas.planeDistance = 15.0f;
	}

	// unity update
	void Update()
	{
		// Debug.Log( "Update()" );

		// are we generating planets?
		if ( m_starSystem.GeneratingPlanets() )
		{
			// yes - continue generating planets
			var totalProgress = m_starSystem.GeneratePlanets();

			// show / update the pop up dialog
			PopupController.m_instance.ShowPopup( "Commencing System Penetration", totalProgress );
		}
		else
		{
			// hide the pop up dialog
			PopupController.m_instance.HidePopup();

			// fade in the scene if we haven't already
			if ( !m_alreadyFadedIn )
			{
				// fade in the scene
				SceneFadeController.m_instance.FadeIn();

				// remember that we have faded in the scene
				m_alreadyFadedIn = true;
			}
		}

		// don't do anything if the game is paused
		if ( m_gameIsPaused )
		{
			return;
		}

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// are we in the star system or hyperspace locations?
		if ( ( playerData.m_general.m_location == PD_General.Location.StarSystem ) || ( playerData.m_general.m_location == PD_General.Location.Hyperspace ) )
		{
			// yes - update the game time
			playerData.m_general.UpdateGameTime( Time.deltaTime );
		}

		// save the game once in a while
		m_timer += Time.deltaTime;

		if ( m_timer >= 30.0f )
		{
			m_timer -= 30.0f;

			// DataController.m_instance.SaveActiveGame();
		}

		// when player hits cancel (esc) show the save game panel (except when using the starmap)
		if ( !m_starmap.IsOpen() )
		{
			if ( InputController.m_instance.m_cancel )
			{
				InputController.m_instance.Debounce();

				PanelController.m_instance.m_saveGamePanel.SetCallbackObject( this );

				PanelController.m_instance.Open( PanelController.m_instance.m_saveGamePanel );

				m_gameIsPaused = true;
			}
		}
	}

	// call this to hide everything
	void HideEverything()
	{
		// hide all of the locations
		m_dockingBay.Hide();
		m_starSystem.Hide();
		m_inOrbit.Hide();
		m_planetside.Hide();
		m_disembarked.Hide();
		m_hyperspace.Hide();
		m_encounter.Hide();

		// hide various components
		m_playerShip.Hide();
		m_countdown.Hide();
		m_radar.Hide();
		m_scanner.Hide();
		m_starmap.Hide();
	}

	// call this to switch to the correct location
	public void SwitchLocation( PD_General.Location newLocation )
	{
		// switch to the correct mode
		Debug.Log( "Switching to location " + newLocation );

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// are we switching to a new location?
		if ( playerData.m_general.m_location != newLocation )
		{
			// yes - remember the last location
			playerData.m_general.m_lastLocation = playerData.m_general.m_location;
		}

		// make sure the display is updated (in case we are loading from a save game)
		m_messages.Refresh();

		// update the player data
		playerData.m_general.m_location = newLocation;

		// stop all looping sounds
		SoundController.m_instance.StopAllLoopingSounds();

		// switching to starport is a special case
		if ( playerData.m_general.m_location == PD_General.Location.Starport )
		{
			// force shields to lower and weapons to disarm
			playerData.m_playerShip.DropShields();
			playerData.m_playerShip.DisarmWeapons();

			// start fading out the spaceflight scene
			SceneFadeController.m_instance.FadeOut( "Starport" );
		}
		else
		{
			// hide everything
			HideEverything();

			// make sure the map is visible
			m_viewport.StartFade( 1.0f, 2.0f );

			// switch the location
			switch ( playerData.m_general.m_location )
			{
				case PD_General.Location.DockingBay:
					m_dockingBay.Show();
					break;

				case PD_General.Location.JustLaunched:
					m_viewport.StartFade( 0.0f, 0.0f );
					m_messages.Clear();
					m_messages.AddText( "<color=white>Starport clear.\nStanding by to maneuver.</color>" );
					break;

				case PD_General.Location.StarSystem:
					m_starSystem.Initialize();
					m_starSystem.Show();
					m_playerShip.Show();
					break;

				case PD_General.Location.InOrbit:
					m_starSystem.Initialize();
					m_inOrbit.Show();
					break;

				case PD_General.Location.Planetside:
					m_starSystem.Initialize();
					m_planetside.Show();
					break;

				case PD_General.Location.Disembarked:
					m_starSystem.Initialize();
					m_disembarked.Show();
					break;

				case PD_General.Location.Hyperspace:
					m_hyperspace.Show();
					m_playerShip.Show();
					break;

				case PD_General.Location.Encounter:
					m_encounter.Show();
					m_playerShip.Show();
					break;
			}
		}

		// save the player data (since the location was most likely changed)
		// DataController.m_instance.SaveActiveGame();
	}
	
	// call this when a the save game panel was closed
	public void PanelWasClosed()
	{
		// save the player data in case something has been updated
		DataController.m_instance.SaveActiveGame();

		// unpause the game
		m_gameIsPaused = false;
	}

	// updates the encounters (call only from hyperspace or starsystem locations)
	public void UpdateEncounters()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the current player location
		var location = playerData.m_general.m_location;

		// get the current star id
		var starId = playerData.m_general.m_currentStarId;

		// get the coordinates
		var coordinates = playerData.m_general.m_coordinates;

		// get the correct alien radar distance
		var alienRadarDistance = ( location == PD_General.Location.Hyperspace ) ? m_alienHyperspaceRadarDistance : m_alienStarSystemRadarDistance;

		// go through each potential encounter
		foreach ( var encounter in playerData.m_encounterList )
		{
			// assume this encounter is in a different location
			encounter.SetDistance( float.MaxValue );

			// get the encounter location
			var encounterLocation = encounter.GetLocation();

			// orbit encounters are handled in maneuver
			if ( encounterLocation == PD_General.Location.InOrbit )
			{
				continue;
			}

			// are we in the star system location?
			if ( location == PD_General.Location.StarSystem )
			{
				// yes - is this encounter a star system encounter and in the same star system?
				if ( ( encounterLocation != PD_General.Location.StarSystem ) || (  starId != encounter.GetStarId() ) )
				{
					// no - skip it
					continue;
				}
			}
			else
			{
				// no - is this encounter a hyperspace encounter?
				if ( encounterLocation != PD_General.Location.Hyperspace )
				{
					// no - skip it
					continue;
				}
			}

			// calculate the distance from the player to the encounter
			var distance = encounter.CalculateDistance( coordinates );

			// can the aliens detect the player?
			if ( distance < alienRadarDistance )
			{
				// yes - move the aliens towards the player (at a fixed speed)
				encounter.MoveTowards( coordinates );
			}
			else
			{
				// no - move the aliens towards their home coordinates (at a fixed speed)
				encounter.GoHome();
			}

			// are the aliens and the player within encounter range?
			if ( distance < m_encounterRange )
			{
				// yes - save encounter information in the player data
				playerData.m_general.m_currentEncounterId = encounter.m_encounterId;

				// put the player in the middle of the encounter
				playerData.m_general.m_lastEncounterCoordinates = Vector3.zero;

				// let the encounter system know we are now entering this encounter
				m_encounter.JustEntered();

				// switch to the encounter location
				SwitchLocation( PD_General.Location.Encounter );
			}
		}
	}

#if UNITY_EDITOR

	// draw gizmos to help debug the game
	void OnDrawGizmos()
	{
		if ( DataController.m_instance == null )
		{
			return;
		}

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the current player location
		var location = playerData.m_general.m_location;

		// if we are in hyperspace draw the map grid
		if ( location == PD_General.Location.Hyperspace )
		{
			Gizmos.color = Color.green;

			for ( var x = 0; x < 250; x += 10 )
			{
				var start = Tools.GameToWorldCoordinates( new Vector3( x, 0.0f, 0.0f ) );
				var end = Tools.GameToWorldCoordinates( new Vector3( x, 0.0f, 250.0f ) );

				Gizmos.DrawLine( start, end );
			}

			for ( var z = 0; z < 250; z += 10 )
			{
				var start = Tools.GameToWorldCoordinates( new Vector3( 0.0f, 0.0f, z ) );
				var end = Tools.GameToWorldCoordinates( new Vector3( 250.0f, 0.0f, z ) );

				Gizmos.DrawLine( start, end );
			}
		}

		// if we are in hyperspace draw the flux paths
		if ( location == PD_General.Location.Hyperspace )
		{
			Gizmos.color = Color.cyan;

			foreach ( var flux in gameData.m_fluxList )
			{
				Gizmos.DrawLine( flux.GetFrom(), flux.GetTo() );
			}
		}

		// if we are in hyperspace draw the coordinates around the cursor point
		if ( location == PD_General.Location.Hyperspace )
		{
			var ray = UnityEditor.HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );

			var plane = new Plane( Vector3.up, 0.0f );

			float enter;

			if ( plane.Raycast( ray, out enter ) )
			{
				var worldCoordinates = ray.origin + ray.direction * enter;

				var gameCoordinates = Tools.WorldToGameCoordinates( worldCoordinates );

				UnityEditor.Handles.color = Color.blue;

				UnityEditor.Handles.Label( worldCoordinates + Vector3.forward * 64.0f + Vector3.right * 64.0f, Mathf.RoundToInt( gameCoordinates.x ) + "," + Mathf.RoundToInt( gameCoordinates.z ) );
			}
		}

		// if we are in either hyperspace or a star system then draw the alien encounters
		if ( ( location == PD_General.Location.StarSystem ) || ( location == PD_General.Location.Hyperspace ) )
		{
			// draw the encounter radius
			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawWireDisc( playerData.m_general.m_coordinates, Vector3.up, m_encounterRange );

			// draw the radar range
			UnityEditor.Handles.color = Color.magenta;

			if ( location == PD_General.Location.Hyperspace )
			{
				UnityEditor.Handles.DrawWireDisc( playerData.m_general.m_coordinates, Vector3.up, m_radar.m_maxHyperspaceDetectionDistance );
			}
			else
			{
				UnityEditor.Handles.DrawWireDisc( playerData.m_general.m_coordinates, Vector3.up, m_radar.m_maxStarSystemDetectionDistance );
			}

			// draw the positions on each encounter
			UnityEditor.Handles.color = Color.red;

			foreach ( var pdEncounter in playerData.m_encounterList )
			{
				var encounterLocation = pdEncounter.GetLocation();

				if ( ( encounterLocation == location ) && ( location == PD_General.Location.Hyperspace ) || ( pdEncounter.GetStarId() == playerData.m_general.m_currentStarId ) )
				{
					// get access to the encounter
					var gdEncounter = gameData.m_encounterList[ pdEncounter.m_encounterId ];

					// draw alien position
					UnityEditor.Handles.color = Color.red;
					UnityEditor.Handles.DrawWireDisc( pdEncounter.m_currentCoordinates, Vector3.up, 16.0f );

					// print alien race
					UnityEditor.Handles.Label( pdEncounter.m_currentCoordinates + Vector3.up * 16.0f, gdEncounter.m_race.ToString() );

					// draw the alien radar range
					UnityEditor.Handles.color = Color.yellow;

					if ( location == PD_General.Location.Hyperspace )
					{
						UnityEditor.Handles.DrawWireDisc( pdEncounter.m_currentCoordinates, Vector3.up, m_alienHyperspaceRadarDistance );
					}
					else
					{
						UnityEditor.Handles.DrawWireDisc( pdEncounter.m_currentCoordinates, Vector3.up, m_alienStarSystemRadarDistance );
					}
				}
			}
		}
	}

#endif // UNITY_EDITOR
}
