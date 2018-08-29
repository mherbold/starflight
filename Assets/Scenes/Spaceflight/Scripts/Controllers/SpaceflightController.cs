
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SpaceflightController : MonoBehaviour
{
	// text elements
	public TextMeshProUGUI m_messages;
	public TextMeshProUGUI m_countdown;
	public TextMeshProUGUI m_currentOfficer;

	// the main camera
	public Camera m_camera;

	// the map ui object
	public RawImage m_map;

	// the docking bay doors
	public Animator m_dockingBayDoorTop;
	public Animator m_dockingBayDoorBottom;

	// particle systems
	public ParticleSystem m_decompressionParticleSystem;
	public InfiniteStarfield m_infiniteStarfield;

	// various game objects
	public GameObject m_player;
	public GameObject m_ship;
	public GameObject m_star;
	public GameObject m_lensFlare;

	// set this to true to skip cinematics
	public bool m_skipCinematics;

	// controllers
	public ButtonController m_buttonController { get; protected set; }
	public DisplayController m_displayController { get; protected set; }
	public SystemController m_systemController { get; protected set; }

	// save game timer
	float m_timer;

	// unity awake
	void Awake()
	{
		// check if we loaded the persistent scene
		if ( DataController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			DataController.m_sceneToLoad = "Spaceflight";

			SceneManager.LoadScene( "Persistent" );
		}
		else
		{
			// get access to the various controllers
			m_buttonController = GetComponent<ButtonController>();
			m_displayController = GetComponent<DisplayController>();
			m_systemController = GetComponent<SystemController>();
		}
	}

	// unity start
	void Start()
	{
		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;

		// update the game time
		//PlayerData playerData = DataController.m_instance.m_playerData;

		// temporary
		//playerData.m_starflight.m_location = Starflight.Location.DockingBay;

		// switch to the correct mode
		SwitchMode();

		// fade in the scene
		SceneFadeController.m_instance.FadeIn();

		// reset the save game timer
		m_timer = 0.0f;
	}

	// unity update
	void Update()
	{
		// update the game time
		PlayerData playerData = DataController.m_instance.m_playerData;

		playerData.m_starflight.UpdateGameTime( Time.deltaTime );

		// save the game once in a while
		m_timer += Time.deltaTime;

		if ( m_timer >= 30.0f )
		{
			m_timer -= 30.0f;

			DataController.m_instance.SavePlayerData();
		}
	}

	// this is called by each switch function
	void HideAllObjects()
	{
		// hide various objects that aren't used by every mode
		m_countdown.gameObject.SetActive( false );
		m_dockingBayDoorTop.gameObject.SetActive( false );
		m_dockingBayDoorBottom.gameObject.SetActive( false );
		m_decompressionParticleSystem.gameObject.SetActive( false );
		m_ship.SetActive( false );
		m_star.SetActive( false );
		m_lensFlare.SetActive( false );

		// make sure the map is visible
		m_map.color = new Color( 1.0f, 1.0f, 1.0f );
	}

	// call this to switch to the correct mode
	public void SwitchMode()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// switch to the correct mode
		Debug.Log( "Switching to " + playerData.m_starflight.m_location );

		switch ( playerData.m_starflight.m_location )
		{
			case Starflight.Location.DockingBay:
				SwitchToDockingBay();
				break;
			case Starflight.Location.JustLaunched:
				SwitchToJustLaunched();
				break;
			case Starflight.Location.StarSystem:
				SwitchToStarSystem();
				break;
			case Starflight.Location.Hyperspace:
				SwitchToHyperspace();
				break;
		}

		// save the player data (since the location was most likely changed)
		DataController.m_instance.SavePlayerData();
	}

	// call this to switch to the docking bay
	public void SwitchToDockingBay()
	{
		HideAllObjects();

		// show the docking bay doors
		m_dockingBayDoorTop.gameObject.SetActive( true );
		m_dockingBayDoorBottom.gameObject.SetActive( true );

		// play the docking bay music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.DockingBay );
	}

	// call this to switch to the just launched mode
	public void SwitchToJustLaunched()
	{
		HideAllObjects();

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// update the system controller
		m_systemController.EnterSystem( playerData.m_starflight.m_currentStarId );

		// make sure the map is NOT visible
		m_map.color = new Color( 0.0f, 0.0f, 0.0f );
	}

	// call this to switch to the star system
	public void SwitchToStarSystem()
	{
		HideAllObjects();

		// show various objects
		m_ship.SetActive( true );
		m_star.SetActive( true );
		m_lensFlare.SetActive( true );

		// make sure the camera is at the right height above the zero plane
		Vector3 cameraPosition = new Vector3( 0.0f, 1500.0f, 0.0f );
		m_camera.transform.localPosition = cameraPosition;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// move the player object
		m_player.transform.position = playerData.m_starflight.m_systemCoordinates;

		// update the system controller
		m_systemController.EnterSystem( playerData.m_starflight.m_currentStarId );

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.StarSystem );
	}

	// call this to switch to hyperspace
	public void SwitchToHyperspace()
	{
		HideAllObjects();

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Hyperspace );
	}

	// call this to update the player's position
	public void UpdatePlayerPosition( Vector3 newPosition )
	{
		// update the game object
		m_player.transform.position = newPosition;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// update the player data (it will save out to disk eventually)
		if ( playerData.m_starflight.m_location != Starflight.Location.Hyperspace )
		{
			playerData.m_starflight.m_systemCoordinates = newPosition;
		}
		else
		{
			playerData.m_starflight.m_hyperspaceCoordinates = newPosition;
		}
	}
}
