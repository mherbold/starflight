
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
	public TextMeshProUGUI m_currentOfficer;

	// buttons
	private ShipButton[] m_buttonList;
	private ShipButton m_currentButton;

	// private stuff we don't want the editor to see
	int m_currentButtonIndex;
	bool m_activatingButton;
	float m_activatingButtonTimer;
	float m_ignoreControllerTimer;

	// bridge buttons
	ShipButton[] m_bridgeButtons;

	// convenient access to the spaceflight controller
	SpaceflightController m_spaceflightController;

	// unity awake
	void Awake()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();

		// create the six ship buttons
		m_buttonList = new ShipButton[ c_numButtons ];

		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;

		// bridge buttons
		m_bridgeButtons = new ShipButton[] { new CommandButton(), new ScienceButton(), new NavigationButton(), new EngineeringButton(), new CommunicationsButton(), new MedicalButton() };
	}

	// unity start
	void Start()
	{
		// reset everything
		m_activatingButton = false;
		m_activatingButtonTimer = 0.0f;

		// reset the buttons to default
		RestoreBridgeButtons();
	}

	// unity update
	void Update()
	{
		// don't do anything if we have a panel open
		if ( PanelController.m_instance.HasActivePanel() )
		{
			return;
		}

		// don't do anything if we have a pop up dialog open
		if ( PopupController.m_instance.IsActive() )
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
				ShipButton activatedButton = m_buttonList[ m_currentButtonIndex ];

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
		if ( InputController.m_instance.m_south )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentButtonIndex < ( m_buttonList.Length - 1 ) )
				{
					m_currentButtonIndex++;

					UpdateButtonSprites();

					SoundController.m_instance.PlaySound( SoundController.Sound.Click );
				}
			}
		}
		else if ( InputController.m_instance.m_north ) // check if we have moved the stick up
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentButtonIndex > 0 )
				{
					m_currentButtonIndex--;

					UpdateButtonSprites();

					SoundController.m_instance.PlaySound( SoundController.Sound.Click );
				}
			}
		}
		else // we have centered the stick
		{
			m_ignoreControllerTimer = 0.0f;
		}

		// check if we have pressed the cancel button
		if ( InputController.m_instance.CancelWasPressed() )
		{
			if ( m_currentButton == null )
			{
				SoundController.m_instance.PlaySound( SoundController.Sound.Error );
			}
			else
			{
				// cancel the current button
				m_currentButton.Cancel();
			}
		}

		// check if we have pressed the fire button
		if ( InputController.m_instance.SubmitWasPressed() )
		{
			if ( m_buttonList[ m_currentButtonIndex ] == null )
			{
				SoundController.m_instance.PlaySound( SoundController.Sound.Error );
			}
			else
			{
				// set the activate button flag and reset the timer
				m_activatingButton = true;
				m_activatingButtonTimer = 0.0f;

				// update the button sprite for the currently selected button
				m_buttonImageList[ m_currentButtonIndex ].sprite = m_buttonActiveSprite;

				// play the activate sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
			}
		}
	}

	// call this to change the current officer text
	public void ChangeOfficerText( string newOfficer )
	{
		m_currentOfficer.text = newOfficer;
	}

	// restore the bridge buttons
	public void RestoreBridgeButtons()
	{
		// there is no current funciton
		m_currentButton = null;

		// restore the bridge buttons
		UpdateButtons( m_bridgeButtons );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// change the current officer label to the name of the ship
		ChangeOfficerText( "ISS " + playerData.m_playerShip.m_name );

		// update the message
		switch ( playerData.m_general.m_location )
		{
			case PD_General.Location.DockingBay:
				m_spaceflightController.m_messages.ChangeText( "<color=white>Ship computer activated.\nPre-launch procedures complete.\nStanding by to initiate launch.</color>" );
				break;

			case PD_General.Location.JustLaunched:
				m_spaceflightController.m_messages.ChangeText( "<color=white>Starport clear.\nStanding by to maneuver.</color>" );
				break;
		}
	}

	// update the buttons and change the current button index
	public void UpdateButtons( ShipButton[] buttonList )
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

	// call this to deactivate the current button
	public void DeactivateButton()
	{
		// we no longer have a current button
		m_currentButton = null;

		// remove the "active" dot from the current button
		UpdateButtonSprites();

		// play the deactivate sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
	}
}
