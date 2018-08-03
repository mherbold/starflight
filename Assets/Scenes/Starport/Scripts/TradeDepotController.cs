
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TradeDepotController : PanelController
{
	enum State
	{
		MenuBar, BuyItem, BuyAmount, SellItem, SellAmount, AnalyzeItem, AnalyzeConfirm, AnalyzeShow, ErrorMessage
	}

	private class Item
	{
		public int m_row;
		public int m_type;
		public int m_id;
		public int m_volume;

		public Item( int row, int type, int id, int volume )
		{
			m_row = row;
			m_type = type;
			m_id = id;
			m_volume = volume;
		}
	}

	// public stuff we want to set using the editor
	public TextMeshProUGUI m_itemListText;
	public TextMeshProUGUI m_volumeListText;
	public TextMeshProUGUI m_unitValueListText;
	public TextMeshProUGUI m_currentBalanceText;
	public TextMeshProUGUI m_amountLabelText;
	public TextMeshProUGUI m_errorMessageText;
	public TextMeshProUGUI m_analyzeText;
	public Image m_upArrowImage;
	public Image m_downArrowImage;
	public Button m_buyButton;
	public Button m_sellButton;
	public Button m_analyzeButton;
	public Button m_exitButton;
	public Button m_noButton;
	public GameObject m_selectionXform;
	public GameObject m_welcomeGameObject;
	public GameObject m_tradeGameObject;
	public GameObject m_overlayPanel;
	public GameObject m_amountGameObject;
	public GameObject m_itemListMask;
	public GameObject m_errorPanel;
	public GameObject m_analyzePanel;
	public GameObject m_confirmAnalyzePanel;
	public InputField m_amountInputField;

	// private stuff we don't want the editor to see
	private StarportController m_starportController;
	private InputManager m_inputManager;
	private State m_currentState;
	private State m_stateBeforeError;
	private int m_currentItemIndex;
	private int m_startingBankBalance;
	private Vector3 m_baseSelectionOffsetMin;
	private Vector3 m_baseSelectionOffsetMax;
	private float m_ignoreControllerTimer;
	private List<Item> m_itemList;
	private int m_rowCount;
	private int m_currentRowOffset;

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
			case State.BuyItem:
			{
				UpdateControllerForBuyItemState();
				break;
			}

			case State.SellItem:
			{
				UpdateControllerForSellItemState();
				break;
			}

			case State.AnalyzeItem:
			{
				UpdateControllerForAnalyzeItemState();
				break;
			}

			case State.AnalyzeShow:
			{
				UpdateControllerForAnalyzeShowState();
				break;
			}

			case State.ErrorMessage:
			{
				UpdateControllerForErrorMessageState();
				break;
			}
		}
	}

	// common controller updates for the buy item, sell item, and analyze item states
	private void UpdateController()
	{
		// get the controller stick position
		float y = m_inputManager.GetRawY();

		// check if we moved the stick down
		if ( y <= -0.5f )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				if ( m_currentItemIndex < ( m_itemList.Count - 1 ) )
				{
					m_currentItemIndex++;

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

				if ( m_currentItemIndex > 0 )
				{
					m_currentItemIndex--;

					UpdateScreen();

					GetComponent<UISoundController>().Play( UISoundController.UISound.Click );
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
			SwitchToMenuBarState();

			GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
		}
	}

	// controller updates for when we are in the buy item state
	private void UpdateControllerForBuyItemState()
	{
		// do common controller updates
		UpdateController();

		// check if we have pressed the fire button
		if ( m_inputManager.GetSubmitDown() )
		{
			GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );

			BuySelectedItem();
		}
	}

	// controller updates for when we are in the sell item state
	private void UpdateControllerForSellItemState()
	{
		// do common controller updates
		UpdateController();

		// check if we have pressed the fire button
		if ( m_inputManager.GetSubmitDown() )
		{
			GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );

			SellSelectedItem();
		}
	}

	// controller updates for when we are in the analyze item state
	private void UpdateControllerForAnalyzeItemState()
	{
		// do common controller updates
		UpdateController();

		// check if we have pressed the fire button
		if ( m_inputManager.GetSubmitDown() )
		{
			GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );

			AnalyzeSelectedItem();
		}
	}

	private void UpdateControllerForAnalyzeShowState()
	{
		// check if we have pressed the fire or cancel button
		if ( m_inputManager.GetSubmitDown() || m_inputManager.GetCancelDown() )
		{
			// switch back to the analyze item state
			SwitchToAnalyzeItemState( false );

			// play a ui sound
			GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
		}
	}

	private void UpdateControllerForErrorMessageState()
	{
		// check if we have pressed the fire or cancel button
		if ( m_inputManager.GetSubmitDown() || m_inputManager.GetCancelDown() )
		{
			// switch back to the previous state
			switch ( m_stateBeforeError )
			{
				case State.BuyItem:
				SwitchToBuyItemState( false );
				break;

				case State.BuyAmount:
				SwitchToBuyAmountState();
				break;

				case State.SellItem:
				SwitchToSellItemState( false );
				break;

				case State.SellAmount:
				SwitchToSellAmountState();
				break;

				case State.AnalyzeItem:
				SwitchToAnalyzeItemState( false );
				break;

				case State.MenuBar:
				SwitchToMenuBarState();
				break;
			}

			// play a ui sound
			GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
		}
	}

	// call this to show the operations ui
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

	// call this to hide the operations ui
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

			Bank.Transaction transaction = new Bank.Transaction( PersistentController.m_instance.m_playerData.m_starflight.m_currentStardate, "Trade depot", deltaBalance.ToString() + sign );

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

	// call this to switch to the buy item state
	private void SwitchToBuyItemState( bool resetCurrentItemIndex = true )
	{
		// change the current state
		m_currentState = State.BuyItem;

		// select the first part by default
		if ( resetCurrentItemIndex )
		{
			m_currentItemIndex = 0;
			m_currentRowOffset = 0;
		}

		// update the screen
		UpdateScreen();

		// debounce the input
		m_inputManager.DebounceNextUpdate();
	}

	// call this to switch to the buy amount state
	private void SwitchToBuyAmountState()
	{
		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// change the current state
		m_currentState = State.BuyAmount;

		// update the screen
		UpdateScreen();

		// erase the current text input
		m_amountInputField.text = "";

		// select the text input by default
		m_amountInputField.Select();
	}

	// call this to switch to the sell item state
	private void SwitchToSellItemState( bool resetCurrentItemIndex = true )
	{
		// change the current state
		m_currentState = State.SellItem;

		// select the first part by default
		if ( resetCurrentItemIndex )
		{
			m_currentItemIndex = 0;
			m_currentRowOffset = 0;
		}

		// update the screen
		UpdateScreen();

		// debounce the input
		m_inputManager.DebounceNextUpdate();
	}

	// call this to switch to the sell amount state
	private void SwitchToSellAmountState()
	{
		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// change the current state
		m_currentState = State.SellAmount;

		// update the screen
		UpdateScreen();

		// erase the current text input
		m_amountInputField.text = "";

		// select the text input by default
		m_amountInputField.Select();
	}

	// call this to switch to the analyze item state
	private void SwitchToAnalyzeItemState( bool resetCurrentItemIndex = true )
	{
		// change the current state
		m_currentState = State.AnalyzeItem;

		// select the first part by default
		if ( resetCurrentItemIndex )
		{
			m_currentItemIndex = 0;
			m_currentRowOffset = 0;
		}

		// update the screen
		UpdateScreen();

		// debounce the input
		m_inputManager.DebounceNextUpdate();
	}

	// call this to switch to the confirm analysis state
	private void SwitchToAnalyzeConfirmState()
	{
		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// change the current state
		m_currentState = State.AnalyzeConfirm;

		// update the screen
		UpdateScreen();

		// select the no button by default
		m_noButton.Select();
	}

	// call this to switch to the show analysis state
	private void SwitchToAnalyzeShowState()
	{
		// get the currently selected item
		Item item = m_itemList[ m_currentItemIndex ];

		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// update the analyze text
		m_analyzeText.text = gameData.m_artifactList[ item.m_id ].m_analysisText;

		// change the current state
		m_currentState = State.AnalyzeShow;

		// update the screen
		UpdateScreen();

		// debounce the input
		m_inputManager.DebounceNextUpdate();
	}

	// call this to switch to the error message state
	private void SwitchToErrorMessageState( string errorMessage )
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
		GetComponent<UISoundController>().Play( UISoundController.UISound.Error );

		// debounce the input
		m_inputManager.DebounceNextUpdate();
	}

	// call this whenever we change state or do something that would result in something changing on the screen
	private void UpdateScreen()
	{
		if ( m_currentState == State.ErrorMessage )
		{
			// turn on the error message display
			m_errorPanel.SetActive( true );
			m_overlayPanel.SetActive( true );

			// turn off the other popup panels
			m_amountGameObject.SetActive( false );
			m_confirmAnalyzePanel.SetActive( false );
			m_analyzePanel.SetActive( false );
		}
		else
		{
			// turn off the error message display
			m_errorPanel.SetActive( false );
			m_overlayPanel.SetActive( false );

			if ( m_currentState == State.MenuBar )
			{
				// all we need to do in this state is hide the trade screen and show the welcome screen
				m_welcomeGameObject.SetActive( true );
				m_tradeGameObject.SetActive( false );
				m_amountGameObject.SetActive( false );
				m_confirmAnalyzePanel.SetActive( false );
				m_analyzePanel.SetActive( false );
			}
			else
			{
				// hide the welcome screen and show the trade screen
				m_welcomeGameObject.SetActive( false );
				m_tradeGameObject.SetActive( true );

				// get access to the game data
				GameData gameData = PersistentController.m_instance.m_gameData;

				// get access to the player data
				PlayerData playerData = PersistentController.m_instance.m_playerData;

				// check if we are needing the buy or sell amount
				if ( ( m_currentState == State.BuyAmount ) || ( m_currentState == State.SellAmount ) )
				{
					// display the amount input
					m_amountGameObject.SetActive( true );
					m_overlayPanel.SetActive( true );

					// get the currently selected element id
					int elementId = m_itemList[ m_currentItemIndex ].m_id;

					// figure out the maximum amount the player can buy
					int maximumAmount = 0;

					if ( m_currentState == State.BuyAmount )
					{
						maximumAmount = GetMaximumBuyAmountDueToCurrentBalance( elementId );

						if ( maximumAmount == 0 )
						{
							// the player is broke and cannot buy anything - so immediately block the player
							m_currentState = State.BuyItem;
							SwitchToErrorMessageState( "Insufficient funds" );
							return;
						}

						int remainingVolume = playerData.m_shipConfiguration.GetRemainingVolme();

						if ( remainingVolume == 0 )
						{
							// the cargo hold is full and the player cannot buy anything - so immediately block the player
							m_currentState = State.BuyItem;
							SwitchToErrorMessageState( "Insufficient cargo space" );
							return;
						}

						// the maximum amount the player can buy is the lesser of the funds remaining or the cargo hold space remaining
						if ( remainingVolume < maximumAmount )
						{
							maximumAmount = remainingVolume;
						}
					}
					else
					{
						// the maximum amount is however much the player has in the ships cargo hold
						ElementReference elementReference = playerData.m_shipCargo.m_elementStorage.Find( elementId );

						maximumAmount = elementReference.m_volume;
					}

					// update the amount label
					m_amountLabelText.text = "Transfer how many cubic meters? (0.0 to " + ( maximumAmount / 10 ) + "." + ( maximumAmount % 10 ) + ")";
				}
				else
				{
					// hide the amount input
					m_amountGameObject.SetActive( false );
				}

				// check if we are confirming analysis of an artifact
				if ( m_currentState == State.AnalyzeConfirm )
				{
					// show the confirm analyze panel
					m_confirmAnalyzePanel.SetActive( true );
					m_overlayPanel.SetActive( true );
				}
				else
				{
					// hide the confirm analyze panel
					m_confirmAnalyzePanel.SetActive( false );
				}

				// check if we are showing the analysis of an artifact
				if ( m_currentState == State.AnalyzeShow )
				{
					// show the analyze panel
					m_analyzePanel.SetActive( true );
					m_overlayPanel.SetActive( true );
				}
				else
				{
					// hide the analyze panel
					m_analyzePanel.SetActive( false );
				}

				// reset the trade item list
				m_itemList = new List<Item>();

				// clear out the text
				m_itemListText.text = "";
				m_volumeListText.text = "";
				m_unitValueListText.text = "";

				m_rowCount = 0;

				if ( ( m_currentState != State.AnalyzeItem ) && ( m_currentState != State.AnalyzeConfirm ) && ( m_currentState != State.AnalyzeShow ) )
				{
					// add elements heading
					m_itemListText.text += "<color=#A35514>Elements</color>" + Environment.NewLine;
					m_volumeListText.text += Environment.NewLine;
					m_unitValueListText.text += Environment.NewLine;

					m_rowCount++;

					// get access to the ship cargo data for elements
					ElementStorage elementStorage = playerData.m_shipCargo.m_elementStorage;

					if ( ( m_currentState == State.BuyItem ) || ( m_currentState == State.BuyAmount ) )
					{
						// add all elements available to buy in starport
						for ( int elementId = 0; elementId < gameData.m_elementList.Length; elementId++ )
						{
							ElementGameData elementGameData = gameData.m_elementList[ elementId ];

							if ( elementGameData.m_availableInStarport )
							{
								m_itemListText.text += elementGameData.m_name + Environment.NewLine;

								ElementReference elementReference = elementStorage.Find( elementId );

								if ( elementReference == null )
								{
									m_volumeListText.text += "0.0" + Environment.NewLine;
								}
								else
								{
									m_volumeListText.text += ( elementReference.m_volume / 10 ) + "." + ( elementReference.m_volume % 10 ) + Environment.NewLine;
								}

								m_unitValueListText.text += elementGameData.m_starportPrice + Environment.NewLine;

								m_itemList.Add( new Item( m_rowCount++, 0, elementId, 0 ) );
							}
						}
					}
					else if ( ( m_currentState == State.SellItem ) || ( m_currentState == State.SellAmount ) )
					{
						// add all elements in the ship cargo hold
						foreach ( ElementReference elementReference in elementStorage.m_elementList )
						{
							ElementGameData elementGameData = elementReference.GetElementGameData();

							m_itemListText.text += elementGameData.m_name + Environment.NewLine;
							m_volumeListText.text += ( elementReference.m_volume / 10 ) + "." + ( elementReference.m_volume % 10 ) + Environment.NewLine;
							m_unitValueListText.text += elementGameData.m_starportPrice + Environment.NewLine;

							m_itemList.Add( new Item( m_rowCount++, 0, elementReference.m_elementId, 0 ) );
						}
					}
				}

				if ( ( m_currentState == State.BuyItem ) || ( m_currentState == State.BuyAmount ) || ( m_currentState == State.AnalyzeItem ) || ( m_currentState == State.AnalyzeConfirm ) || ( m_currentState == State.AnalyzeShow ) )
				{
					// add artifacts heading
					m_itemListText.text += "<color=#A35514>Artifacts</color>" + Environment.NewLine;
					m_volumeListText.text += Environment.NewLine;
					m_unitValueListText.text += Environment.NewLine;

					m_rowCount++;

					// get access to the starport data for artifacts
					ArtifactStorage artifactStorage = playerData.m_starportCargo.m_artifactStorage;

					// add all artifacts available to buy in starport
					foreach (  ArtifactReference artifactReference in artifactStorage.m_artifactList )
					{
						ArtifactGameData artifactGameData = artifactReference.GetArtifactGameData();

						m_itemListText.text += artifactGameData.m_name + Environment.NewLine;
						m_volumeListText.text += ( artifactGameData.m_volume / 10 ) + "." + ( artifactGameData.m_volume % 10 ) + Environment.NewLine;
						m_unitValueListText.text += artifactGameData.m_starportPrice + Environment.NewLine;

						m_itemList.Add( new Item( m_rowCount++, 1, artifactReference.m_artifactId, 0 ) );
					}
				}
				
				if ( ( m_currentState == State.SellItem ) || ( m_currentState == State.SellAmount ) || ( m_currentState == State.AnalyzeItem ) || ( m_currentState == State.AnalyzeConfirm ) || ( m_currentState == State.AnalyzeShow ) )
				{
					// add artifacts heading
					m_itemListText.text += "<color=#A35514>Artifacts</color>" + Environment.NewLine;
					m_volumeListText.text += Environment.NewLine;
					m_unitValueListText.text += Environment.NewLine;

					m_rowCount++;

					// get access to the ship storage for artifacts
					ArtifactStorage artifactStorage = playerData.m_shipCargo.m_artifactStorage;

					// add all artifacts in the ship cargo hold
					foreach ( ArtifactReference artifactReference in artifactStorage.m_artifactList )
					{
						ArtifactGameData artifactGameData = artifactReference.GetArtifactGameData();

						m_itemListText.text += artifactGameData.m_name + Environment.NewLine;
						m_volumeListText.text += ( artifactGameData.m_volume / 10 ) + "." + ( artifactGameData.m_volume % 10 ) + Environment.NewLine;
						m_unitValueListText.text += artifactGameData.m_actualValue + Environment.NewLine;

						m_itemList.Add( new Item( m_rowCount++, 1, artifactReference.m_artifactId, 0 ) );
					}
				}

				// add end of list heading
				m_itemListText.text += "<color=#A35514>End of List</color>" + Environment.NewLine;
				m_volumeListText.text += Environment.NewLine;
				m_unitValueListText.text += Environment.NewLine;

				m_rowCount++;

				// make sure we have something in the list
				if ( m_itemList.Count == 0 )
				{
					// there is nothing sell or analyze - so immediately block the player
					SwitchToMenuBarState();
					return;
				}

				// keep the current selection index within bounds
				if ( m_currentItemIndex >= m_itemList.Count )
				{
					m_currentItemIndex = m_itemList.Count - 1;
				}

				// force the text object to update (so we can get the correct height)
				m_itemListText.ForceMeshUpdate();

				// force the canvas to update
				Canvas.ForceUpdateCanvases();

				// show the up arrow only if the first item is not selected
				m_upArrowImage.gameObject.SetActive( ( m_currentItemIndex > 0 ) );

				// show the down arrow only if the last part is not selected
				m_downArrowImage.gameObject.SetActive( m_currentItemIndex < ( m_itemList.Count - 1 ) );

				// get the row number of the currently selected item
				int row = m_itemList[ m_currentItemIndex ].m_row;

				// get the height of the item list viewport
				float viewportHeight = m_itemListMask.GetComponent<RectTransform>().rect.height;

				// calculate height of each text row
				float rowHeight = m_itemListText.renderedHeight / m_rowCount;

				// figure out the offset for the selection box
				float selectionBoxOffset;

				while ( true )
				{
					selectionBoxOffset = ( row + m_currentRowOffset ) * rowHeight;

					if ( ( selectionBoxOffset + rowHeight * 2 ) >= viewportHeight )
					{
						m_currentRowOffset--;
					}
					else if ( selectionBoxOffset < rowHeight )
					{
						m_currentRowOffset++;
					}
					else
					{
						break;
					}
				}

				// put the item selection box in the right place
				RectTransform rectTransform = m_selectionXform.GetComponent<RectTransform>();
				rectTransform.offsetMin = m_baseSelectionOffsetMin + new Vector3( 0.0f, -selectionBoxOffset, 0.0f );
				rectTransform.offsetMax = m_baseSelectionOffsetMax + new Vector3( 0.0f, -selectionBoxOffset, 0.0f );

				// calculate the offset for the text
				float textOffset = m_currentRowOffset * rowHeight;

				// move the text in all 3 columns
				m_itemListText.rectTransform.offsetMax = new Vector3( 0.0f, -textOffset, 0.0f );
				m_volumeListText.rectTransform.offsetMax = new Vector3( 0.0f, -textOffset, 0.0f );
				m_unitValueListText.rectTransform.offsetMax = new Vector3( 0.0f, -textOffset, 0.0f );

				// update bank balance amount
				m_currentBalanceText.text = "Your account balance is: " + playerData.m_bank.m_currentBalance + " M.U.";
			}
		}

		// enable or disable buttons now
		bool enableButtons = ( m_currentState == State.MenuBar );

		m_buyButton.interactable = enableButtons;
		m_sellButton.interactable = enableButtons;
		m_analyzeButton.interactable = enableButtons;
		m_exitButton.interactable = enableButtons;
	}

	// this is called if we clicked on the buy button
	public void BuyClicked()
	{
		// switch to the buy item state
		SwitchToBuyItemState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the sell button
	public void SellClicked()
	{
		// get access to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// check if the player has something to sell
		if ( ( playerData.m_shipCargo.m_artifactStorage.m_artifactList.Count == 0 ) && ( playerData.m_shipCargo.m_elementStorage.m_elementList.Count == 0 ) )
		{
			// the player has nothing to sell - block the action
			SwitchToErrorMessageState( "Starship hold is empty" );
		}
		else
		{
			// switch to the sell item state
			SwitchToSellItemState();

			// play a ui sound
			GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
		}
	}

	// this is called if we clicked on the analyze button
	public void AnalyzeClicked()
	{
		// get access to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// check if the player has something to analyze
		if ( ( playerData.m_shipCargo.m_artifactStorage.m_artifactList.Count == 0 ) && ( playerData.m_shipCargo.m_elementStorage.m_elementList.Count == 0 ) && ( playerData.m_starportCargo.m_artifactStorage.m_artifactList.Count == 0 ) )
		{
			// the player has nothing to analyze - block the action
			SwitchToErrorMessageState( "There are no artifacts to analyze" );
		}
		else
		{
			// switch to the analyze item state
			SwitchToAnalyzeItemState();

			// play a ui sound
			GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
		}
	}

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		// close this ui
		Hide();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
	}

	// this is called if we clicked on yes to analyze the selected item
	public void YesClicked()
	{
		// get the currently selected item
		Item item = m_itemList[ m_currentItemIndex ];

		// get access to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// deduct the cost of analyzing the artifact from the players bank balance
		playerData.m_bank.m_currentBalance -= gameData.m_starportGameData.m_artifactAnalysisPrice;

		// add artifact to the list of known artifacts
		playerData.m_knownArtifacts.Add( item.m_id );

		// switch to the show analysis state
		SwitchToAnalyzeShowState();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}

	// this is called if we clicked on no to analyze the selected item
	public void NoClicked()
	{
		// switch back to the analyze item state
		SwitchToAnalyzeItemState( false );

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
	}

	// this is called when we hit enter in the amount input field
	public void OnEndEdit()
	{
		// get the amount to transfer
		int desiredAmount = 0;

		if ( m_amountInputField.text.IndexOf( '.' ) == -1 )
		{
			// player did not enter a decimal point
			if ( m_amountInputField.text.Length > 0 )
			{
				desiredAmount = Convert.ToInt32( m_amountInputField.text ) * 10;
			}
		}
		else
		{
			// player did enter a decimal point
			string[] amountParts = m_amountInputField.text.Split( '.' );

			if ( amountParts[ 0 ].Length > 0 )
			{
				desiredAmount = Convert.ToInt32( amountParts[ 0 ] ) * 10;
			}

			if ( amountParts[ 1 ].Length > 0 )
			{
				desiredAmount += Convert.ToInt32( amountParts[ 1 ] );
			}
		}

		// if the desired amount was zero then that's the same as hitting escape
		if ( desiredAmount <= 0 )
		{
			if ( m_currentState == State.BuyAmount )
			{
				// switch back to the buy item state
				SwitchToBuyItemState( false );
			}
			else
			{
				// switch back to the sell item state
				SwitchToSellItemState( false );
			}

			// play a ui sound
			GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
		}
		else
		{
			// get the currently selected element id
			int elementId = m_itemList[ m_currentItemIndex ].m_id;

			// check if we are buying or selling
			if ( m_currentState == State.BuyAmount )
			{
				// check if the player has enough money to buy that amount
				int maximumAmount = GetMaximumBuyAmountDueToCurrentBalance( elementId );

				if ( desiredAmount > maximumAmount )
				{
					SwitchToErrorMessageState( "Insufficent funds" );
				}
				else
				{
					// get access to the player data
					PlayerData playerData = PersistentController.m_instance.m_playerData;

					// check if the ship has room in the cargo hold
					if ( desiredAmount > playerData.m_shipConfiguration.GetRemainingVolme() )
					{
						SwitchToErrorMessageState( "Insufficient cargo space" );
					}
					else
					{
						// get access to the game data
						GameData gameData = PersistentController.m_instance.m_gameData;

						// get the starport price of this element
						int starportPrice = gameData.m_elementList[ elementId ].m_starportPrice;

						// deduct the price of the artifact from the player's bank balance
						playerData.m_bank.m_currentBalance -= starportPrice * desiredAmount / 10;

						// transfer the element to the ship
						playerData.m_shipCargo.AddElement( elementId, desiredAmount );

						// switch back to the buy item state
						SwitchToBuyItemState( false );

						// play a ui sound
						GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
					}
				}
			}
			else
			{
				// get access to the player data
				PlayerData playerData = PersistentController.m_instance.m_playerData;

				// get access to the game data
				GameData gameData = PersistentController.m_instance.m_gameData;

				// get the sell price of this element
				int sellPrice = gameData.m_elementList[ elementId ].m_actualValue;

				// add the sell price of the artifact to the player's bank balance
				playerData.m_bank.m_currentBalance += sellPrice * desiredAmount / 10;

				// transfer the element to starport
				playerData.m_shipCargo.RemoveElement( elementId, desiredAmount );

				// switch back to the sell item state
				SwitchToSellItemState( false );

				// play a ui sound
				GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
			}
		}
	}

	// buy the currently selected item
	private void BuySelectedItem()
	{
		// check if the currently selected item is an element (0) or an artifact (1)
		Item item = m_itemList[ m_currentItemIndex ];

		if ( item.m_type == 0 )
		{
			// it's an element - we need to know how much
			SwitchToBuyAmountState();
		}
		else
		{
			// get access to the game data
			GameData gameData = PersistentController.m_instance.m_gameData;

			// get access to the player data
			PlayerData playerData = PersistentController.m_instance.m_playerData;

			// get access to the artifact data
			ArtifactGameData artifactGameData = gameData.m_artifactList[ item.m_id ];

			// it's an artifact - check if the player can afford it
			if ( playerData.m_bank.m_currentBalance < artifactGameData.m_starportPrice )
			{
				// nope - show an error message
				SwitchToErrorMessageState( "Insufficient funds" );

				Debug.Log( "playerData.m_bankPlayerData.m_currentBalance = " + playerData.m_bank.m_currentBalance );
				Debug.Log( "artifactGameData.m_starportPrice = " + artifactGameData.m_starportPrice );
			}
			else if ( artifactGameData.m_volume > playerData.m_shipConfiguration.GetRemainingVolme() )
			{
				// player's ship has no room for it - show an error message
				SwitchToErrorMessageState( "Insufficient cargo space" );

				Debug.Log( "playerData.m_shipCargoPlayerData.m_volumeUsed = " + playerData.m_shipCargo.m_volumeUsed );
				Debug.Log( "artifactGameData.m_volume = " + artifactGameData.m_volume );
				Debug.Log( "playerData.m_shipConfigurationPlayerData.m_volume = " + playerData.m_shipConfiguration.m_volume );
			}
			else
			{
				// deduct the price of the artifact from the player's bank balance
				playerData.m_bank.m_currentBalance -= gameData.m_artifactList[ item.m_id ].m_starportPrice;

				// transfer the artifact from the starport to the ship
				playerData.m_starportCargo.m_artifactStorage.Remove( item.m_id );
				playerData.m_shipCargo.AddArtifact( item.m_id );

				// update the screen
				UpdateScreen();

				// play a ui sound
				GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
			}
		}
	}

	// sell the currently selected item
	private void SellSelectedItem()
	{
		// check if the currently selected item is an element (0) or an artifact (1)
		Item item = m_itemList[ m_currentItemIndex ];

		if ( item.m_type == 0 )
		{
			// it's an element - we need to know how much
			SwitchToSellAmountState();
		}
		else
		{
			// get access to the game data
			GameData gameData = PersistentController.m_instance.m_gameData;

			// get access to the player data
			PlayerData playerData = PersistentController.m_instance.m_playerData;

			// add the sale price of the artifact to the player's bank balance
			playerData.m_bank.m_currentBalance += gameData.m_artifactList[ item.m_id ].m_actualValue;

			// transfer the artifact from the ship to the starport
			playerData.m_starportCargo.m_artifactStorage.Add( item.m_id );
			playerData.m_shipCargo.RemoveArtifact( item.m_id );

			// update the screen
			UpdateScreen();

			// play a ui sound
			GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
		}
	}

	// analyze the currently selected item
	private void AnalyzeSelectedItem()
	{
		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// get access to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// check if the selected artifact is a known artifact
		Item item = m_itemList[ m_currentItemIndex ];

		if ( playerData.m_knownArtifacts.IsKnown( item.m_id ) )
		{
			// yes we know it - go straight to show analysis
			SwitchToAnalyzeShowState();
		}
		else
		{
			// we do not know it - check if the player can afford it
			if ( playerData.m_bank.m_currentBalance < gameData.m_starportGameData.m_artifactAnalysisPrice )
			{
				// the player cannot afford it
				SwitchToErrorMessageState( "Insufficient funds" );
			}
			else
			{
				// switch to confirm analysis
				SwitchToAnalyzeConfirmState();
			}
		}
	}

	// calculate the maximum amount the player can buy of an element with remaining funds
	private int GetMaximumBuyAmountDueToCurrentBalance( int elementId )
	{
		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// get access to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the starport price of this element
		int starportPrice = gameData.m_elementList[ elementId ].m_starportPrice;

		// get the player's current bank balance
		int currentBalance = playerData.m_bank.m_currentBalance;

		// calculate and return the maximum amount the player can buy
		return currentBalance * 10 / starportPrice;
	}
}
