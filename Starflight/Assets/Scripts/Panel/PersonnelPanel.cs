
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PersonnelPanel : Panel
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

	// the starport controller
	public AstronautController m_astronautController;

	// private stuff we don't want the editor to see
	State m_currentState;
	int m_currentFileIndex;
	int m_currentRaceIndex;
	int m_currentSkillIndex;
	int m_startingBankBalance;
	Vector3 m_baseSelectionOffsetMin;
	Vector3 m_baseSelectionOffsetMax;
	float m_ignoreControllerTimer;

	// constant values
	const int c_numRaces = 5;
	const int c_numSkills = 5;

	// unity awake
	void Awake()
	{
		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;

		// remember the base selection offset
		RectTransform rectTransform = m_selectionXform.GetComponent<RectTransform>();
		m_baseSelectionOffsetMin = rectTransform.offsetMin;
		m_baseSelectionOffsetMax = rectTransform.offsetMax;
	}

	// panel open
	public override bool Open()
	{
		// base panel open
		base.Open();

		// view the first file
		m_currentFileIndex = 0;

		// remember the starting bank balance
		m_startingBankBalance = DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		// update the ui
		UpdateScreen();

		// switch to the default view
		SwitchToViewFileState();

		// panel was opened
		return true; 
	}

	// call this to hide the personnel ui
	public override void Close()
	{
		// if the bank balance has changed then record it in the bank transaction log
		int deltaBalance = m_startingBankBalance - DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		if ( deltaBalance > 0 )
		{
			PD_Bank.Transaction transaction = new PD_Bank.Transaction( DataController.m_instance.m_playerData.m_starflight.m_currentStardateYMD, "Personnel", deltaBalance.ToString() + "-" );

			DataController.m_instance.m_playerData.m_bank.m_transactionList.Add( transaction );
		}

		// base panel close
		base.Close();
	}

	// panel closed
	public override void Closed()
	{
		// base panel closed
		base.Closed();

		// let the starport controller know
		m_astronautController.PanelWasClosed();
	}

	// panel tick
	public override void Tick()
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
		// check if we moved the stick left
		if ( InputController.m_instance.m_west )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				m_currentRaceIndex = ( m_currentRaceIndex + c_numRaces - 1 ) % c_numRaces;

				UpdateScreen();

				SoundController.m_instance.PlaySound( SoundController.Sound.Click );
			}
		}
		else if ( InputController.m_instance.m_east ) // check if we moved the stick right
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				m_currentRaceIndex = ( m_currentRaceIndex + 1 ) % c_numRaces;

				UpdateScreen();

				SoundController.m_instance.PlaySound( SoundController.Sound.Click );
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
		// check if we moved the stick down
		if ( InputController.m_instance.m_south )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentSkillIndex < ( c_numSkills - 1 ) )
				{
					m_currentSkillIndex++;

					UpdateTrainingText( 0 );

					UpdateScreen();

					SoundController.m_instance.PlaySound( SoundController.Sound.Click );
				}
			}
		}
		else if ( InputController.m_instance.m_north ) // check if we have moved the stick up
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentSkillIndex > 0 )
				{
					m_currentSkillIndex--;

					UpdateTrainingText( 0 );

					UpdateScreen();

					SoundController.m_instance.PlaySound( SoundController.Sound.Click );
				}
			}
		}
		else // we have centered the stick
		{
			m_ignoreControllerTimer = 0.0f;
		}

		// check if we have pressed the fire button
		if ( InputController.m_instance.SubmitWasPressed() )
		{
			TrainSelectedSkill();
		}

		// check if we have pressed the cancel button
		if ( InputController.m_instance.CancelWasPressed() )
		{
			SwitchToViewFileState();

			SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
		}
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
		InputController.m_instance.m_debounceNextUpdate = true;
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
		PlayerData playerData = DataController.m_instance.m_playerData;

		// enable the create button only if we have less than 20 personnel files
		if ( playerData.m_personnel.m_personnelList.Count < 20 )
		{
			buttonIsInteractable[ (int) Buttons.CreateButton ] = true;
		}

		// check if we have any personnel files
		if ( playerData.m_personnel.m_personnelList.Count == 0 )
		{
			// we dont have any personnel files
			m_fileNumberText.text = "No Personnel Files Found";
		}
		else
		{
			// get access to the current personnel file we are looking at
			PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_personnel.m_personnelList[ m_currentFileIndex ];

			// update the current race index
			m_currentRaceIndex = personnelFile.m_raceIndex;

			// update the personnel file number
			m_fileNumberText.text = "File # " + ( m_currentFileIndex + 1 ) + ": " + personnelFile.m_name;

			// enable the previous button if we are not looking at the first personnel file
			if ( m_currentFileIndex > 0 )
			{
				buttonIsInteractable[ (int) Buttons.PreviousButton ] = true;
			}

			// enable the next button if we are not looking at the last personnel file
			if ( m_currentFileIndex < ( playerData.m_personnel.m_personnelList.Count - 1 ) )
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
		GD_Race race = DataController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

		// update the skill values text to show the race's initial values
		m_skillValuesText.text = "";

		for ( int skillIndex = 0; skillIndex < c_numSkills; skillIndex++ )
		{
			m_skillValuesText.text += race.GetInitialSkill( skillIndex ).ToString();

			if ( skillIndex < ( c_numSkills - 1 ) )
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
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get access to the current personnel file we are looking at
		PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_personnel.m_personnelList[ m_currentFileIndex ];

		// update the skill values with the ones in this personnel file
		m_skillValuesText.text = "";

		for ( int skillIndex = 0; skillIndex < c_numSkills; skillIndex++ )
		{
			m_skillValuesText.text += personnelFile.GetSkill( skillIndex ).ToString();

			if ( skillIndex < ( c_numSkills - 1 ) )
			{
				m_skillValuesText.text += Environment.NewLine;
			}
		}
	}

	// show the selected race
	private void ShowRace( bool[] gameObjectIsVisible )
	{
		// get the current race game data
		GD_Race race = DataController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

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
		PD_Bank bank = DataController.m_instance.m_playerData.m_bank;

		// update the bank balance
		m_bankBalanceText.text = "Bank balance: " + string.Format( "{0:n0}", bank.m_currentBalance ) + " M.U.";

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
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get access to the bank player data
		PD_Bank bank = playerData.m_bank;

		if ( bank.m_currentBalance < 300 )
		{
			UpdateTrainingText( 5 );

			SoundController.m_instance.PlaySound( SoundController.Sound.Error );
		}
		else
		{
			// get access to the current personnel file we are looking at
			PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_personnel.m_personnelList[ m_currentFileIndex ];

			// get access to the race data for this personnel file
			GD_Race race = DataController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

			// calculate the current skill and maximum skill points for the selected skill
			int currentSkill = personnelFile.GetSkill( m_currentSkillIndex );
			int maximumSkill = race.GetMaximumSkill( m_currentSkillIndex );

			// check if the maximum skill is zero
			if ( maximumSkill == 0 )
			{
				UpdateTrainingText( 4 );

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );
			}
			else if ( currentSkill < maximumSkill ) // check if we are still below the maximum skill points
			{
				// increase the skill by the learn amount
				personnelFile.SetSkill( m_currentSkillIndex, Math.Min( maximumSkill, currentSkill + race.m_learningRate ) );

				// take off 300 credits from the bank balance
				bank.m_currentBalance -= 300;

				// update the bank balance text
				UpdateBankBalanceText();

				// update the skill values text
				UpdateSkillValues();

				// play a ui sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Update );
			}
			else // the selected skill is already maxxed out
			{
				UpdateTrainingText( 3 );

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );
			}
		}
	}

	// this is called if we clicked on the create button
	public void CreateClicked()
	{
		// switch to the create select race state
		SwitchToSelectRaceState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the previous button
	public void PreviousClicked()
	{
		// go back one personnel file
		m_currentFileIndex--;

		// update the screen
		UpdateScreen();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the next button
	public void NextClicked()
	{
		// go to the next personnel file
		m_currentFileIndex++;

		// update the screen
		UpdateScreen();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		// close this panel
		PanelController.m_instance.Close();
	}

	// this is called if we clicked on the train button
	public void TrainClicked()
	{
		// check if the current race is an android
		if ( m_currentRaceIndex == 4 )
		{
			UpdateTrainingText( 1 );

			SoundController.m_instance.PlaySound( SoundController.Sound.Error );
		}
		else
		{
			// get access to the player data
			PlayerData playerData = DataController.m_instance.m_playerData;

			// get access to the current personnel file we are looking at
			PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_personnel.m_personnelList[ m_currentFileIndex ];

			// get access to the race data for this personnel file
			GD_Race race = DataController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

			// enable the train button only if the current personnel is not maxxed out
			int maxTotalPoints = 0;
			int currentTotalPoints = 0;

			for ( int skillIndex = 0; skillIndex < c_numSkills; skillIndex++ )
			{
				maxTotalPoints = race.GetMaximumSkill( skillIndex );
				currentTotalPoints = personnelFile.GetSkill( skillIndex );
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

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );
			}
		}

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the delete button
	public void DeleteClicked()
	{
		// switch to the delete crewmember state
		SwitchToDeleteCrewmemberState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the select button
	public void SelectClicked()
	{
		// switch to the create name state
		SwitchToGiveNameState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the cancel button
	public void CancelClicked()
	{
		// switch to the doing nothing state
		SwitchToViewFileState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
	}

	// this is called when the yes button in the delete panel is clicked
	public void YesClicked()
	{
		// get to the personnel player data
		PD_Personnel personnel = DataController.m_instance.m_playerData.m_personnel;

		// delete the crewmember
		personnel.m_personnelList.RemoveAt( m_currentFileIndex );

		// change the current file index if necessary
		if ( m_currentFileIndex >= personnel.m_personnelList.Count )
		{
			m_currentFileIndex = Math.Max( 0, personnel.m_personnelList.Count - 1 );
		}

		// switch to the doing nothing state
		SwitchToViewFileState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
	}

	// this is called when the no button in the delete panel is clicked
	public void NoClicked()
	{
		// switch to the doing nothing state
		SwitchToViewFileState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
	}

	// this is called when we hit enter in the name input field
	public void OnEndEdit()
	{
		if ( m_nameInputField.text.Length == 0 )
		{
			// cancel because the player did not type in anything
			SwitchToViewFileState();

			// play a ui sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
		}
		else
		{
			// get the current race game data
			GD_Race race = DataController.m_instance.m_gameData.m_raceList[ m_currentRaceIndex ];

			// create a new personnel file
			PD_Personnel.PD_PersonnelFile personnelFile = DataController.m_instance.m_playerData.m_personnel.CreateNewPersonnel();

			// set up the personnel file
			personnelFile.m_name = m_nameInputField.text;
			personnelFile.m_vitality = 100.0f;
			personnelFile.m_raceIndex = m_currentRaceIndex;
			personnelFile.m_science = race.m_scienceInitial;
			personnelFile.m_navigation = race.m_navigationInitial;
			personnelFile.m_engineering = race.m_engineeringInitial;
			personnelFile.m_communications = race.m_communicationsInitial;
			personnelFile.m_medicine = race.m_medicineInitial;

			// add the new personnel file to the list
			DataController.m_instance.m_playerData.m_personnel.m_personnelList.Add( personnelFile );


			// make the new file our current one
			m_currentFileIndex = DataController.m_instance.m_playerData.m_personnel.m_personnelList.Count - 1;

			// switch to the doing nothing state
			SwitchToViewFileState();

			// play a ui sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Update );
		}
	}
}
