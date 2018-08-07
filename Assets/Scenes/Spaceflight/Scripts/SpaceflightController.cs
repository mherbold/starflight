
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
	public Image[] m_buttonImageList;
	public TextMeshProUGUI[] m_buttonLabelList;
	public TextMeshProUGUI m_currentOfficer;
	public TextMeshProUGUI m_messages;

	// stuff shared by all the controllers
	public InputManager m_inputManager { get; private set; }
	public UISoundController m_uiSoundController { get; protected set; }
	public bool m_inOrbit;
	public bool m_hasCurrentSenorReading;
	public bool m_inDockingBay;
	public bool m_inHyperspace;

	// button functions
	public ButtonFunction[] m_buttonFunctionList;
	public ButtonFunction m_currentFunction;

	// private stuff we don't want the editor to see
	private int m_currentButtonIndex;
	private float m_ignoreControllerTimer;

	// bridge button functions and labels
	private ButtonFunction[] m_bridgeButtonFunctions;
	private string[] m_bridgeButtonLabels;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the input manager
		m_inputManager = GetComponent<InputManager>();

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
			m_bridgeButtonFunctions = new ButtonFunction[] { new CommandFunction(), new ScienceFunction(), new NavigationFunction(), new EngineerFunction(), new CommunicationsFunction(), new MedicalFunction() };
			m_bridgeButtonLabels = new string[] { "Command", "Science", "Navigation", "Engineering", "Communications", "Medical" };

			// turn off controller navigation of the UI
			EventSystem.current.sendNavigationEvents = false;

			// reset everything
			m_inOrbit = false;
			m_hasCurrentSenorReading = false;
			m_inDockingBay = true;
			m_inHyperspace = false;

			// reset the buttons to default
			RestoreBridgeButtons();
		}
	}

	// this is called by unity every frame
	private void Update()
	{
		// get the controller stick position
		float y = m_inputManager.m_yRaw;

		// check if we moved the stick down
		if ( y <= -0.5f )
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
		else if ( y >= 0.5f ) // check if we have moved the stick up
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
				m_buttonFunctionList[ m_currentButtonIndex ].Cancel();
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
				// yep - execute the current button function
				m_buttonFunctionList[ m_currentButtonIndex ].Execute();
			}
		}
	}

	// go through each button image and set it to the on or off button sprite depending on what is currently selected
	private void UpdateButtonSprites()
	{
		for ( int i = 0; i < c_numButtons; i++ )
		{
			m_buttonImageList[ i ].sprite = ( m_currentButtonIndex == i ) ? m_buttonOnSprite : m_buttonOffSprite;
		}
	}

	// restore the bridge buttons
	public void RestoreBridgeButtons()
	{
		// there is no current funciton
		m_currentFunction = null;

		// restore the bridge functions and labels
		UpdateButtonList( m_bridgeButtonFunctions, m_bridgeButtonLabels );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// change the current officer label to the name of the ship
		m_currentOfficer.text = "ISS " + playerData.m_shipConfiguration.m_name;
	}

	// update the button functions and labels and change the current button index
	public void UpdateButtonList( ButtonFunction [] buttonFunctionList, string [] buttonLabelList )
	{
		// go through all 6 buttons
		for ( int i = 0; i < c_numButtons; i++ )
		{
			m_buttonFunctionList[ i ] = ( i < buttonFunctionList.Length ) ? buttonFunctionList[ i ] : null;
			m_buttonLabelList[ i ].text = ( i < buttonLabelList.Length ) ? buttonLabelList[ i ] : null;
		}

		// reset the current button index
		m_currentButtonIndex = 0;

		// update the button sprites
		UpdateButtonSprites();
	}
}