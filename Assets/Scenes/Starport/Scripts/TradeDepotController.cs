
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TradeDepotController : PanelController
{
	// public stuff we want to set using the editor
	public Button m_buyButton;
	public Button m_sellButton;
	public Button m_analyzeButton;
	public Button m_exitButton;
	public GameObject m_welcomeGameObject;
	public GameObject m_tradeGameObject;
	public GameObject m_transferPanelGameObject;
	public TextMeshProUGUI m_itemListText;
	public TextMeshProUGUI m_volumeListText;
	public TextMeshProUGUI m_unitValueListText;
	public TextMeshProUGUI m_currentBalanceText;
	public float m_baseOffset;

	// private stuff we don't want the editor to see
	private StarportController m_starportController;
	//private bool m_haveFocus;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the starport controller
		m_starportController = GetComponent<StarportController>();

		// we don't have the focus
		//m_haveFocus = false;
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		// hide the ui
		m_panelGameObject.SetActive( false );
	}

	// call this to show the operations ui
	public override void Show()
	{
		// show the welcome screen
		ShowScreen( 0 );

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
	}

	// call this to take control
	public void TakeFocus()
	{
		// we have the controller focus
		//m_haveFocus = true;

		// turn on controller navigation of the UI
		EventSystem.current.sendNavigationEvents = true;

		// automatically select the "notices" button for the player
		m_exitButton.Select();

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

	// this is called if we clicked on the buy button
	public void BuyClicked()
	{
		// show the trade screen
		ShowScreen( 1 );

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the sell button
	public void SellClicked()
	{
		// show the trade screen
		ShowScreen( 1 );

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the analyze button
	public void AnalyzeClicked()
	{
		// show the trade screen
		ShowScreen( 1 );

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

	// show one of the two screens in the trade depot
	private void ShowScreen( int screenIndex )
	{
		m_welcomeGameObject.SetActive( screenIndex == 0 ? true : false );
		m_tradeGameObject.SetActive( screenIndex == 1 ? true : false );

		// hide the transfer panel
		m_transferPanelGameObject.SetActive( false );
	}
}
