
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShipConfigurationPanel : Panel
{
	enum State
	{
		MenuBar, BuyPart, SellPart, SelectClass, GiveName, ErrorMessage
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

	// the starport controller
	public AstronautController m_astronautController;

	// private stuff we don't want the editor to see
	State m_currentState;
	State m_stateBeforeError;
	int m_currentPartIndex;
	int m_currentClassIndex;
	int m_startingBankBalance;
	Vector3 m_baseSelectionOffsetMin;
	Vector3 m_baseSelectionOffsetMax;
	float m_ignoreControllerTimer;

	// constant values
	const int c_numParts = 6;
	const int c_numClasses = 5;
	const int c_numComponentValuesLines = 13;

	// unity awake
	void Awake()
	{
		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;

		// remember the base selection offset
		var rectTransform = m_selectionXform.GetComponent<RectTransform>();
		m_baseSelectionOffsetMin = rectTransform.offsetMin;
		m_baseSelectionOffsetMax = rectTransform.offsetMax;
	}

	// call this to show the ship configuraton ui
	public override bool Open()
	{
		// base panel open
		base.Open();

		// remember the starting bank balance
		m_startingBankBalance = DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		// switch to the menu bar state
		SwitchToMenuBarState();

		// update the ui
		UpdateScreen();

		// panel was opened
		return true;
	}

	// call this to hide the ship configuration ui
	public override void Close()
	{
		// if the bank balance has changed then record it in the bank transaction log
		var deltaBalance = m_startingBankBalance - DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		if ( deltaBalance != 0 )
		{
			var sign = ( deltaBalance > 0 ) ? "-" : "+";

			var transaction = new PD_Bank.Transaction( DataController.m_instance.m_playerData.m_general.m_currentStardateYMD, "Ship Configuration", deltaBalance.ToString() + sign );

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

	// this is called by unity every frame
	public override void Tick()
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

			case State.SellPart:
			{
				UpdateControllerForSellPartState();
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

	// controller stuff common between the buy and sell part states
	void UpdateController()
	{
		// check if we moved the stick down
		if ( InputController.m_instance.m_south )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentPartIndex < ( c_numParts - 1 ) )
				{
					m_currentPartIndex++;

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

				if ( m_currentPartIndex > 0 )
				{
					m_currentPartIndex--;

					UpdateScreen();

					SoundController.m_instance.PlaySound( SoundController.Sound.Click );
				}
			}
		}
		else // we have centered the stick
		{
			m_ignoreControllerTimer = 0.0f;
		}

		// check if we have pressed the cancel button
		if ( InputController.m_instance.m_west || InputController.m_instance.m_east || InputController.m_instance.m_cancel )
		{
			InputController.m_instance.Debounce();

			SwitchToMenuBarState();

			SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
		}
	}

	// controller updates for when we are in the buy part state
	void UpdateControllerForBuyPartState()
	{
		// perform common controller update
		UpdateController();

		// check if we have pressed the fire button
		if ( InputController.m_instance.m_submit )
		{
			InputController.m_instance.Debounce();

			BuySelectedPart();

			SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
		}
	}

	// controller updates for when we are in the sell part state
	void UpdateControllerForSellPartState()
	{
		// perform common controller update
		UpdateController();

		// check if we have pressed the fire button
		if ( InputController.m_instance.m_submit )
		{
			InputController.m_instance.Debounce();

			SellSelectedPart();
		}
	}

	// controller updates for when we are in the select class state
	void UpdateControllerForSelectClassState()
	{
		// check if we moved the stick down
		if ( InputController.m_instance.m_south )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentClassIndex < ( c_numClasses - 1 ) )
				{
					m_currentClassIndex++;

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

				if ( m_currentClassIndex > 0 )
				{
					m_currentClassIndex--;

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
		if ( InputController.m_instance.m_submit )
		{
			InputController.m_instance.Debounce();

			BuySelectedClass();
		}

		// check if we have pressed the cancel button
		if ( InputController.m_instance.m_west || InputController.m_instance.m_east || InputController.m_instance.m_cancel )
		{
			InputController.m_instance.Debounce();

			SwitchToBuyPartState( false );

			SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
		}
	}

	void UpdateControllerForErrorMessageState()
	{
		// check if we have pressed the fire or cancel button
		if ( InputController.m_instance.m_submit || InputController.m_instance.m_cancel )
		{
			InputController.m_instance.Debounce();

			// switch back to the previous state
			switch ( m_stateBeforeError )
			{
				case State.BuyPart:
				SwitchToBuyPartState( false );
				break;

				case State.SelectClass:
				SwitchToSelectClassState( false );
				break;

				default:
				SwitchToMenuBarState();
				break;
			}

			SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
		}
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
	void SwitchToBuyPartState( bool resetCurrentPartIndex = true )
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
	}

	// call this to switch to the sell part state
	void SwitchToSellPartState( bool resetCurrentPartIndex = true )
	{
		// change the current state
		m_currentState = State.SellPart;

		// select the first part by default
		if ( resetCurrentPartIndex )
		{
			m_currentPartIndex = 0;
		}

		// update the screen
		UpdateScreen();
	}

	// call this to switch to the select class state
	void SwitchToSelectClassState( bool resetCurrentClassIndex = true )
	{
		// change the current state
		m_currentState = State.SelectClass;

		// select the first part by default
		if ( resetCurrentClassIndex )
		{
			m_currentClassIndex = 0;
		}

		// update the screen
		UpdateScreen();
	}

	// call this to switch to the give name state
	void SwitchToGiveNameState()
	{
		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// change the current state
		m_currentState = State.GiveName;

		// update the screen
		UpdateScreen();

		// set the current text input to the current name of the ship
		m_nameInputField.text = DataController.m_instance.m_playerData.m_playerShip.m_name;

		// select the text input by default
		m_nameInputField.Select();
	}

	// call this to switch to the error message state
	void SwitchToErrorMessageState( string errorMessage )
	{
		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// remember the current state
		m_stateBeforeError = m_currentState;

		// change the current state
		m_currentState = State.ErrorMessage;

		// update the screen
		UpdateScreen();

		// set the error message
		m_errorMessageText.text = errorMessage;

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );
	}

	// call this whenever we change state or do something that would result in something changing on the screen
	void UpdateScreen()
	{
		// get the game data
		var gameData = DataController.m_instance.m_gameData;

		// get the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the ship player data
		var ship = playerData.m_playerShip;

		// clear out the component values text
		m_componentValuesText.text = "";

		// reset game objects and buttons
		var gameObjectIsVisible = new bool[ (int) GameObjects.GameObjectCount ];

		switch ( m_currentState )
		{
			// we are currently buying parts
			case State.BuyPart:
			{
				UpdateScreenForBuyPartState( gameObjectIsVisible );
				break;
			}

			// we are currently selling parts
			case State.SellPart:
			{
				UpdateScreenForSellPartState( gameObjectIsVisible );
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
		var enableButtons = ( m_currentState == State.MenuBar );

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

		// update configuration values
		m_configurationValuesText.text = ship.m_numCargoPods.ToString() + Environment.NewLine;
		m_configurationValuesText.text += Environment.NewLine;
		m_configurationValuesText.text += ship.GetEngines().m_name + Environment.NewLine;
		m_configurationValuesText.text += ship.GetSheilding().m_name + Environment.NewLine;
		m_configurationValuesText.text += ship.GetArmor().m_name + Environment.NewLine;
		m_configurationValuesText.text += ship.GetMissileLauncher().m_name + Environment.NewLine;
		m_configurationValuesText.text += ship.GetLaserCannon().m_name + Environment.NewLine;

		// show only as many cargo pods as we have purchased
		for ( var cargoPodId = 0; cargoPodId < m_cargoPods.Length; cargoPodId++ )
		{
			m_cargoPods[ cargoPodId ].SetActive( cargoPodId < ship.m_numCargoPods );
		}

		// hide or show the shield image depending on if we have them
		m_shieldImage.gameObject.SetActive( ship.m_shieldingClass > 0 );

		// hide or show the missile launchers depending on if we have them
		m_missileLauncher.SetActive( ship.m_missileLauncherClass > 0 );

		// hide or show the missile launchers depending on if we have them
		m_laserCannon.SetActive( ship.m_laserCannonClass > 0 );

		// update status values
		m_statusValuesText.text = ship.m_mass + " Tons" + Environment.NewLine;
		m_statusValuesText.text += ship.m_acceleration + " G" + Environment.NewLine;

		// report the amount of endurium on the ship
		var elementReference = playerData.m_playerShip.m_elementStorage.Find( gameData.m_misc.m_enduriumElementId );

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
	void UpdateScreenForBuyPartState( bool[] gameObjectIsVisible )
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
		var offset = ( ( m_currentPartIndex == 0 ) ? 0 : ( m_currentPartIndex + 1 ) ) * m_componentNamesText.renderedHeight / c_numComponentValuesLines;

		var rectTransform = m_selectionXform.GetComponent<RectTransform>();
		rectTransform.offsetMin = m_baseSelectionOffsetMin + new Vector3( 0.0f, -offset, 0.0f );
		rectTransform.offsetMax = m_baseSelectionOffsetMax + new Vector3( 0.0f, -offset, 0.0f );
	}

	// update screen for the sell part state
	void UpdateScreenForSellPartState( bool[] gameObjectIsVisible )
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
		var offset = ( ( m_currentPartIndex == 0 ) ? 0 : ( m_currentPartIndex + 1 ) ) * m_componentNamesText.renderedHeight / c_numComponentValuesLines;

		var rectTransform = m_selectionXform.GetComponent<RectTransform>();
		rectTransform.offsetMin = m_baseSelectionOffsetMin + new Vector3( 0.0f, -offset, 0.0f );
		rectTransform.offsetMax = m_baseSelectionOffsetMax + new Vector3( 0.0f, -offset, 0.0f );
	}

	// update screen for the select class state
	void UpdateScreenForSelectClassState( bool[] gameObjectIsVisible )
	{
		// put in the prices for the parts
		UpdatePartPrices( false );

		// show the selected part object
		gameObjectIsVisible[ (int) GameObjects.SelectedPartPanel ] = true;

		// put the selected part box in the right place
		float offset = ( m_currentPartIndex + 1 ) * m_componentNamesText.renderedHeight / c_numComponentValuesLines;

		var rectTransform = m_selectedPartPanel.GetComponent<RectTransform>();
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
	void UpdateScreenForGiveNameState( bool[] gameObjectIsVisible )
	{
		// show the overlay
		gameObjectIsVisible[ (int) GameObjects.OverlayPanel ] = true;

		// show the name panel
		gameObjectIsVisible[ (int) GameObjects.NamePanel ] = true;
	}

	// update screen for the error message state
	void UpdateScreenForErrorMessageState( bool[] gameObjectIsVisible )
	{
		// show the overlay
		gameObjectIsVisible[ (int) GameObjects.OverlayPanel ] = true;

		// show the name panel
		gameObjectIsVisible[ (int) GameObjects.ErrorPanel ] = true;
	}

	// this is called if we clicked on the buy button
	public void BuyClicked()
	{
		InputController.m_instance.Debounce();

		// switch to the buy part state
		SwitchToBuyPartState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the sell button
	public void SellClicked()
	{
		InputController.m_instance.Debounce();

		// switch to the buy part state
		SwitchToSellPartState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the repair button
	public void RepairClicked()
	{
		InputController.m_instance.Debounce();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the name button
	public void NameClicked()
	{
		InputController.m_instance.Debounce();

		// switch to the give name state
		SwitchToGiveNameState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		InputController.m_instance.Debounce();

		// close this panel
		PanelController.m_instance.Close();
	}

	// this is called when we hit enter in the name input field
	public void OnEndEdit()
	{
		InputController.m_instance.Debounce();

		// update the ship name in the player data
		DataController.m_instance.m_playerData.m_playerShip.m_name = m_nameInputField.text;

		// switch to the menu bar state
		SwitchToMenuBarState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Update );
	}

	// buy the currently selected part
	void BuySelectedPart()
	{
		// check if we are buying a cargo pod and handle that differently
		if ( m_currentPartIndex == 0 )
		{
			BuyCargoPod();
		}
		else // we are buying a part
		{
			// get to the player data
			var playerData = DataController.m_instance.m_playerData;

			// get what is currently installed on the ship
			var currentClass = 0;

			switch ( m_currentPartIndex )
			{
				case 1: currentClass = playerData.m_playerShip.m_enginesClass; break;
				case 2: currentClass = playerData.m_playerShip.m_shieldingClass; break;
				case 3: currentClass = playerData.m_playerShip.m_armorClass; break;
				case 4: currentClass = playerData.m_playerShip.m_missileLauncherClass; break;
				case 5: currentClass = playerData.m_playerShip.m_laserCannonClass; break;
			}

			// check if the ship has this part installed already
			if ( currentClass != 0 )
			{
				// yes - tell the player to sell it first
				SwitchToErrorMessageState( "De-equip first" );
			}
			else
			{
				// no - let the player select the class to buy
				SwitchToSelectClassState();
			}
		}
	}

	// sell the currently selected part
	void SellSelectedPart()
	{
		// check if we are selling a cargo pod and handle that differently
		if ( m_currentPartIndex == 0 )
		{
			SellCargoPod();
		}
		else // we are selling a part
		{
			// get to the player data
			var playerData = DataController.m_instance.m_playerData;

			// get what is currently installed on the ship
			var currentClass = 0;

			switch ( m_currentPartIndex )
			{
				case 1: currentClass = playerData.m_playerShip.m_enginesClass; break;
				case 2: currentClass = playerData.m_playerShip.m_shieldingClass; break;
				case 3: currentClass = playerData.m_playerShip.m_armorClass; break;
				case 4: currentClass = playerData.m_playerShip.m_missileLauncherClass; break;
				case 5: currentClass = playerData.m_playerShip.m_laserCannonClass; break;
			}

			// check if the ship has this part installed already
			if ( currentClass != 0 )
			{
				// get to the game data
				var gameData = DataController.m_instance.m_gameData;

				// get the part list
				GD_ShipPart[] shipPartList = null;

				switch ( m_currentPartIndex )
				{
					case 1: shipPartList = gameData.m_enginesList; break;
					case 2: shipPartList = gameData.m_shieldingList; break;
					case 3: shipPartList = gameData.m_armorList; break;
					case 4: shipPartList = gameData.m_missileLauncherList; break;
					case 5: shipPartList = gameData.m_laserCannonList; break;
				}

				// add the funds to the players bank balance
				playerData.m_bank.m_currentBalance += shipPartList[ currentClass ].m_sellPrice;

				// remove the part from the ship
				switch ( m_currentPartIndex )
				{
					case 1: playerData.m_playerShip.m_enginesClass = 0; break;
					case 2: playerData.m_playerShip.m_shieldingClass = 0; break;
					case 3: playerData.m_playerShip.m_armorClass = 0; break;
					case 4: playerData.m_playerShip.m_missileLauncherClass = 0; break;
					case 5: playerData.m_playerShip.m_laserCannonClass = 0; break;
				}

				// recalculate the ship mass and acceleration
				playerData.m_playerShip.RecalculateMass();
				playerData.m_playerShip.RecalculateAcceleration();

				// update the screen
				UpdateScreen();

				// play a ui sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Update );
			}
		}
	}
	// buy the currently selected class
	void BuySelectedClass()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get the ship part list
		GD_ShipPart[] shipPartList = null;

		switch ( m_currentPartIndex )
		{
			case 1: shipPartList = gameData.m_enginesList; break;
			case 2: shipPartList = gameData.m_shieldingList; break;
			case 3: shipPartList = gameData.m_armorList; break;
			case 4: shipPartList = gameData.m_missileLauncherList; break;
			case 5: shipPartList = gameData.m_laserCannonList; break;
		}

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the class to buy
		var classIndex = m_currentClassIndex + 1;

		// check if the player can afford it
		if ( playerData.m_bank.m_currentBalance < shipPartList[ classIndex ].m_buyPrice )
		{
			// nope!
			SwitchToErrorMessageState( "Insufficient funds" );
		}
		else
		{
			// yes so make the transaction and install the part
			playerData.m_bank.m_currentBalance -= shipPartList[ classIndex ].m_buyPrice;

			// upgrade the ship part
			switch ( m_currentPartIndex )
			{
				case 1: playerData.m_playerShip.m_enginesClass = classIndex; break;
				case 2: playerData.m_playerShip.m_shieldingClass = classIndex; break;
				case 3: playerData.m_playerShip.m_armorClass = classIndex; break;
				case 4: playerData.m_playerShip.m_missileLauncherClass = classIndex; break;
				case 5: playerData.m_playerShip.m_laserCannonClass = classIndex; break;
			}

			// recalculate the ship mass and acceleration
			playerData.m_playerShip.RecalculateMass();
			playerData.m_playerShip.RecalculateAcceleration();

			// if we just bought armor then update the armor points on the ship
			if ( m_currentPartIndex == 3 )
			{
				var armor = playerData.m_playerShip.GetArmor();

				playerData.m_playerShip.m_armorPoints = armor.m_points;
			}

			// switch back to the buy part state
			SwitchToBuyPartState();

			// play a ui sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Update );
		}
	}

	void UpdatePartPrices( bool includeCargoPods = true )
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if we are selling
		if ( m_currentState == State.SellPart )
		{
			// hide the sell prices for cargo pods if we don't have any
			if ( playerData.m_playerShip.m_numCargoPods == 0 )
			{
				includeCargoPods = false;
			}
		}

		if ( includeCargoPods )
		{
			// show the cargo pod price
			m_componentValuesText.text = gameData.m_misc.m_cargoPodBuyPrice.ToString() + Environment.NewLine;
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

		// get what is currently installed on the ship
		var currentClass = 0;

		switch ( m_currentPartIndex )
		{
			case 1: currentClass = playerData.m_playerShip.m_enginesClass; break;
			case 2: currentClass = playerData.m_playerShip.m_shieldingClass; break;
			case 3: currentClass = playerData.m_playerShip.m_armorClass; break;
			case 4: currentClass = playerData.m_playerShip.m_missileLauncherClass; break;
			case 5: currentClass = playerData.m_playerShip.m_laserCannonClass; break;
		}

		// update part class prices (if we have anything but cargo pods selected)
		if ( m_currentPartIndex > 0 )
		{
			GD_ShipPart[] shipPartList = null;

			switch ( m_currentPartIndex )
			{
				case 1: shipPartList = gameData.m_enginesList; break;
				case 2: shipPartList = gameData.m_shieldingList; break;
				case 3: shipPartList = gameData.m_armorList; break;
				case 4: shipPartList = gameData.m_missileLauncherList; break;
				case 5: shipPartList = gameData.m_laserCannonList; break;
			}

			for ( var classIndex = 1; classIndex < shipPartList.Length; classIndex++ )
			{
				// check if we are selling
				if ( m_currentState == State.SellPart )
				{
					// yep - show the sell price only for the class we have installed
					if ( classIndex == currentClass )
					{
						m_componentValuesText.text += shipPartList[ classIndex ].m_sellPrice.ToString() + Environment.NewLine;
					}
					else
					{
						m_componentValuesText.text += Environment.NewLine;
					}
				}
				else
				{
					// no - we are buying - show all prices
					m_componentValuesText.text += shipPartList[ classIndex ].m_buyPrice.ToString() + Environment.NewLine;
				}
			}
		}
	}

	// buy a cargo pod
	void BuyCargoPod()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if we have room for another cargo pod
		if ( playerData.m_playerShip.m_numCargoPods == 16 )
		{
			// nope - show an error message
			SwitchToErrorMessageState( "No cargo pod bays available" );
		}
		else
		{
			// can we afford it?
			if ( playerData.m_bank.m_currentBalance < gameData.m_misc.m_cargoPodBuyPrice )
			{
				// nope - show an error message
				SwitchToErrorMessageState( "Insufficient funds" );
			}
			else
			{
				// deduct the price of the cargo pod from the player's bank balance
				playerData.m_bank.m_currentBalance -= gameData.m_misc.m_cargoPodBuyPrice;

				// add one cargo pod to the ship
				playerData.m_playerShip.AddCargoPod();

				// update the screen
				UpdateScreen();

				// play a ui sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Update );
			}
		}
	}

	// sell a cargo pod
	void SellCargoPod()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if we have room for another cargo pod
		if ( playerData.m_playerShip.m_numCargoPods > 0 )
		{
			// pay for the cargo pod into the player's bank balance
			playerData.m_bank.m_currentBalance += gameData.m_misc.m_cargoPodSellPrice;

			// remove one cargo pod from the ship
			playerData.m_playerShip.RemoveCargoPod();

			// update the screen
			UpdateScreen();

			// play a ui sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Update );
		}
	}
}
