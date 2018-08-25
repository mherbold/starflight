
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
	public GameObject m_lensFlare;
	public GameObject m_player;
	public GameObject m_ship;
	public TextMeshProUGUI m_currentOfficer;
	public bool m_skipCinematics;

	// controllers
	public InputManager m_inputManager { get; private set; }
	public BasicSound m_basicSound { get; private set; }
	public UISoundController m_uiSoundController { get; protected set; }
	public ButtonController m_buttonController { get; protected set; }
	public DisplayController m_displayController { get; protected set; }
	public SystemController m_systemController { get; protected set; }

	// stuff shared by all the controllers
	public bool m_started;
	public bool m_inOrbit;
	public bool m_hasCurrentSenorReading;
	public bool m_inDockingBay;
	public bool m_inHyperspace;
	public bool m_justLaunched;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the various controllers
		m_inputManager = GetComponent<InputManager>();
		m_basicSound = GetComponent<BasicSound>();
		m_uiSoundController = GetComponent<UISoundController>();
		m_buttonController = GetComponent<ButtonController>();
		m_displayController = GetComponent<DisplayController>();
		m_systemController = GetComponent<SystemController>();
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		// check if we loaded the persistent scene
		if ( PersistentController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			PersistentController.m_sceneToLoad = "Spaceflight";

			SceneManager.LoadScene( "Persistent" );
		}
		else
		{
			// turn off controller navigation of the UI
			EventSystem.current.sendNavigationEvents = false;

			// reset everything
			m_started = true;
			m_inOrbit = false;
			m_hasCurrentSenorReading = false;
			m_inDockingBay = true;
			m_inHyperspace = false;

			// hide various objects
			m_countdown.gameObject.SetActive( false );
			m_decompressionParticleSystem.gameObject.SetActive( false );
			m_ship.SetActive( false );
			m_lensFlare.SetActive( false );

			// show the docking bay doors (closed) if we just came from the spaceport
			m_dockingBayDoorTop.gameObject.SetActive( m_inDockingBay );
			m_dockingBayDoorBottom.gameObject.SetActive( m_inDockingBay );
		}
	}

	// this is called by unity every frame
	private void Update()
	{
		// stop here if the spaceflight contoller has not started
		if ( !m_started )
		{
			return;
		}

		// update the game time
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		playerData.m_starflight.UpdateGameTime( Time.deltaTime );
	}
}
