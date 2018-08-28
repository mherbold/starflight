
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SpaceflightController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public TextMeshProUGUI m_messages;
	public TextMeshProUGUI m_countdown;
	public Camera m_camera;
	public GameObject m_map;
	public Animator m_dockingBayDoorTop;
	public Animator m_dockingBayDoorBottom;
	public ParticleSystem m_decompressionParticleSystem;
	public RawImage m_mapRawImage;
	public GameObject m_player;
	public GameObject m_ship;
	public GameObject m_star;
	public GameObject m_lensFlare;
	public TextMeshProUGUI m_currentOfficer;
	public bool m_skipCinematics;

	// controllers
	public ButtonController m_buttonController { get; protected set; }
	public DisplayController m_displayController { get; protected set; }
	public SystemController m_systemController { get; protected set; }

	// stuff shared by all the controllers
	public bool m_inOrbit;
	public bool m_hasCurrentSenorReading;
	public bool m_inDockingBay;
	public bool m_inHyperspace;
	public bool m_justLaunched;

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

		// reset everything
		m_inOrbit = false;
		m_hasCurrentSenorReading = false;
		m_inDockingBay = true;
		m_inHyperspace = false;

		// hide various objects
		m_countdown.gameObject.SetActive( false );
		m_decompressionParticleSystem.gameObject.SetActive( false );
		m_ship.SetActive( false );
		m_star.SetActive( false );
		m_lensFlare.SetActive( false );

		// show the docking bay doors (closed) if we just came from the spaceport
		m_dockingBayDoorTop.gameObject.SetActive( m_inDockingBay );
		m_dockingBayDoorBottom.gameObject.SetActive( m_inDockingBay );

		// start playing music
		if ( m_inDockingBay )
		{
			MusicController.m_instance.ChangeToTrack( MusicController.Track.DockingBay );
		}
		else
		{
			MusicController.m_instance.ChangeToTrack( MusicController.Track.StarSystem );
		}

		// fade in the scene
		SceneFadeController.m_instance.FadeIn();
	}

	// this is called by unity every frame
	private void Update()
	{
		// update the game time
		PlayerData playerData = DataController.m_instance.m_playerData;

		playerData.m_starflight.UpdateGameTime( Time.deltaTime );
	}
}
