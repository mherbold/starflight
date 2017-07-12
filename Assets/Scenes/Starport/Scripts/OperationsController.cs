
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OperationsController : PanelController
{
	// public stuff we want to set using the editor
	public Button m_noticesButton;
	public Button m_evaluationButton;
	public Button m_exitButton;
	public Button m_moreButton;
	public Button m_previousButton;
	public Button m_nextButton;
	public Button m_quitButton;
	public GameObject m_welcomeGameObject;
	public GameObject m_noticesGameObject;
	public GameObject m_evaluationGameObject;

	// private stuff we don't want the editor to see
	private StarportController m_starportController;
	private NoticesController m_noticesController;
	//private bool m_haveFocus;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the starport controller
		m_starportController = GetComponent<StarportController>();

		// get access to the notices controller
		m_noticesController = GetComponent<NoticesController>();

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
	}

	// call this to show the operations ui
	public override void Show()
	{
		// show the welcome screen
		ShowScreen( 0 );

		// show the main buttons
		ShowButtons( 0 );

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

		// show the welcome screen
		ShowScreen( 0 );

		// show the main buttons
		ShowButtons( 0 );

		// automatically select the "notices" button for the player
		m_noticesButton.Select();

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

	// this is called if we clicked on the notices button
	public void NoticesClicked()
	{
		// show the notices screen
		ShowScreen( 1 );

		// show the notices buttons
		ShowButtons( 1 );

		// give up the focus
		LoseFocus();

		// show the notices
		m_noticesController.Show();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Activate );
	}

	// this is called if we clicked on the evaluation button
	public void EvaluationClicked()
	{
		// show the evaluations screen
		ShowScreen( 2 );

		// show the main buttons
		ShowButtons( 0 );

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

	// show one of the three screens in operations
	private void ShowScreen( int screenIndex )
	{
		m_welcomeGameObject.SetActive( screenIndex == 0 ? true : false );
		m_noticesGameObject.SetActive( screenIndex == 1 ? true : false );
		m_evaluationGameObject.SetActive( screenIndex == 2 ? true : false );
	}

	// show one of the two button sets
	private void ShowButtons( int buttonSetIndex )
	{
		bool firstSetActive = ( buttonSetIndex == 0 ? true : false );

		m_noticesButton.gameObject.SetActive( firstSetActive );
		m_evaluationButton.gameObject.SetActive( firstSetActive );
		m_exitButton.gameObject.SetActive( firstSetActive );

		bool secondSetActive = ( buttonSetIndex == 1 ? true : false );

		m_moreButton.gameObject.SetActive( secondSetActive );
		m_previousButton.gameObject.SetActive( secondSetActive );
		m_nextButton.gameObject.SetActive( secondSetActive );
		m_quitButton.gameObject.SetActive( secondSetActive );
	}
}
