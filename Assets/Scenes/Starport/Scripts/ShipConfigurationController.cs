
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShipConfigurationController : PanelController
{
	enum State
	{
		MenuBar, GiveName
	}

	enum GameObjects
	{
		OverlayPanel, NamePanel, GameObjectCount
	}

	// public stuff we want to set using the editor
	public Button m_buyButton;
	public Button m_sellButton;
	public Button m_repairButton;
	public Button m_nameButton;
	public Button m_exitButton;
	public GameObject m_overlayPanel;
	public GameObject m_namePanel;
	public InputField m_nameInputField;
	public TextMeshProUGUI m_currentConfigurationValues;

	// private stuff we don't want the editor to see
	private StarportController m_starportController;
	private InputManager m_inputManager;
	private State m_currentState;
	private float m_ignoreControllerTimer;

	// constant values

	// this is called by unity before start
	private void Awake()
	{
		// get access to the starport controller
		m_starportController = GetComponent<StarportController>();

		// get access to the input manager
		m_inputManager = GetComponent<InputManager>();

		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;
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
		}
	}

	// call this to show the ship configuraton ui
	public override void Show()
	{
		// reset the current state
		m_currentState = State.MenuBar;

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

	// call this to switch to the view file state
	private void SwitchToMenuBarState()
	{
		// change the current state
		m_currentState = State.MenuBar;

		// update the screen
		UpdateScreen();

		// select the buy button
		m_buyButton.Select();
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

		// erase the current text input
		m_nameInputField.text = PersistentController.m_instance.m_playerData.m_shipConfigurationPlayerData.m_name;

		// select the text input by default
		m_nameInputField.Select();
	}

	// call this whenever we change state or do something that would result in something changing on the screen
	private void UpdateScreen()
	{
		// reset game objects and buttons
		bool[] gameObjectIsVisible = new bool[ (int) GameObjects.GameObjectCount ];

		switch ( m_currentState )
		{
			// we're not doing anything particular at the moment
			case State.MenuBar:
			{
				UpdateScreenForMenuBarState( gameObjectIsVisible );
				break;
			}

			// we are currently giving a name
			case State.GiveName:
			{
				UpdateScreenForGiveNameState( gameObjectIsVisible );
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

		// get the ship configuration player data
		ShipConfigurationPlayerData shipConfigurationPlayerData = PersistentController.m_instance.m_playerData.m_shipConfigurationPlayerData;

		// update right panel text
		m_currentConfigurationValues.text = shipConfigurationPlayerData.m_numCargoPods.ToString() + Environment.NewLine;
		m_currentConfigurationValues.text += Environment.NewLine;
		m_currentConfigurationValues.text += shipConfigurationPlayerData.GetEnginesClassString() + Environment.NewLine;
		m_currentConfigurationValues.text += shipConfigurationPlayerData.GetSheildingClassString() + Environment.NewLine;
		m_currentConfigurationValues.text += shipConfigurationPlayerData.GetArmorClassString() + Environment.NewLine;
		m_currentConfigurationValues.text += shipConfigurationPlayerData.GetMissileLauncherClassString() + Environment.NewLine;
		m_currentConfigurationValues.text += shipConfigurationPlayerData.GetLaserCannonClassString() + Environment.NewLine;
	}

	// update screen for the view file state
	private void UpdateScreenForMenuBarState( bool[] gameObjectIsVisible )
	{
	}

	// update screen for the give name state
	private void UpdateScreenForGiveNameState( bool[] gameObjectIsVisible )
	{
		// show the overlay
		gameObjectIsVisible[ (int) GameObjects.OverlayPanel ] = true;

		// show the name panel
		gameObjectIsVisible[ (int) GameObjects.NamePanel ] = true;
	}

	// this is called if we clicked on the buy button
	public void BuyClicked()
	{
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
		PersistentController.m_instance.m_playerData.m_shipConfigurationPlayerData.m_name = m_nameInputField.text;

		// save the new ship name to disk
		PersistentController.m_instance.SavePlayerData();

		// switch to the menu bar state
		SwitchToMenuBarState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}
}
