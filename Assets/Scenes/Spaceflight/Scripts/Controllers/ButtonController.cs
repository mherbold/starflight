
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
	// constants
	const int c_numButtons = 6;

	// public stuff we want to set using the editor
	public Sprite m_buttonOffSprite;
	public Sprite m_buttonOnSprite;
	public Sprite m_buttonActiveSprite;
	public Image[] m_buttonImageList;
	public TextMeshProUGUI[] m_buttonLabelList;

	// buttons
	private Button[] m_buttonList;
	private Button m_currentButton;

	// private stuff we don't want the editor to see
	private int m_currentButtonIndex;
	private bool m_activatingButton;
	private float m_activatingButtonTimer;
	private float m_ignoreControllerTimer;

	// bridge buttons
	private Button[] m_bridgeButtons;

	// convenient access to the spaceflight controller
	private SpaceflightController m_spaceflightController;

	// this is called by unity before start
	private void Awake()
	{
		// create the six ship buttons
		m_buttonList = new Button[ c_numButtons ];

		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();

		// stop here if the spaceflight contoller has not started
		if ( !m_spaceflightController.m_started )
		{
			return;
		}

		// bridge buttons
		m_bridgeButtons = new Button[] { new CommandButton(), new ScienceButton(), new NavigationButton(), new EngineeringButton(), new CommunicationsButton(), new MedicalButton() };

		// reset everything
		m_activatingButton = false;
		m_activatingButtonTimer = 0.0f;

		// reset the buttons to default
		RestoreBridgeButtons();
	}

	// this is called by unity every frame
	private void Update()
	{
		// stop here if the spaceflight contoller has not started
		if ( !m_spaceflightController.m_started )
		{
			return;
		}

		// check if we are activating the currently selected button
		if ( m_activatingButton )
		{
			// yes - so update the timer
			m_activatingButtonTimer += Time.deltaTime;

			// after a certain amount of time, execute the button
			if ( m_activatingButtonTimer >= 0.35f )
			{
				// reset the activate button flag and timer
				m_activatingButton = false;
				m_activatingButtonTimer = 0.0f;

				// get the activated button (execute might change this so grab it now)
				Button activatedButton = m_buttonList[ m_currentButtonIndex ];

				// execute the current button and check if it returned true
				if ( activatedButton.Execute() )
				{
					// update the current button
					m_currentButton = activatedButton;

					// do the first update
					m_currentButton.Update();
				}
			}

			return;
		}

		// check if we have a current funciton
		if ( m_currentButton != null )
		{
			// call update on it
			if ( m_currentButton.Update() )
			{
				// the button did the update - don't let this update do anything more
				return;
			}
		}

		// check if we moved the stick down
		if ( m_spaceflightController.m_inputManager.m_south )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentButtonIndex < ( m_buttonList.Length - 1 ) )
				{
					m_currentButtonIndex++;

					UpdateButtonSprites();

					m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Click );
				}
			}
		}
		else if ( m_spaceflightController.m_inputManager.m_north ) // check if we have moved the stick up
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentButtonIndex > 0 )
				{
					m_currentButtonIndex--;

					UpdateButtonSprites();

					m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Click );
				}
			}
		}
		else // we have centered the stick
		{
			m_ignoreControllerTimer = 0.0f;
		}

		// check if we have pressed the cancel button
		if ( m_spaceflightController.m_inputManager.GetCancelDown() )
		{
			if ( m_currentButton == null )
			{
				m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );
			}
			else
			{
				// cancel the current button
				m_currentButton.Cancel();
			}
		}

		// check if we have pressed the fire button
		if ( m_spaceflightController.m_inputManager.GetSubmitDown() )
		{
			if ( m_buttonList[ m_currentButtonIndex ] == null )
			{
				m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );
			}
			else
			{
				// set the activate button flag and reset the timer
				m_activatingButton = true;
				m_activatingButtonTimer = 0.0f;

				// update the button sprite for the currently selected button
				m_buttonImageList[ m_currentButtonIndex ].sprite = m_buttonActiveSprite;

				// play the activate sound
				m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Activate );
			}
		}
	}

	// restore the bridge buttons
	public void RestoreBridgeButtons()
	{
		// there is no current funciton
		m_currentButton = null;

		// restore the bridge buttons
		UpdateButtons( m_bridgeButtons );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// change the current officer label to the name of the ship
		m_spaceflightController.m_currentOfficer.text = "ISS " + playerData.m_shipConfiguration.m_name;

		// update the message
		if ( m_spaceflightController.m_inDockingBay )
		{
			m_spaceflightController.m_messages.text = "Ship computer activated.\r\nPre-launch procedures complete.\r\nStanding by to initiate launch.";
		}
		else if ( m_spaceflightController.m_justLaunched )
		{
			m_spaceflightController.m_messages.text = "Starport clear.\r\nStanding by to maneuver.";
		}
	}

	// update the buttons and change the current button index
	public void UpdateButtons( Button[] buttonList )
	{
		// go through all 6 buttons
		for ( int i = 0; i < c_numButtons; i++ )
		{
			if ( i < buttonList.Length )
			{
				m_buttonList[ i ] = buttonList[ i ];

				m_buttonLabelList[ i ].text = m_buttonList[ i ].GetLabel();
			}
			else
			{
				m_buttonList[ i ] = null;

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
