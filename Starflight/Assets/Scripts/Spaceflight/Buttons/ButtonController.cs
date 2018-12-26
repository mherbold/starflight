
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
	public enum ButtonSet
	{
		Bridge,
		CommandA,
		CommandB,
		CommunicationsA,
		CommunicationsB,
		Engineering,
		Medical,
		Navigation,
		Science,
		Land,
		Launch,
		Hail,
		Respond,
		Comm,
		AskQuestion,
		AnswerQuestion,
		Posture,
		Count
	};

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
	int m_selectedButtonIndex;
	bool m_activatingButton;
	float m_activatingButtonTimer;
	float m_ignoreControllerTimer;

	// button sets
	ShipButton[][] m_buttonSets;

	// the current button set
	ButtonSet m_currentButtonSet;

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

		// allocate memory for button sets
		m_buttonSets = new ShipButton[ (int) ButtonSet.Count ][];

		m_buttonSets[ (int) ButtonSet.Bridge ] = new ShipButton[] { new CommandButton(), new ScienceButton(), new NavigationButton(), new EngineeringButton(), new CommunicationsButton(), new MedicalButton() };
		m_buttonSets[ (int) ButtonSet.CommandA ] = new ShipButton[] { new LaunchButton(), new DisembarkButton(), new CargoButton(), new LogPlanetButton(), new ShipsLogButton(), new BridgeButton() };
		m_buttonSets[ (int) ButtonSet.CommandB ] = new ShipButton[] { new LandButton(), new DisembarkButton(), new CargoButton(), new LogPlanetButton(), new ShipsLogButton(), new BridgeButton() };
		m_buttonSets[ (int) ButtonSet.CommunicationsA ] = new ShipButton[] { new HailButton(), new DistressButton(), new BridgeButton() };
		m_buttonSets[ (int) ButtonSet.CommunicationsB ] = new ShipButton[] { new RespondButton(), new DistressButton(), new BridgeButton() };
		m_buttonSets[ (int) ButtonSet.Engineering ] = new ShipButton[] { new DamageButton(), new RepairButton(), new BridgeButton() };
		m_buttonSets[ (int) ButtonSet.Medical ] = new ShipButton[] { new ExamineButton(), new TreatButton(), new BridgeButton() };
		m_buttonSets[ (int) ButtonSet.Navigation ] = new ShipButton[] { new ManeuverButton(), new StarmapButton(), new RaiseShieldsButton(), new ArmWeaponButton(), new CombatButton(), new BridgeButton() };
		m_buttonSets[ (int) ButtonSet.Science ] = new ShipButton[] { new SensorsButton(), new AnalysisButton(), new StatusButton(), new BridgeButton() };
		m_buttonSets[ (int) ButtonSet.Land ] = new ShipButton[] { new SelectSiteButton(), new DescendButton(), new AbortButton() };
		m_buttonSets[ (int) ButtonSet.Launch ] = new ShipButton[] { new LaunchYesButton(), new LaunchNoButton() };
		m_buttonSets[ (int) ButtonSet.Hail ] = new ShipButton[] { new FriendlyButton(), new HostileButton(), new ObsequiousButton() };
		m_buttonSets[ (int) ButtonSet.Respond ] = new ShipButton[] { new FriendlyButton(), new HostileButton(), new ObsequiousButton() };
		m_buttonSets[ (int) ButtonSet.Comm ] = new ShipButton[] { new StatementButton(), new QuestionButton(), new PostureButton(), new TerminateButton() };
		m_buttonSets[ (int) ButtonSet.AskQuestion ] = new ShipButton[] { new ThemselvesButton(), new OtherRacesButton(), new OldEmpireButton(), new TheAncientsButton(), new GeneralInfoButton() };
		m_buttonSets[ (int) ButtonSet.AnswerQuestion ] = new ShipButton[] { new AnswerYesButton(), new AnswerNoButton(), new TerminateButton() };
		m_buttonSets[ (int) ButtonSet.Posture ] = new ShipButton[] { new FriendlyButton(), new HostileButton(), new ObsequiousButton() };
	}

	// unity start
	void Start()
	{
		// reset everything
		m_activatingButton = false;
		m_activatingButtonTimer = 0.0f;
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
				ShipButton activatedButton = m_buttonList[ m_selectedButtonIndex ];

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

				if ( m_selectedButtonIndex < ( m_buttonList.Length - 1 ) )
				{
					m_selectedButtonIndex++;

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

				if ( m_selectedButtonIndex > 0 )
				{
					m_selectedButtonIndex--;

					UpdateButtonSprites();

					SoundController.m_instance.PlaySound( SoundController.Sound.Click );
				}
			}
		}
		else // we have centered the stick
		{
			m_ignoreControllerTimer = 0.0f;
		}

		// check if we have pressed the fire button
		if ( InputController.m_instance.m_submit )
		{
			InputController.m_instance.Debounce();

			if ( m_buttonList[ m_selectedButtonIndex ] == null )
			{
				SoundController.m_instance.PlaySound( SoundController.Sound.Error );
			}
			else
			{
				// set the activate button flag and reset the timer
				m_activatingButton = true;
				m_activatingButtonTimer = 0.0f;

				// update the button sprite for the currently selected button
				m_buttonImageList[ m_selectedButtonIndex ].sprite = m_buttonActiveSprite;

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
		ClearCurrentButton();

		// restore the bridge buttons
		ChangeButtonSet( ButtonSet.Bridge );

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
	public void ChangeButtonSet( ButtonSet buttonSet )
	{
		// get the desired button set list
		var buttonList = m_buttonSets[ (int) buttonSet ];

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

		// update the current button set
		m_currentButtonSet = buttonSet;

		// reset the current button index
		m_selectedButtonIndex = 0;

		// update the button sprites
		UpdateButtonSprites();
	}

	// go through each button image and set it to the on or off or active button sprite depending on what is currently selected
	public void UpdateButtonSprites()
	{
		for ( int i = 0; i < c_numButtons; i++ )
		{
			m_buttonImageList[ i ].sprite = ( m_selectedButtonIndex == i ) ? m_buttonOnSprite : m_buttonOffSprite;
		}
	}

	// call this to stop calling update on the current button
	public void ClearCurrentButton()
	{
		m_currentButton = null;
	}

	// call this to deactivate the current button
	public void DeactivateButton()
	{
		// we no longer have a current button
		ClearCurrentButton();

		// remove the "active" dot from the current button
		UpdateButtonSprites();

		// play the deactivate sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
	}

	// gets the current button set
	public ButtonSet GetCurrentButtonSet()
	{
		return m_currentButtonSet;
	}
}