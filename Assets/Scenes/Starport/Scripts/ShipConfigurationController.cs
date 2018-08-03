
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShipConfigurationController : PanelController
{
	enum State
	{
		MenuBar, BuyPart, SelectClass, GiveName, ErrorMessage
	}

	enum GameObjects
	{
		OverlayPanel, NamePanel, ErrorPanel, SelectedPartPanel, SelectionXform, UpArrowImage, DownArrowImage, GameObjectCount
	}

	// public stuff we want to set using the editor
	public Button m_buyButton;
	public Button m_sellButton;
	public Button m_repairButton;
	public Button m_nameButton;
	public Button m_exitButton;
	public TextMeshProUGUI m_componentNamesText;
	public TextMeshProUGUI m_componentValuesText;
	public TextMeshProUGUI m_configurationValuesText;
	public TextMeshProUGUI m_statusValuesText;
	public TextMeshProUGUI m_currentBalanceText;
	public TextMeshProUGUI m_errorMessageText;
	public Image m_upArrowImage;
	public Image m_downArrowImage;
	public Image m_shieldImage;
	public GameObject m_overlayPanel;
	public GameObject m_namePanel;
	public GameObject m_errorPanel;
	public GameObject m_selectedPartPanel;
	public GameObject m_selectionXform;
	public GameObject m_missileLauncher;
	public GameObject m_laserCannon;
	public GameObject[] m_cargoPods;
	public InputField m_nameInputField;

	// private stuff we don't want the editor to see
	private StarportController m_starportController;
	private InputManager m_inputManager;
	private State m_currentState;
	private int m_currentPartIndex;
	private int m_currentClassIndex;
	private int m_startingBankBalance;
	private Vector3 m_baseSelectionOffsetMin;
	private Vector3 m_baseSelectionOffsetMax;
	private float m_ignoreControllerTimer;

	// constant values
	private const int c_numParts = 6;
	private const int c_numClasses = 5;
	private const int c_numComponentValuesLines = 13;

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
			case State.BuyPart:
			{
				UpdateControllerForBuyPartState();
				break;
			}

			case State.SelectClass:
			{
				UpdateControllerForSelectClassState();
				break;
			}

			case State.ErrorMessage:
			{
				UpdateControllerForErrorMessageState();
				break;
			}
		}
	}

	// controller updates for when we are in the buy part state
	private void UpdateControllerForBuyPartState()
	{
		// get the controller stick position
		float y = m_inputManager.GetRawY();

		// check if we moved the stick down
		if ( y <= -0.5f )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentPartIndex < ( c_numParts - 1 ) )
				{
					m_currentPartIndex++;

					UpdateScreen();

					GetComponent<UISoundController>().Play( UISoundController.UISound.Click );
				}
			}
		}
		else if ( y >= 0.5f ) // check if we have moved the stick up
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentPartIndex > 0 )
				{
					m_currentPartIndex--;

					UpdateScreen();

					GetComponent<UISoundController>().Play( UISoundController.UISound.Click );
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
			BuySelectedPart();

			GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
		}

		// check if we have pressed the cancel button
		if ( m_inputManager.GetCancelDown() )
		{
			SwitchToMenuBarState();

			GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
		}
	}

	// controller updates for when we are in the select class state
	private void UpdateControllerForSelectClassState()
	{
		// get the controller stick position
		float y = m_inputManager.GetRawY();

		// check if we moved the stick down
		if ( y <= -0.5f )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentClassIndex < ( c_numClasses - 1 ) )
				{
					m_currentClassIndex++;

					UpdateScreen();

					GetComponent<UISoundController>().Play( UISoundController.UISound.Click );
				}
			}
		}
		else if ( y >= 0.5f ) // check if we have moved the stick up
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentClassIndex > 0 )
				{
					m_currentClassIndex--;

					UpdateScreen();

					GetComponent<UISoundController>().Play( UISoundController.UISound.Click );
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
			BuySelectedPart();
		}

		// check if we have pressed the cancel button
		if ( m_inputManager.GetCancelDown() )
		{
			SwitchToBuyPartState( false );

			GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
		}
	}

	private void UpdateControllerForErrorMessageState()
	{
		// check if we have pressed the fire or cancel button
		if ( m_inputManager.GetSubmitDown() || m_inputManager.GetCancelDown() )
		{
			SwitchToMenuBarState();

			GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
		}
	}

	// call this to show the ship configuraton ui
	public override void Show()
	{
		// reset the current state
		m_currentState = State.MenuBar;

		// remember the starting bank balance
		m_startingBankBalance = PersistentController.m_instance.m_playerData.m_bank.m_currentBalance;

		// update the ui
		UpdateScreen();

		// start the opening animation
		StartOpeningUI();
	}

	// call this to hide the ship configuration ui
	public override void Hide()
	{
		// lose the focus
		LoseFocus();

		// start the closing animation
		StartClosingUI();

		// if the bank balance has changed then record it in the bank transaction log
		int deltaBalance = m_startingBankBalance - PersistentController.m_instance.m_playerData.m_bank.m_currentBalance;

		if ( deltaBalance != 0 )
		{
			string sign = ( deltaBalance > 0 ) ? "-" : "+";

			Bank.Transaction transaction = new Bank.Transaction( PersistentController.m_instance.m_playerData.m_starflight.m_currentStardate, "Ship Configuration", deltaBalance.ToString() + sign );

			PersistentController.m_instance.m_playerData.m_bank.m_transactionList.Add( transaction );
		}
	}

	// call this to take control
	public void TakeFocus()
	{
		// turn on controller navigation of the UI
		EventSystem.current.sendNavigationEvents = true;

		// switch to the default view
		SwitchToMenuBarState();

		// cancel the ui sounds
		GetComponent<UISoundController>().CancelSounds();
	}

	// call this to give up control
	public void LoseFocus()
	{
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

	// call this to switch to the menu bar state
	private void SwitchToMenuBarState()
	{
		// change the current state
		m_currentState = State.MenuBar;

		// update the screen
		UpdateScreen();

		// select the buy button
		m_buyButton.Select();
	}

	// call this to switch to the buy part state
	private void SwitchToBuyPartState( bool resetCurrentPartIndex = true )
	{
		// change the current state
		m_currentState = State.BuyPart;

		// select the first part by default
		if ( resetCurrentPartIndex )
		{
			m_currentPartIndex = 0;
		}

		// update the screen
		UpdateScreen();

		// debounce the input
		m_inputManager.DebounceNextUpdate();
	}

	// call this to switch to the select class state
	private void SwitchToSelectClassState()
	{
		// change the current state
		m_currentState = State.SelectClass;

		// select the first part by default
		m_currentClassIndex = 0;

		// update the screen
		UpdateScreen();

		// debounce the input
		m_inputManager.DebounceNextUpdate();
	}

	// call this to switch to the give name state
	private void SwitchToGiveNameState()
	{
		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// change the current state
		m_currentState = State.GiveName;

		// update the screen
		UpdateScreen();

		// set the current text input to the current name of the ship
		m_nameInputField.text = PersistentController.m_instance.m_playerData.m_shipConfiguration.m_name;

		// select the text input by default
		m_nameInputField.Select();
	}

	// call this to switch to the error message state
	private void SwitchToErrorMessageState( string errorMessage )
	{
		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// change the current state
		m_currentState = State.ErrorMessage;

		// update the screen
		UpdateScreen();

		// set the error message
		m_errorMessageText.text = errorMessage;

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Error );
	}

	// call this whenever we change state or do something that would result in something changing on the screen
	private void UpdateScreen()
	{
		// clear out the component values text
		m_componentValuesText.text = "";

		// reset game objects and buttons
		bool[] gameObjectIsVisible = new bool[ (int) GameObjects.GameObjectCount ];

		switch ( m_currentState )
		{
			// we are currently buying parts
			case State.BuyPart:
			{
				UpdateScreenForBuyPartState( gameObjectIsVisible );
				break;
			}

			// we are currently selecting a class
			case State.SelectClass:
			{
				UpdateScreenForSelectClassState( gameObjectIsVisible );
				break;
			}

			// we are currently giving a name
			case State.GiveName:
			{
				UpdateScreenForGiveNameState( gameObjectIsVisible );
				break;
			}

			// we are currently showing the error message
			case State.ErrorMessage:
			{
				UpdateScreenForErrorMessageState( gameObjectIsVisible );
				break;
			}
		}

		// enable or disable buttons now
		bool enableButtons = ( m_currentState == State.MenuBar );

		m_buyButton.interactable = enableButtons;
		m_sellButton.interactable = enableButtons;
		m_repairButton.interactable = enableButtons;
		m_nameButton.interactable = enableButtons;
		m_exitButton.interactable = enableButtons;

		// show or hide game objects now
		m_overlayPanel.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.OverlayPanel ] );
		m_namePanel.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.NamePanel ] );
		m_errorPanel.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.ErrorPanel ] );
		m_selectedPartPanel.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.SelectedPartPanel ] );
		m_selectionXform.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.SelectionXform ] );
		m_upArrowImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.UpArrowImage ] );
		m_downArrowImage.gameObject.SetActive( gameObjectIsVisible[ (int) GameObjects.DownArrowImage ] );

		// get the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the ship configuration player data
		ShipConfiguration shipConfiguration = playerData.m_shipConfiguration;

		// update configuration values
		m_configurationValuesText.text = shipConfiguration.m_numCargoPods.ToString() + Environment.NewLine;
		m_configurationValuesText.text += Environment.NewLine;
		m_configurationValuesText.text += shipConfiguration.GetEnginesClassString() + Environment.NewLine;
		m_configurationValuesText.text += shipConfiguration.GetSheildingClassString() + Environment.NewLine;
		m_configurationValuesText.text += shipConfiguration.GetArmorClassString() + Environment.NewLine;
		m_configurationValuesText.text += shipConfiguration.GetMissileLauncherClassString() + Environment.NewLine;
		m_configurationValuesText.text += shipConfiguration.GetLaserCannonClassString() + Environment.NewLine;

		// show only as many cargo pods as we have purchased
		for ( int cargoPodId = 0; cargoPodId < m_cargoPods.Length; cargoPodId++ )
		{
			m_cargoPods[ cargoPodId ].SetActive( cargoPodId < shipConfiguration.m_numCargoPods );
		}

		// hide or show the shield image depending on if we have them
		m_shieldImage.gameObject.SetActive( shipConfiguration.m_shieldingClass > 0 );

		// hide or show the missile launchers depending on if we have them
		m_missileLauncher.SetActive( shipConfiguration.m_missileLauncherClass > 0 );

		// hide or show the missile launchers depending on if we have them
		m_laserCannon.SetActive( shipConfiguration.m_laserCannonClass > 0 );

		// update status values
		m_statusValuesText.text = shipConfiguration.m_mass + " Tons" + Environment.NewLine;
		m_statusValuesText.text += shipConfiguration.m_acceleration + " G" + Environment.NewLine;

		// report the amount of endurium on the ship
		ElementReference elementReference = playerData.m_shipCargo.m_elementStorage.Find( "Endurium" );

		if ( elementReference == null )
		{
			m_statusValuesText.text += "0.0M<sup>3</sup>";
		}
		else
		{
			m_statusValuesText.text += ( elementReference.m_volume / 10 ) + "." + ( elementReference.m_volume % 10 ) + "M<sup>3</sup>";
		}

		// update bank balance amount
		m_currentBalanceText.text = "Your account balance is: " + playerData.m_bank.m_currentBalance + " M.U.";
	}

	// update screen for the buy part state
	private void UpdateScreenForBuyPartState( bool[] gameObjectIsVisible )
	{
		// put in the prices for the parts
		UpdatePartPrices();

		// show the selection xform object
		gameObjectIsVisible[ (int) GameObjects.SelectionXform ] = true;

		// show the up arrow only if the first part is not selected
		if ( m_currentPartIndex > 0 )
		{
			gameObjectIsVisible[ (int) GameObjects.UpArrowImage ] = true;
		}

		// show the down arrow only if the last part is not selected
		if ( m_currentPartIndex < ( c_numParts - 1 ) )
		{
			gameObjectIsVisible[ (int) GameObjects.DownArrowImage ] = true;
		}

		// put the part selection box in the right place
		float offset = ( ( m_currentPartIndex == 0 ) ? 0 : ( m_currentPartIndex + 1 ) ) * m_componentNamesText.renderedHeight / c_numComponentValuesLines;

		RectTransform rectTransform = m_selectionXform.GetComponent<RectTransform>();
		rectTransform.offsetMin = m_baseSelectionOffsetMin + new Vector3( 0.0f, -offset, 0.0f );
		rectTransform.offsetMax = m_baseSelectionOffsetMax + new Vector3( 0.0f, -offset, 0.0f );
	}

	// update screen for the select class state
	private void UpdateScreenForSelectClassState( bool[] gameObjectIsVisible )
	{
		// put in the prices for the parts
		UpdatePartPrices( false );

		// show the selected part object
		gameObjectIsVisible[ (int) GameObjects.SelectedPartPanel ] = true;

		// put the selected part box in the right place
		float offset = ( m_currentPartIndex + 1 ) * m_componentNamesText.renderedHeight / c_numComponentValuesLines;

		RectTransform rectTransform = m_selectedPartPanel.GetComponent<RectTransform>();
		rectTransform.offsetMin = m_baseSelectionOffsetMin + new Vector3( 0.0f, -offset, 0.0f );
		rectTransform.offsetMax = m_baseSelectionOffsetMax + new Vector3( 0.0f, -offset, 0.0f );

		// show the selection xform object
		gameObjectIsVisible[ (int) GameObjects.SelectionXform ] = true;

		// show the up arrow only if the first class is not selected
		if ( m_currentClassIndex > 0 )
		{
			gameObjectIsVisible[ (int) GameObjects.UpArrowImage ] = true;
		}

		// show the down arrow only if the last class is not selected
		if ( m_currentClassIndex < ( c_numClasses - 1 ) )
		{
			gameObjectIsVisible[ (int) GameObjects.DownArrowImage ] = true;
		}

		// put the class selection box in the right place
		offset = ( m_currentClassIndex + 8 ) * m_componentNamesText.renderedHeight / c_numComponentValuesLines;

		rectTransform = m_selectionXform.GetComponent<RectTransform>();
		rectTransform.offsetMin = m_baseSelectionOffsetMin + new Vector3( 0.0f, -offset, 0.0f );
		rectTransform.offsetMax = m_baseSelectionOffsetMax + new Vector3( 0.0f, -offset, 0.0f );
	}

	// update screen for the give name state
	private void UpdateScreenForGiveNameState( bool[] gameObjectIsVisible )
	{
		// show the overlay
		gameObjectIsVisible[ (int) GameObjects.OverlayPanel ] = true;

		// show the name panel
		gameObjectIsVisible[ (int) GameObjects.NamePanel ] = true;
	}

	// update screen for the error message state
	private void UpdateScreenForErrorMessageState( bool[] gameObjectIsVisible )
	{
		// show the overlay
		gameObjectIsVisible[ (int) GameObjects.OverlayPanel ] = true;

		// show the name panel
		gameObjectIsVisible[ (int) GameObjects.ErrorPanel ] = true;
	}

	// this is called if we clicked on the buy button
	public void BuyClicked()
	{
		// switch to the buy part state
		SwitchToBuyPartState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the sell button
	public void SellClicked()
	{
		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the repair button
	public void RepairClicked()
	{
		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the name button
	public void NameClicked()
	{
		// switch to the give name state
		SwitchToGiveNameState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		// close this ui
		Hide();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
	}

	// this is called when we hit enter in the name input field
	public void OnEndEdit()
	{
		// update the ship name in the player data
		PersistentController.m_instance.m_playerData.m_shipConfiguration.m_name = m_nameInputField.text;

		// switch to the menu bar state
		SwitchToMenuBarState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}

	// buy the currently selected part
	private void BuySelectedPart()
	{
		// check if we are buying a cargo pod and handle that differently
		if ( m_currentPartIndex == 0 )
		{
			BuyCargoPod();
		}
		else // we are buying a part
		{
			SwitchToSelectClassState();
		}
	}

	private void UpdatePartPrices( bool includeCargoPods = true )
	{
		// get to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		if ( includeCargoPods )
		{
			// show the cargo pod price
			m_componentValuesText.text = gameData.m_shipGameData.m_cargoPodBuyPrice.ToString() + Environment.NewLine;
		}
		else
		{
			m_componentValuesText.text = Environment.NewLine;
		}

		// skip to the class prices
		for ( int i = 0; i < 7; i++ )
		{
			m_componentValuesText.text += Environment.NewLine;
		}

		// update part class prices (if we have anything but cargo pods selected)
		if ( m_currentPartIndex > 0 )
		{
			ShipPartGameData[] shipPartList = null;

			switch ( m_currentPartIndex )
			{
				case 1: shipPartList = gameData.m_enginesList; break;
				case 2: shipPartList = gameData.m_shieldingList; break;
				case 3: shipPartList = gameData.m_armorList; break;
				case 4: shipPartList = gameData.m_missileLauncherList; break;
				case 5: shipPartList = gameData.m_laserCannonList; break;
			}

			for ( int shipPartId = 1; shipPartId < shipPartList.Length; shipPartId++ )
			{
				m_componentValuesText.text += shipPartList[ shipPartId ].m_buyPrice.ToString() + Environment.NewLine;
			}
		}
	}

	// buy a cargo pod
	private void BuyCargoPod()
	{
		// get to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// check if we have room for another cargo pod
		if ( playerData.m_shipConfiguration.m_numCargoPods == 16 )
		{
			// nope - show an error message
			SwitchToErrorMessageState( "No cargo pod bays available" );
		}
		else
		{
			// can we afford it?
			if ( playerData.m_bank.m_currentBalance < gameData.m_shipGameData.m_cargoPodBuyPrice )
			{
				// nope - show an error message
				SwitchToErrorMessageState( "Insufficient funds" );
			}
			else
			{
				// deduct the price of the cargo pod from the player's bank balance
				playerData.m_bank.m_currentBalance -= gameData.m_shipGameData.m_cargoPodBuyPrice;

				// add one cargo pod to the ship
				playerData.m_shipConfiguration.AddCargoPod();

				// update the screen
				UpdateScreen();

				// play a ui sound
				GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
			}
		}
	}
}
