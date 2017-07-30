
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PersonnelController : PanelController
{
	enum State
	{
		ViewFile, SelectRace, GiveName, DeleteCrewmember, TrainCrewmember
	}

	enum GameObjects
	{
		SkillValuesText, RaceNameText, BioValuesText, VitalityText, BankBalanceText, TrainingText, HumanImage, VeloxImage, ThrynnImage, ElowanImage, AndroidImage, LeftArrowImage, RightArrowImage, TrainButton, DeleteButton, SelectButton, CancelButton, OverlayPanel, NamePanel, DeletePanel, SelectionXform, UpArrowImage, DownArrowImage, GameObjectCount
	}

	enum Buttons
	{
		PreviousButton, NextButton, CreateButton, DeleteButton, TrainButton, ExitButton, SelectButton, CancelButton, ButtonCount
	}

	// public stuff we want to set using the editor
	public TextMeshProUGUI m_fileNumberText;
	public TextMeshProUGUI m_skillValuesText;
	public TextMeshProUGUI m_raceNameText;
	public TextMeshProUGUI m_bioValuesText;
	public TextMeshProUGUI m_vitalityText;
	public TextMeshProUGUI m_bankBalanceText;
	public TextMeshProUGUI m_trainingText;
	public Image m_humanImage;
	public Image m_veloxImage;
	public Image m_thrynnImage;
	public Image m_elowanImage;
	public Image m_androidImage;
	public Image m_leftArrowImage;
	public Image m_rightArrowImage;
	public Image m_upArrowImage;
	public Image m_downArrowImage;
	public Button m_previousButton;
	public Button m_nextButton;
	public Button m_createButton;
	public Button m_deleteButton;
	public Button m_trainButton;
	public Button m_exitButton;
	public Button m_selectButton;
	public Button m_cancelButton;
	public Button m_yesButton;
	public Button m_noButton;
	public GameObject m_overlayPanel;
	public GameObject m_namePanel;
	public GameObject m_deletePanel;
	public GameObject m_selectionXform;
	public InputField m_nameInputField;

	// private stuff we don't want the editor to see
	private StarportController m_starportController;
	private InputManager m_inputManager;
	private State m_currentState;
	private int m_currentFileIndex;
	private int m_currentRaceIndex;
	private int m_currentSkillIndex;
	private int m_startingBankBalance;
	private Vector3 m_baseSelectionOffsetMin;
	private Vector3 m_baseSelectionOffsetMax;
	private float m_ignoreControllerTimer;
	//private bool m_haveFocus;

	// constant values
	private const int c_numRaces = 5;
	private const int c_numSkills = 5;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the starport controller
		m_starportController = GetComponent<StarportController>();

		// get access to the input manager
		m_inputManager = GetComponent<InputManager>();

		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;

		// remember the base selection offset
		RectTransform rectTransform = m_selectionXform.GetComponent<RectTransform>();
		m_baseSelectionOffsetMin = rectTransform.offsetMin;
		m_baseSelectionOffsetMax = rectTransform.offsetMax;

		// we don't have the focus
		//m_haveFocus = false;
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		// hide the ui
		m_panelGameObject.SetActive( false );
	}

	// this is called by unity every frame
	private void Update()
	{
		// update the ignore controller timer
		m_ignoreControllerTimer = Mathf.Max( 0.0f, m_ignoreControllerTimer - Time.deltaTime );

		// call the proper update function based on our current state
		switch ( m_currentState )
		{
			case State.SelectRace:
			{
				UpdateControllerForSelectRaceState();
				break;
			}

			case State.TrainCrewmember:
			{
				UpdateControllerForTrainCrewmemberState();
				break;
			}
		}
	}

	// controller updates for when we are currently selecting a race
	public void UpdateControllerForSelectRaceState()
	{
		// get the controller stick position
		float x = m_inputManager.GetRawX();

		// check if we moved the stick left
		if ( x <= -0.5f )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				m_currentRaceIndex = ( m_currentRaceIndex + c_numRaces - 1 ) % c_numRaces;

				UpdateScreen();

				GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
			}
		}
		else if ( x >= 0.5f ) // check if we moved the stick right
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				m_currentRaceIndex = ( m_currentRaceIndex + 1 ) % c_numRaces;

				UpdateScreen();

				GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
			}
		}
		else // we have centered the stick
		{
			m_ignoreControllerTimer = 0.0f;
		}
	}

	// controller updates for when we are currently selecting a race
	public void UpdateControllerForTrainCrewmemberState()
	{
		// get the controller stick position
		float y = m_inputManager.GetRawY();

		// check if we moved the stick down
		if ( y <= -0.5f )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentSkillIndex < ( c_numSkills - 1 ) )
				{
					m_currentSkillIndex++;

					UpdateTrainingText( 0 );

					UpdateScreen();

					GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
				}
			}
		}
		else if ( y >= 0.5f ) // check if we have moved the stick up
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentSkillIndex > 0 )
				{
					m_currentSkillIndex--;

					UpdateTrainingText( 0 );

					UpdateScreen();

					GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
				}
			}
		}
		else // we have centered the stick
		{
			m_ignoreControllerTimer = 0.0f;
		}

		// check if we have pressed the fire button
		if ( m_inputManager.GetSubmitDown() )
		{
			TrainSelectedSkill();
		}

		// check if we have pressed the cancel button
		if ( m_inputManager.GetCancelDown() )
		{
			SwitchToViewFileState();

			GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
		}
	}

	// call this to show the personnel ui
	public override void Show()
	{
		// reset the current state
		m_currentState = State.ViewFile;

		// view the first file
		m_currentFileIndex = 0;

		// remember the starting bank balance
		m_startingBankBalance = PersistentController.m_instance.m_playerData.m_bankPlayerData.m_currentBalance;

		// update the ui
		UpdateScreen();

		// start the opening animation
		StartOpeningUI();
	}

	// call this to hide the personnel ui
	public override void Hide()
	{
		// lose the focus
		LoseFocus();

		// start the closing animation
		StartClosingUI();

		// if the bank balance has changed then record it in the bank transaction log
		int deltaBalance = m_startingBankBalance - PersistentController.m_instance.m_playerData.m_bankPlayerData.m_currentBalance;

		if ( deltaBalance > 0 )
		{
			BankPlayerData.Transaction transaction = new BankPlayerData.Transaction( PersistentController.m_instance.m_playerData.m_starflightPlayerData.m_currentStardate, "Personnel", deltaBalance.ToString() + "-" );
			PersistentController.m_instance.m_playerData.m_bankPlayerData.m_transactionList.Add( transaction );
			PersistentController.m_instance.SavePlayerData();
		}
	}

	// call this to take control
	public void TakeFocus()
	{
		// we have the controller focus
		//m_haveFocus = true;

		// turn on controller navigation of the UI
		EventSystem.current.sendNavigationEvents = true;

		// switch to the default view
		SwitchToViewFileState();

		// cancel the ui sounds
		GetComponent<UISoundController>().CancelSounds();
	}

	// call this to give up control
	public void LoseFocus()
	{
		// we have the controller focus
		//m_haveFocus = false;

		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;
	}

	// this is called when the ui has finished animating to the open state
	public override void FinishedOpeningUI()
	{
		// take the focus
		TakeFocus();
	}

	// this is called when the ui has finished animating to the close state
	public override void FinishedClosingUI()
	{
		// give the focus back to the starport controller
		m_starportController.TakeFocus();
	}

	// call this to switch to the view file state
	private void SwitchToViewFileState()
	{
		// change the current state
		m_currentState = State.ViewFile;

		// update the screen
		UpdateScreen();

		// select the create button or the exit button if the create button is not enabled
		if ( m_createButton.interactable )
		{
			m_createButton.Select();
		}
		else
		{
			m_exitButton.Select();
		}
	}

	// call this to switch to the select race state
	private void SwitchToSelectRaceState()
	{
		// change the current state
		m_currentState = State.SelectRace;

		// start with the human
		m_currentRaceIndex = 0;

		// update the screen
		UpdateScreen();

		// select the select button by default
		m_selectButton.Select();
	}

	// call this to switch to the give name state
	private void SwitchToGiveNameState()
	{
		// change the current state
		m_currentState = State.GiveName;

		// update the screen
		UpdateScreen();

		// erase the current text input
		m_nameInputField.text = "";

		// select the text input by default
		m_nameInputField.Select();
	}

	// call this to switch to the delete crewmember state
	private void SwitchToDeleteCrewmemberState()
	{
		// change the current state
		m_currentState = State.DeleteCrewmember;

		//  update the screen
		UpdateScreen();

		// select the no button by default
		m_noButton.Select();
	}

	// call this to switch to the train crewmember state
	private void SwitchToTrainCrewmemberState()
	{
		// change the current state
		m_currentState = State.TrainCrewmember;

		// select the first skill by default
		m_currentSkillIndex = 0;

		// update the screen
		UpdateScreen();

		// debounce the buttons
		m_inputManager.DebounceNextUpdate();
	}

	// call this whenever we change state or do something that would result in something changing on the screen
	private void UpdateScreen()
	{
		// reset game objects and buttons
		bool[] gameObjectIsVisible = new bool[ (int) GameObjects.GameObjectCount ];
		bool[] buttonIsInteractable = new bool[ (int) Buttons.ButtonCount ];

		bool showPersonnelFile = false;

		switch ( m_currentState )
		{
			// we're not doing anything particular at the moment
			case State.ViewFile:
			{
				showPersonnelFile = UpdateScreenForViewFileState( gameObjectIsVisible, buttonIsInteractable );
				break;
			}

			// we are currently selecting a race
			case State.SelectRace:
			{
				showPersonnelFile = UpdateScreenForSelectRaceState( gameObjectIsVisible, buttonIsInteractable );
				break;
			}

			// we are currently giving a name
			case State.GiveName:
			{
				showPersonnelFile = UpdateScreenForGiveNameState( gameObjectIsVisible, buttonIsInteractable );
				break;
			}

			// we are currently deleting a crewmember
			case State.DeleteCrewmember:
			{
				showPersonnelFile = UpdateScreenForDeleteCrewmemberState( gameObjectIsVisible, buttonIsInteractable );
				break;
			}

			// we are currently selecting a skill to train
			case State.TrainCrewmember:
			{
				showPersonnelFile = UpdateScreenForTrainCrewmemberState( gameObjectIsVisible, buttonIsInteractable );
				break;
			}
		}

		// show the personnel file if we want to
		if ( showPersonnelFile )
		{
			ShowRace( gameObjectIsVisible );

			gameObjectIsVisible[ (int) GameObjects.SkillValuesText ] = true;
			gameObjectIsVisible[ (int) GameObjects.RaceNameText ] = true;
			gameObjectIsVisible[ (int) GameObjects.BioValuesText ] = true;
		}

		// if the previous button is currently selected and we are going to disable it, select the exit button
		if ( !buttonIsInteractable[ (int) Buttons.PreviousButton ] )
		{
			if ( EventSystem.current.currentSelectedGameObject == m_previousButton.gameObject )
			{
				m_exitButton.Select();
			}
		}

		// if the next button is currently selected and we are going to disable it, select the exit button
		if ( !buttonIsInteractable[ (int) Buttons.NextButton ] )
		{
			if ( EventSystem.current.currentSelectedGameObject == m_nextButton.gameObject )
			{
				m_exitButton.Select();
			}
		}

		// enable or disable buttons now
		m_previousButton.interactable = buttonIsInteractable[ (int) Buttons.PreviousButton ];
		m_nextButton.interactable = buttonIsInteractable[ (int) Buttons.NextButton ];
		m_createButton.interactable = buttonIsInteractable[ (int) Buttons.CreateButton ];
		m_deleteButton.interactable = buttonIsInteractable[ (int) Buttons.DeleteButton ];
		m_trainButton.interactable = buttonIsInteractable[ (int) Buttons.TrainButton ];
		m_exitButton.interactable = buttonIsInteractable[ (int) Buttons.ExitButton ];
		m_selectButton.interactable = buttonIsInteractable[ (int) Buttons.SelectButton ];
		m_cancelButton.interactable = buttonIsInteractable[ (int) Buttons.CancelButton ];

		// update navigation for create button
		Navigation navigation = m_createButton.navigation;
		navigation.selectOnRight = ( m_previousButton.interactable ? m_previousButton : ( m_nextButton.interactable ? m_nextButton : m_exitButton ) );
		m_createButton.navigation = navigation;

		// update navigation for previous button
		navigation = m_previousButton.navigation;
		navigation.selectOnRight = ( m_nextButton.interactable ? m_nextButton : m_exitButton );
		m_previousButton.navigation = navigation;

		// update navigation for next button
		navigation = m_nextButton.navigation;
		navigation.selectOnLeft = ( m_previousButton.interactable ? m_previousButton : m_createButton );
		m_nextButton.navigation = navigation;

		// update navigation for exit button
		navigation = m_exitButton.navigation;
		navigation.selectOnLeft = ( m_nextButton.interactable ? m_nextButton : ( m_previousButton.interactable ? m_previousButton : m_createButton ) );
		m_exitButton.navigation = navigation;

		// show or hide game objects now
		m_skillValuesText.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.SkillValuesText ] );
		m_raceNameText.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.RaceNameText ] );
		m_bioValuesText.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.BioValuesText ] );
		m_vitalityText.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.VitalityText ] );
		m_bankBalanceText.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.BankBalanceText ] );
		m_trainingText.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.TrainingText ] );
		m_humanImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.HumanImage ] );
		m_veloxImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.VeloxImage ] );
		m_thrynnImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.ThrynnImage ] );
		m_elowanImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.ElowanImage ] );
		m_androidImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.AndroidImage ] );
		m_leftArrowImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.LeftArrowImage ] );
		m_rightArrowImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.RightArrowImage ] );
		m_trainButton.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.TrainButton ] );
		m_deleteButton.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.DeleteButton ] );
		m_selectButton.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.SelectButton ] );
		m_cancelButton.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.CancelButton ] );
		m_overlayPanel.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.OverlayPanel ] );
		m_namePanel.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.NamePanel ] );
		m_deletePanel.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.DeletePanel ] );
		m_selectionXform.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.SelectionXform ] );
		m_upArrowImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.UpArrowImage ] );
		m_downArrowImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.DownArrowImage ] );
	}

	// update screen for the view file state
	private bool UpdateScreenForViewFileState( bool[] gameObjectIsVisible, bool[] buttonIsInteractable )
	{
		// dont show personnel file by default
		bool showPersonnelFile = false;

		// get access to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// enable the create button only if we have less than 20 personnel files
		if ( playerData.m_personnelPlayerData.m_personnelList.Count < 20 )
		{
			buttonIsInteractable[ (int) Buttons.CreateButton ] = true;
		}

		// check if we have any personnel files
		if ( playerData.m_personnelPlayerData.m_personnelList.Count == 0 )
		{
			// we dont have any personnel files
			m_fileNumberText.text = "No Personnel Files Found";
		}
		else
		{
			// get access to the current personnel file we are looking at
			PersonnelPlayerData.Personnel personnel = playerData.m_personnelPlayerData.m_personnelList[ m_currentFileIndex ];

			// update the current race index
			m_currentRaceIndex = personnel.m_raceIndex;

			// update the personnel file number
			m_fileNumberText.text = "File # " + ( m_currentFileIndex + 1 ) + ": " + personnel.m_name;

			// enable the previous button if we are not looking at the first personnel file
			if ( m_currentFileIndex > 0 )
			{
				buttonIsInteractable[ (int) Buttons.PreviousButton ] = true;
			}

			// enable the next button if we are not looking at the last personnel file
			if ( m_currentFileIndex < ( playerData.m_personnelPlayerData.m_personnelList.Count - 1 ) )
			{
				buttonIsInteractable[ (int) Buttons.NextButton ] = true;
			}

			// show the vitaliaty text
			gameObjectIsVisible[ (int) GameObjects.VitalityText ] = true;

			// show the delete button
			gameObjectIsVisible[ (int) GameObjects.DeleteButton ] = true;

			// enable the delete button
			buttonIsInteractable[ (int) Buttons.DeleteButton ] = true;

			// show the train button
			gameObjectIsVisible[ (int) GameObjects.TrainButton ] = true;

			// enable the train button
			buttonIsInteractable[ (int) Buttons.TrainButton ] = true;

			// update the skill values
			UpdateSkillValues();

			// we do want to show the personnel file
			showPersonnelFile = true;
		}

		// always enable the exit button
		buttonIsInteractable[ (int) Buttons.ExitButton ] = true;

		// let the caller know if we want to show the personnel file
		return showPersonnelFile;
	}

	// update screen for the select race state
	private bool UpdateScreenForSelectRaceState( bool[] gameObjectIsVisible, bool[] buttonIsInteractable )
	{
		// update the file number text
		m_fileNumberText.text = "New Personnel File";

		// show the left and right arrows
		gameObjectIsVisible[ (int) GameObjects.LeftArrowImage ] = true;
		gameObjectIsVisible[ (int) GameObjects.RightArrowImage ] = true;

		// show the select and cancel buttons
		gameObjectIsVisible[ (int) GameObjects.SelectButton ] = true;
		gameObjectIsVisible[ (int) GameObjects.CancelButton ] = true;

		buttonIsInteractable[ (int) Buttons.SelectButton ] = true;
		buttonIsInteractable[ (int) Buttons.CancelButton ] = true;

		// get the current race game data
		Race race = PersistentController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

		// update the skill values text to show the race's initial values
		m_skillValuesText.text = "";

		for ( int i = 0; i < c_numSkills; i++ )
		{
			m_skillValuesText.text += race.GetInitialSkill( i ).ToString();

			if ( i < ( c_numSkills - 1 ) )
			{
				m_skillValuesText.text += Environment.NewLine;
			}
		}

		// we do want to show the personnel file
		return true;
	}

	// update screen for the give name state
	private bool UpdateScreenForGiveNameState( bool[] gameObjectIsVisible, bool[] buttonIsInteractable )
	{
		// show the overlay
		gameObjectIsVisible[ (int) GameObjects.OverlayPanel ] = true;

		// show the name panel
		gameObjectIsVisible[ (int) GameObjects.NamePanel ] = true;

		// we do want to show the personnel file
		return true;
	}

	// update screen for the delete crewmember state
	private bool UpdateScreenForDeleteCrewmemberState( bool[] gameObjectIsVisible, bool[] buttonIsInteractable )
	{
		// show the overlay
		gameObjectIsVisible[ (int) GameObjects.OverlayPanel ] = true;

		// show the name panel
		gameObjectIsVisible[ (int) GameObjects.DeletePanel ] = true;

		// show the vitaliaty text
		gameObjectIsVisible[ (int) GameObjects.VitalityText ] = true;

		// we do want to show the personnel file
		return true;
	}

	// update screen for the train crewmember state
	private bool UpdateScreenForTrainCrewmemberState( bool[] gameObjectIsVisible, bool[] buttonIsInteractable )
	{
		// show the selection xform object
		gameObjectIsVisible[ (int) GameObjects.SelectionXform ] = true;

		// show the up arrow only if the first skill is not selected
		if ( m_currentSkillIndex > 0 )
		{
			gameObjectIsVisible[ (int) GameObjects.UpArrowImage ] = true;
		}

		// show the down arrow only if the last skill is not selected
		if ( m_currentSkillIndex < ( c_numSkills - 1 ) )
		{
			gameObjectIsVisible[ (int) GameObjects.DownArrowImage ] = true;
		}

		// put the skill selection box in the right place
		float offset = m_currentSkillIndex * m_skillValuesText.renderedHeight / c_numSkills;

		RectTransform rectTransform = m_selectionXform.GetComponent<RectTransform>();
		rectTransform.offsetMin = m_baseSelectionOffsetMin + new Vector3( 0.0f, -offset, 0.0f );
		rectTransform.offsetMax = m_baseSelectionOffsetMax + new Vector3( 0.0f, -offset, 0.0f );

		// update the bank balance text
		UpdateBankBalanceText();

		// show the bank balance text
		gameObjectIsVisible[ (int) GameObjects.BankBalanceText ] = true;

		// update the training text
		UpdateTrainingText( 0 );

		// show the training text
		gameObjectIsVisible[ (int) GameObjects.TrainingText ] = true;

		// we do want to show the personnel file
		return true;
	}

	// update the skill values
	private void UpdateSkillValues()
	{
		// get access to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get access to the current personnel file we are looking at
		PersonnelPlayerData.Personnel personnel = playerData.m_personnelPlayerData.m_personnelList[ m_currentFileIndex ];

		// update the skill values with the ones in this personnel file
		m_skillValuesText.text = "";

		for ( int i = 0; i < c_numSkills; i++ )
		{
			m_skillValuesText.text += personnel.GetSkill( i ).ToString();

			if ( i < ( c_numSkills - 1 ) )
			{
				m_skillValuesText.text += Environment.NewLine;
			}
		}
	}

	// show the selected race
	private void ShowRace( bool[] gameObjectIsVisible )
	{
		// get the current race game data
		Race race = PersistentController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

		// update the race name
		m_raceNameText.text = race.m_name;

		// update the bio values text to show the race's bio
		m_bioValuesText.text = race.m_type + Environment.NewLine + race.m_averageHeight + Environment.NewLine + race.m_averageWeight + Environment.NewLine + race.m_durability + Environment.NewLine + race.m_learningRate;

		// show the correct image for this race
		switch ( m_currentRaceIndex )
		{
			case 0: gameObjectIsVisible[ (int) GameObjects.HumanImage ] = true; break;
			case 1: gameObjectIsVisible[ (int) GameObjects.VeloxImage ] = true; break;
			case 2: gameObjectIsVisible[ (int) GameObjects.ThrynnImage ] = true; break;
			case 3: gameObjectIsVisible[ (int) GameObjects.ElowanImage ] = true; break;
			case 4: gameObjectIsVisible[ (int) GameObjects.AndroidImage ] = true; break;
		}
	}

	// update the bank balance text and show it
	private void UpdateBankBalanceText()
	{
		// get access to the bank player data
		BankPlayerData bankPlayerData = PersistentController.m_instance.m_playerData.m_bankPlayerData;

		// update the bank balance
		m_bankBalanceText.text = "Bank balance: " + string.Format( "{0:n0}", bankPlayerData.m_currentBalance ) + " M.U.";

		// show the training text
		m_bankBalanceText.gameObject.SetActive( true );
	}

	// update the training text and show it
	private void UpdateTrainingText( int messageIndex )
	{
		switch ( messageIndex )
		{
			case 0:
			{
				m_trainingText.text = "Cost: 300 M.U. per session";
				break;
			}

			case 1:
			{
				m_trainingText.text = "Androids skills are hard-wired";
				break;
			}

			case 2:
			{
				m_trainingText.text = "This crewmember is fully trained";
				break;
			}

			case 3:
			{
				m_trainingText.text = "This skill is fully trained";
				break;
			}

			case 4:
			{
				m_trainingText.text = "This skill cannot be trained";
				break;
			}

			case 5:
			{
				m_trainingText.text = "Your bank balance is too low";
				break;
			}
		}

		// show the training text
		m_trainingText.gameObject.SetActive( true );
	}

	// train the currently selected skill
	private void TrainSelectedSkill()
	{
		// get access to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get access to the bank player data
		BankPlayerData bankPlayerData = playerData.m_bankPlayerData;

		if ( bankPlayerData.m_currentBalance < 300 )
		{
			UpdateTrainingText( 5 );

			GetComponent<UISoundController>().Play( UISoundController.UISound.Error );
		}
		else
		{
			// get access to the current personnel file we are looking at
			PersonnelPlayerData.Personnel personnel = playerData.m_personnelPlayerData.m_personnelList[ m_currentFileIndex ];

			// get access to the race data for this personnel file
			Race race = PersistentController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

			// calculate the current skill and maximum skill points for the selected skill
			int currentSkill = personnel.GetSkill( m_currentSkillIndex );
			int maximumSkill = race.GetMaximumSkill( m_currentSkillIndex );

			// check if the maximum skill is zero
			if ( maximumSkill == 0 )
			{
				UpdateTrainingText( 4 );

				GetComponent<UISoundController>().Play( UISoundController.UISound.Error );
			}
			else if ( currentSkill < maximumSkill ) // check if we are still below the maximum skill points
			{
				// increase the skill by the learn amount
				personnel.SetSkill( m_currentSkillIndex, Math.Min( maximumSkill, currentSkill + race.m_learningRate ) );

				// take off 300 credits from the bank balance
				bankPlayerData.m_currentBalance -= 300;

				// update the bank balance text
				UpdateBankBalanceText();

				// update the skill values text
				UpdateSkillValues();

				// play a ui sound
				GetComponent<UISoundController>().Play( UISoundController.UISound.Update );

				// save the player data
				PersistentController.m_instance.SavePlayerData();
			}
			else // the selected skill is already maxxed out
			{
				UpdateTrainingText( 3 );

				GetComponent<UISoundController>().Play( UISoundController.UISound.Error );
			}
		}
	}

	// this is called if we clicked on the create button
	public void CreateClicked()
	{
		// switch to the create select race state
		SwitchToSelectRaceState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the previous button
	public void PreviousClicked()
	{
		// go back one personnel file
		m_currentFileIndex--;

		// update the screen
		UpdateScreen();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}

	// this is called if we clicked on the next button
	public void NextClicked()
	{
		// go to the next personnel file
		m_currentFileIndex++;

		// update the screen
		UpdateScreen();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		// close this ui
		Hide();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
	}

	// this is called if we clicked on the train button
	public void TrainClicked()
	{
		// check if the current race is an android
		if ( m_currentRaceIndex == 4 )
		{
			UpdateTrainingText( 1 );

			GetComponent<UISoundController>().Play( UISoundController.UISound.Error );
		}
		else
		{
			// get access to the player data
			PlayerData playerData = PersistentController.m_instance.m_playerData;

			// get access to the current personnel file we are looking at
			PersonnelPlayerData.Personnel personnel = playerData.m_personnelPlayerData.m_personnelList[ m_currentFileIndex ];

			// get access to the race data for this personnel file
			Race race = PersistentController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

			// enable the train button only if the current personnel is not maxxed out
			int maxTotalPoints = 0;
			int currentTotalPoints = 0;

			for ( int i = 0; i < c_numSkills; i++ )
			{
				maxTotalPoints = race.GetMaximumSkill( i );
				currentTotalPoints = personnel.GetSkill( i );
			}

			// check if we are maxxed out
			if ( currentTotalPoints < maxTotalPoints )
			{
				// switch to the train select skill state
				SwitchToTrainCrewmemberState();
			}
			else
			{
				UpdateTrainingText( 2 );

				GetComponent<UISoundController>().Play( UISoundController.UISound.Error );
			}
		}

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the delete button
	public void DeleteClicked()
	{
		// switch to the delete crewmember state
		SwitchToDeleteCrewmemberState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the select button
	public void SelectClicked()
	{
		// switch to the create name state
		SwitchToGiveNameState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}

	// this is called if we clicked on the cancel button
	public void CancelClicked()
	{
		// switch to the doing nothing state
		SwitchToViewFileState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
	}

	// this is called when the yes button in the delete panel is clicked
	public void YesClicked()
	{
		// get to the personnel player data
		PersonnelPlayerData personnelPlayerData = PersistentController.m_instance.m_playerData.m_personnelPlayerData;

		// delete the crewmember
		personnelPlayerData.m_personnelList.RemoveAt( m_currentFileIndex );

		// save the changes
		PersistentController.m_instance.SavePlayerData();

		// change the current file index if necessary
		if ( m_currentFileIndex >= personnelPlayerData.m_personnelList.Count )
		{
			m_currentFileIndex = Math.Max( 0, personnelPlayerData.m_personnelList.Count - 1 );
		}

		// switch to the doing nothing state
		SwitchToViewFileState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
	}

	// this is called when the no button in the delete panel is clicked
	public void NoClicked()
	{
		// switch to the doing nothing state
		SwitchToViewFileState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
	}

	// this is called when we hit enter in the name input field
	public void OnEndEdit()
	{
		// get the current race game data
		Race race = PersistentController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

		// create a new personnel file
		PersonnelPlayerData.Personnel personnel = PersistentController.m_instance.m_playerData.m_personnelPlayerData.CreateNewPersonnel();

		// set up the personnel file
		personnel.m_name = m_nameInputField.text;
		personnel.m_vitality = 100.0f;
		personnel.m_raceIndex = m_currentRaceIndex;
		personnel.m_science = race.m_scienceInitial;
		personnel.m_navigation = race.m_navigationInitial;
		personnel.m_engineering = race.m_engineeringInitial;
		personnel.m_communications = race.m_communicationsInitial;
		personnel.m_medicine = race.m_medicineInitial;

		// add the new personnel file to the list
		PersistentController.m_instance.m_playerData.m_personnelPlayerData.m_personnelList.Add( personnel );

		// save the new personnel file to disk
		PersistentController.m_instance.SavePlayerData();

		// make the new file our current one
		m_currentFileIndex = PersistentController.m_instance.m_playerData.m_personnelPlayerData.m_personnelList.Count - 1;

		// switch to the doing nothing state
		SwitchToViewFileState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}
}
