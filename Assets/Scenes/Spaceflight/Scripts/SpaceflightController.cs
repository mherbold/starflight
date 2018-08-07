
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SpaceflightController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public Sprite m_buttonOffSprite;
	public Sprite m_buttonOnSprite;
	public Image[] m_buttonImageList;
	public TextMeshProUGUI[] m_buttonLabelList;
	public TextMeshProUGUI m_currentOfficer;

	// stuff shared by all the controllers
	public InputManager m_inputManager { get; private set; }
	public UISoundController m_uiSoundController { get; protected set; }

	// ship function buttons
	public ShipFunction[] m_buttonFunctionList;
	public ShipFunction m_currentFunction;

	// private stuff we don't want the editor to see
	private int m_currentButtonIndex;
	private float m_ignoreControllerTimer;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the input manager
		m_inputManager = GetComponent<InputManager>();

		// get access to the ui sound controller
		m_uiSoundController = GetComponent<UISoundController>();

		// create the six ship function buttons
		m_buttonFunctionList = new ShipFunction[ 6 ];

		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;

		// check if we loaded the persistent scene
		if ( PersistentController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			PersistentController.m_sceneToLoad = "Spaceflight";

			SceneManager.LoadScene( "Persistent" );
		}

		// reset the buttons to default
		ResetButtonList( 0 );
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
			// yep - execute the current button function
			m_buttonFunctionList[ m_currentButtonIndex ].Execute();
		}
	}

	// go through each button image and set it to the on or off button sprite depending on what is currently selected
	private void UpdateButtonSprites()
	{
		for ( int i = 0; i < 6; i++ )
		{
			m_buttonImageList[ i ].sprite = ( m_currentButtonIndex == i ) ? m_buttonOnSprite : m_buttonOffSprite;
		}
	}

	// reset the function list to default
	public void ResetButtonList( int buttonIndex )
	{
		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// initialize the ship function buttons to startup state
		m_buttonFunctionList[ 0 ] = new CommandFunction();
		m_buttonFunctionList[ 1 ] = new ScienceFunction();
		m_buttonFunctionList[ 2 ] = new NavigationFunction();
		m_buttonFunctionList[ 3 ] = new EngineerFunction();
		m_buttonFunctionList[ 4 ] = new CommunicationsFunction();
		m_buttonFunctionList[ 5 ] = new MedicalFunction();

		// change the button labels
		m_buttonLabelList[ 0 ].text = "Command";
		m_buttonLabelList[ 1 ].text = "Science";
		m_buttonLabelList[ 2 ].text = "Navigation";
		m_buttonLabelList[ 3 ].text = "Engineering";
		m_buttonLabelList[ 4 ].text = "Communications";
		m_buttonLabelList[ 5 ].text = "Medical";

		// change the current button index
		m_currentButtonIndex = buttonIndex;

		// there is no current funciton
		m_currentFunction = null;

		// change the current officer label to the name of the ship
		m_currentOfficer.text = "ISS " + playerData.m_shipConfiguration.m_name;

		// update the button sprites
		UpdateButtonSprites();
	}
}