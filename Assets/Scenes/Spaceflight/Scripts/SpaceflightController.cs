
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SpaceflightController : MonoBehaviour
{
	// constants
	const int c_numButtons = 6;

	// public stuff we want to set using the editor
	public Sprite m_buttonOffSprite;
	public Sprite m_buttonOnSprite;
	public Sprite m_buttonActiveSprite;
	public Image[] m_buttonImageList;
	public TextMeshProUGUI[] m_buttonLabelList;
	public TextMeshProUGUI m_currentOfficer;
	public TextMeshProUGUI m_messages;
	public TextMeshProUGUI m_countdown;
	public Camera m_camera;
	public GameObject m_map;
	public Animator m_dockingBayDoorTop;
	public Animator m_dockingBayDoorBottom;
	public ParticleSystem m_decompressionParticleSystem;
	public Image m_overlay;
	public GameObject m_player;
	public GameObject m_ship;

	// stuff shared by all the controllers
	public InputManager m_inputManager { get; private set; }
	public BasicSound m_basicSound { get; private set; }
	public UISoundController m_uiSoundController { get; protected set; }
	public bool m_inOrbit;
	public bool m_hasCurrentSenorReading;
	public bool m_inDockingBay;
	public bool m_inHyperspace;
	public bool m_justLaunched;

	// button functions
	public ButtonFunction[] m_buttonFunctionList;
	public ButtonFunction m_currentFunction;

	// private stuff we don't want the editor to see
	private int m_currentButtonIndex;
	private bool m_activatingButton;
	private float m_activatingButtonTimer;
	private float m_ignoreControllerTimer;

	// bridge button functions and labels
	private ButtonFunction[] m_bridgeButtonFunctions;
	private string[] m_bridgeButtonLabels;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the input manager
		m_inputManager = GetComponent<InputManager>();

		// get access to the basic sound
		m_basicSound = GetComponent<BasicSound>();

		// get access to the ui sound controller
		m_uiSoundController = GetComponent<UISoundController>();

		// create the six ship function buttons
		m_buttonFunctionList = new ButtonFunction[ c_numButtons ];

		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;
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
			// bridge button functions and labels
			m_bridgeButtonFunctions = new ButtonFunction[] { new CommandFunction(), new ScienceFunction(), new NavigationFunction(), new EngineeringFunction(), new CommunicationsFunction(), new MedicalFunction() };

			// turn off controller navigation of the UI
			EventSystem.current.sendNavigationEvents = false;

			// reset everything
			m_inOrbit = false;
			m_hasCurrentSenorReading = false;
			m_inDockingBay = true;
			m_inHyperspace = false;
			m_activatingButton = false;
			m_activatingButtonTimer = 0.0f;

			// reset the buttons to default
			RestoreBridgeButtons();

			// hide various objects
			m_countdown.gameObject.SetActive( false );
			m_decompressionParticleSystem.gameObject.SetActive( false );
			m_overlay.gameObject.SetActive( false );
			m_ship.SetActive( false );

			// show the docking bay doors (closed) if we just came from the spaceport
			m_dockingBayDoorTop.gameObject.SetActive( m_inDockingBay );
			m_dockingBayDoorBottom.gameObject.SetActive( m_inDockingBay );
		}
	}

	// this is called by unity every frame
	private void Update()
	{
		// check if we are activating the currently selected button
		if ( m_activatingButton )
		{
			// yes - so update the timer
			m_activatingButtonTimer += Time.deltaTime;

			// after a certain amount of time, execute the button function
			if ( m_activatingButtonTimer >= 0.35f )
			{
				// reset the activate button flag and timer
				m_activatingButton = false;
				m_activatingButtonTimer = 0.0f;

				// update the current function
				m_currentFunction = m_buttonFunctionList[ m_currentButtonIndex ];

				// execute the current button function
				m_currentFunction.Execute();

				// do the first update
				m_currentFunction.Update();
			}

			return;
		}

		// check if we have a current funciton
		if ( m_currentFunction != null )
		{
			// call update on it
			if ( m_currentFunction.Update() )
			{
				// the button did the update - don't let this update function do anything more
				return;
			}
		}

		// check if we moved the stick down
		if ( m_inputManager.m_south )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentButtonIndex < ( m_buttonFunctionList.Length - 1 ) )
				{
					m_currentButtonIndex++;

					UpdateButtonSprites();

					m_uiSoundController.Play( UISoundController.UISound.Click );
				}
			}
		}
		else if ( m_inputManager.m_north ) // check if we have moved the stick up
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentButtonIndex > 0 )
				{
					m_currentButtonIndex--;

					UpdateButtonSprites();

					m_uiSoundController.Play( UISoundController.UISound.Click );
				}
			}
		}
		else // we have centered the stick
		{
			m_ignoreControllerTimer = 0.0f;
		}

		// check if we have pressed the cancel button
		if ( m_inputManager.GetCancelDown() )
		{
			if ( m_currentFunction == null )
			{
				m_uiSoundController.Play( UISoundController.UISound.Error );
			}
			else
			{
				// cancel the current function
				m_currentFunction.Cancel();
			}
		}

		// check if we have pressed the fire button
		if ( m_inputManager.GetSubmitDown() )
		{
			if ( m_buttonFunctionList[ m_currentButtonIndex ] == null )
			{
				m_uiSoundController.Play( UISoundController.UISound.Error );
			}
			else
			{
				// set the activate button flag and reset the timer
				m_activatingButton = true;
				m_activatingButtonTimer = 0.0f;

				// update the button sprite for the currently selected button
				m_buttonImageList[ m_currentButtonIndex ].sprite = m_buttonActiveSprite;

				// play the activate sound
				m_uiSoundController.Play( UISoundController.UISound.Activate );
			}
		}
	}

	// restore the bridge buttons
	public void RestoreBridgeButtons()
	{
		// there is no current funciton
		m_currentFunction = null;

		// restore the bridge functions and labels
		UpdateButtonFunctions( m_bridgeButtonFunctions );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// change the current officer label to the name of the ship
		m_currentOfficer.text = "ISS " + playerData.m_shipConfiguration.m_name;

		// update the message
		if ( m_inDockingBay )
		{
			m_messages.text = "Ship computer activated.\r\nPre-launch procedures complete.\r\nStanding by to initiate launch.";
		}
		else if ( m_justLaunched )
		{
			m_messages.text = "Starport clear.\r\nStanding by to maneuver.";
		}
	}

	// update the button functions and labels and change the current button index
	public void UpdateButtonFunctions( ButtonFunction[] buttonFunctionList )
	{
		// go through all 6 buttons
		for ( int i = 0; i < c_numButtons; i++ )
		{
			if ( i < buttonFunctionList.Length )
			{
				m_buttonFunctionList[ i ] = buttonFunctionList[ i ];

				m_buttonLabelList[ i ].text = m_buttonFunctionList[ i ].GetButtonLabel();
			}
			else
			{
				m_buttonFunctionList[ i ] = null;

				m_buttonLabelList[ i ].text = "";
			}
		}

		// reset the current button index
		m_currentButtonIndex = 0;

		// update the button sprites
		UpdateButtonSprites();
	}

	// go through each button image and set it to the on or off or active button sprite depending on what is currently selected
	public void UpdateButtonSprites()
	{
		for ( int i = 0; i < c_numButtons; i++ )
		{
			m_buttonImageList[ i ].sprite = ( m_currentButtonIndex == i ) ? m_buttonOnSprite : m_buttonOffSprite;
		}
	}
}
