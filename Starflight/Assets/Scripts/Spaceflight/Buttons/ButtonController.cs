
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
		NavigationA,
		NavigationB,
		NavigationC,
		NavigationD,
		Science,
		Land,
		Launch,
		Hail,
		Respond,
		Comm,
		AskQuestion,
		AnswerQuestion,
		Posture,
		TerrainVehicle,
		ShipsLog,
		AlienComms,
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

	// unity awake
	void Awake()
	{
		// create the six ship buttons
		m_buttonList = new ShipButton[ c_numButtons ];

		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;

		// common bridge button
		var bridgeButton = new ChangeMenuButton( "Bridge", ButtonSet.Bridge );

		// allocate memory for button sets
		m_buttonSets = new ShipButton[ (int) ButtonSet.Count ][];

		m_buttonSets[ (int) ButtonSet.Bridge ] = new ShipButton[] { new CommandButton(), new ScienceButton(), new NavigationButton(), new EngineeringButton(), new CommunicationsButton(), new MedicalButton() };
		m_buttonSets[ (int) ButtonSet.CommandA ] = new ShipButton[] { new LaunchButton(), new DisembarkButton(), new ShipCargoButton(), new LogPlanetButton(), new ShipsLogButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.CommandB ] = new ShipButton[] { new LandButton(), new DisembarkButton(), new ShipCargoButton(), new LogPlanetButton(), new ShipsLogButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.CommunicationsA ] = new ShipButton[] { new HailButton(), new DistressButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.CommunicationsB ] = new ShipButton[] { new RespondButton(), new DistressButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.Engineering ] = new ShipButton[] { new DamageButton(), new RepairButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.Medical ] = new ShipButton[] { new ExamineButton(), new TreatButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.NavigationA ] = new ShipButton[] { new ManeuverButton(), new StarmapButton(), new RaiseShieldsButton(), new ArmWeaponButton(), new CombatButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.NavigationB ] = new ShipButton[] { new ManeuverButton(), new StarmapButton(), new DropShieldsButton(), new ArmWeaponButton(), new CombatButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.NavigationC ] = new ShipButton[] { new ManeuverButton(), new StarmapButton(), new RaiseShieldsButton(), new DisarmWeaponButton(), new CombatButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.NavigationD ] = new ShipButton[] { new ManeuverButton(), new StarmapButton(), new DropShieldsButton(), new DisarmWeaponButton(), new CombatButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.Science ] = new ShipButton[] { new SensorsButton(), new AnalysisButton(), new StatusButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.Land ] = new ShipButton[] { new SelectSiteButton(), new DescendButton(), new AbortButton() };
		m_buttonSets[ (int) ButtonSet.Launch ] = new ShipButton[] { new LaunchYesButton(), new LaunchNoButton() };
		m_buttonSets[ (int) ButtonSet.Hail ] = new ShipButton[] { new FriendlyButton(), new HostileButton(), new ObsequiousButton() };
		m_buttonSets[ (int) ButtonSet.Respond ] = new ShipButton[] { new FriendlyButton(), new HostileButton(), new ObsequiousButton() };
		m_buttonSets[ (int) ButtonSet.Comm ] = new ShipButton[] { new StatementButton(), new QuestionButton(), new PostureButton(), new TerminateButton() };
		m_buttonSets[ (int) ButtonSet.AskQuestion ] = new ShipButton[] { new QThemselvesButton(), new QOtherRacesButton(), new QOldEmpireButton(), new QTheAncientsButton(), new QGeneralInfoButton() };
		m_buttonSets[ (int) ButtonSet.AnswerQuestion ] = new ShipButton[] { new AnswerYesButton(), new AnswerNoButton(), new TerminateButton() };
		m_buttonSets[ (int) ButtonSet.Posture ] = new ShipButton[] { new FriendlyButton(), new HostileButton(), new ObsequiousButton() };
		m_buttonSets[ (int) ButtonSet.TerrainVehicle ] = new ShipButton[] { new MapButton(), new MoveButton(), new TVCargoButton(), new LookButton(), new ScanButton(), new WeaponButton() };
		m_buttonSets[ (int) ButtonSet.ShipsLog ] = new ShipButton[] { new StarportNoticesButton(), new AlienCommsButton(), new MessagesButton(), bridgeButton };
		m_buttonSets[ (int) ButtonSet.AlienComms ] = new ShipButton[] { new ACThemselvesButton(), new ACOtherRacesButton(), new ACOldEmpireButton(), new ACTheAncientsButton(), new ACGeneralInfoButton(), new ChangeMenuButton( "Cancel", ButtonSet.ShipsLog ) };
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
		// don't do anything if the game is paused
		if ( SpaceflightController.m_instance.m_gameIsPaused )
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

				ActivateButton();
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
					if ( m_buttonList[ m_selectedButtonIndex + 1 ] != null )
					{
						m_selectedButtonIndex++;

						UpdateButtonSprites();

						SoundController.m_instance.PlaySound( SoundController.Sound.Click );
					}
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

	// set the bridge buttons
	public void SetBridgeButtons()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// there is no current funciton
		ClearCurrentButton();

		// restore the bridge buttons
		ChangeButtonSet( ButtonSet.Bridge );

		// change the current officer label to the name of the ship
		ChangeOfficerText( "ISS " + playerData.m_playerShip.m_name );

		// update the message
		switch ( playerData.m_general.m_location )
		{
			case PD_General.Location.DockingBay:
				SpaceflightController.m_instance.m_messages.Clear();
				SpaceflightController.m_instance.m_messages.AddText( "<color=white>Ship computer activated.\nPre-launch procedures complete.\nStanding by to initiate launch.</color>" );
				break;

			case PD_General.Location.JustLaunched:
				SpaceflightController.m_instance.m_messages.Clear();
				SpaceflightController.m_instance.m_messages.AddText( "<color=white>Starport clear.\nStanding by to maneuver.</color>" );
				break;
		}
	}

	// set the maneuver buttons
	public void SetManeuverButtons()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// button set to use depends on whether we have shields up and/or weapons armed
		if ( playerData.m_playerShip.m_shieldsAreUp && playerData.m_playerShip.m_weaponsAreArmed )
		{
			SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonSet.NavigationD );
		}
		else if ( playerData.m_playerShip.m_weaponsAreArmed )
		{
			SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonSet.NavigationC );
		}
		else if ( playerData.m_playerShip.m_shieldsAreUp )
		{
			SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonSet.NavigationB );
		}
		else
		{
			SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonSet.NavigationA );
		}

		// get the personnel file on this officer
		var personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.Navigator );

		// set the name of the officer
		SpaceflightController.m_instance.m_buttonController.ChangeOfficerText( "Officer " + personnelFile.m_name );
	}

	// update the buttons and change the current button index
	public void ChangeButtonSet( ButtonSet buttonSet )
	{
		// there is no current funciton
		ClearCurrentButton();

		// get the desired button set list
		var buttonList = m_buttonSets[ (int) buttonSet ];

		// go through all 6 buttons
		for ( var i = 0; i < c_numButtons; i++ )
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
		for ( var i = 0; i < c_numButtons; i++ )
		{
			m_buttonImageList[ i ].sprite = ( m_selectedButtonIndex == i ) ? m_buttonOnSprite : m_buttonOffSprite;
		}
	}

	// call this to stop calling update on the current button
	public void ClearCurrentButton()
	{
		m_currentButton = null;
	}

	// call this to change the button that is selected
	public void SetSelectedButton( int buttonIndex )
	{
		ClearCurrentButton();
		
		m_selectedButtonIndex = buttonIndex;

		UpdateButtonSprites();

		m_buttonImageList[ m_selectedButtonIndex ].sprite = m_buttonActiveSprite;
	}

	// call this to activate the selected button
	public void ActivateButton()
	{
		// get the activated button (execute might change this so grab it now)
		var activatedButton = m_buttonList[ m_selectedButtonIndex ];

		// execute the current button and check if it returned true
		if ( activatedButton.Execute() )
		{
			// update the current button
			m_currentButton = activatedButton;

			// do the first update
			m_currentButton.Update();
		}
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