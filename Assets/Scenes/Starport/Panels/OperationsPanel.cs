
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OperationsPanel : Panel
{
	// the screens
	public GameObject m_welcomeGameObject;
	public GameObject m_noticesGameObject;
	public GameObject m_evaluationGameObject;

	// main buttons
	public Button m_noticesButton;
	public Button m_evaluationButton;
	public Button m_exitButton;

	// notices buttons
	public Button m_moreButton;
	public Button m_previousButton;
	public Button m_nextButton;
	public Button m_quitButton;

	// notice screen components
	public TextMeshProUGUI m_stardateText;
	public TextMeshProUGUI m_messageText;
	public float m_baseOffset;
	public float m_smoothScrollSpeed;

	// notices stuff
	int m_latestNoticeId;
	int m_currentNoticeId;
	int m_currentLine;
	float m_currentOffset;
	bool m_endOfMessageReached;

	// the starport controller
	public StarportController m_starportController;

	// panel open
	public override bool Open()
	{
		// base panel open
		base.Open();

		// show the welcome screen
		ShowWelcome();

		// panel was opened
		return true;
	}

	// panel closed
	public override void Closed()
	{
		// base panel closed
		base.Closed();

		// let the starport controller know
		m_starportController.PanelWasClosed();
	}

	// panel tick
	public override void Tick()
	{
		// get the height of the message shown
		float newOffset = m_messageText.renderedHeight + m_baseOffset;

		// smooth scroll the position (y)
		m_currentOffset = Mathf.Lerp( m_currentOffset, newOffset, m_smoothScrollSpeed * Time.deltaTime );

		// move up the message text game object
		m_messageText.rectTransform.offsetMax = new Vector3( 0.0f, m_currentOffset, 0.0f );
	}

	// call this to enter the welcome screen
	public void ShowWelcome()
	{
		// show and hide objects
		m_welcomeGameObject.SetActive( true );
		m_noticesGameObject.SetActive( false );
		m_evaluationGameObject.SetActive( false );

		// show the main buttons
		m_noticesButton.gameObject.SetActive( true );
		m_evaluationButton.gameObject.SetActive( true );
		m_exitButton.gameObject.SetActive( true );

		// hide the notices buttons
		m_moreButton.gameObject.SetActive( false );
		m_previousButton.gameObject.SetActive( false );
		m_nextButton.gameObject.SetActive( false );
		m_quitButton.gameObject.SetActive( false );

		// automatically select the "notices" button for the player
		m_noticesButton.Select();
	}

	// call this to enter the notices screen
	public void ShowNotices()
	{
		// show and hide objects
		m_welcomeGameObject.SetActive( false );
		m_noticesGameObject.SetActive( true );
		m_evaluationGameObject.SetActive( false );

		// hide the main buttons
		m_noticesButton.gameObject.SetActive( false );
		m_evaluationButton.gameObject.SetActive( false );
		m_exitButton.gameObject.SetActive( false );

		// show the notices buttons
		m_moreButton.gameObject.SetActive( true );
		m_previousButton.gameObject.SetActive( true );
		m_nextButton.gameObject.SetActive( true );
		m_quitButton.gameObject.SetActive( true );

		// reset some variables
		m_latestNoticeId = 0;
		m_currentNoticeId = 0;

		// get access to the player progress
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get access to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// update the stardate text
		DateTime dateTime = DateTime.ParseExact( playerData.m_starflight.m_currentStardateYMD, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );
		m_stardateText.text = "Today is " + dateTime.ToLongDateString();

		// figure out which notice we should be showing (current notice)
		string earliestNewNoticeStardate = "9999-12-31";

		for ( int noticeId = 0; noticeId < gameData.m_noticeList.Length; noticeId++ )
		{
			Notice notice = gameData.m_noticeList[ noticeId ];

			if ( string.Compare( playerData.m_starflight.m_currentStardateYMD, notice.m_stardate ) >= 0 )
			{
				m_latestNoticeId = noticeId;

				if ( string.Compare( notice.m_stardate, playerData.m_starport.m_lastReadNoticeStardate ) >= 0 )
				{
					if ( string.Compare( earliestNewNoticeStardate, notice.m_stardate ) >= 0 )
					{
						if ( notice.m_stardate != playerData.m_starport.m_lastReadNoticeStardate )
						{
							earliestNewNoticeStardate = notice.m_stardate;
						}

						m_currentNoticeId = noticeId;
					}
				}
			}
		}

		// show the current message
		ShowCurrentMessage();
	}

	// call this to enter the evaluations screen
	public void ShowEvaluations()
	{
		// show and hide objects
		m_welcomeGameObject.SetActive( false );
		m_noticesGameObject.SetActive( false );
		m_evaluationGameObject.SetActive( true );

		// show the main buttons
		m_noticesButton.gameObject.SetActive( true );
		m_evaluationButton.gameObject.SetActive( true );
		m_exitButton.gameObject.SetActive( true );

		// hide the notices buttons
		m_moreButton.gameObject.SetActive( false );
		m_previousButton.gameObject.SetActive( false );
		m_nextButton.gameObject.SetActive( false );
		m_quitButton.gameObject.SetActive( false );
	}

	// this is called if we clicked on the notices button
	public void NoticesClicked()
	{
		// show the notices
		ShowNotices();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the evaluation button
	public void EvaluationClicked()
	{
		// show the evaluations screen
		ShowEvaluations();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		// close this panel
		PanelController.m_instance.Close();
	}

	// this is called if we clicked on the more button
	public void MoreClicked()
	{
		// show the next line of the notice
		ShowNextLine();

		// update the buttons
		UpdateButtons();
	}

	// this is called if we clicked on the prev button
	public void PreviousClicked()
	{
		if ( m_currentNoticeId > 0 )
		{
			m_currentNoticeId--;

			ShowCurrentMessage();
		}

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Update );
	}

	// this is called if we clicked on the next button
	public void NextClicked()
	{
		if ( m_currentNoticeId < m_latestNoticeId )
		{
			m_currentNoticeId++;

			ShowCurrentMessage();
		}

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Update );
	}

	// this is called if we clicked on the quit button
	public void QuitClicked()
	{
		// show the welcome screen
		ShowWelcome();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
	}

	// call this to start displaying the current message
	void ShowCurrentMessage()
	{
		// reset some variables
		m_currentLine = 0;
		m_currentOffset = 0;
		m_endOfMessageReached = false;

		// show the first line of the current message
		ShowNextLine();

		// update the buttons
		UpdateButtons();
	}

	// update which buttons are enabled and selected
	void UpdateButtons()
	{
		// select the quit button by default
		m_quitButton.Select();

		// enable / disable the previous and next buttons based on what our current notice index is
		m_previousButton.interactable = ( m_currentNoticeId > 0 );
		m_nextButton.interactable = ( m_currentNoticeId < m_latestNoticeId );

		// check if we have reached the end of the current notice
		if ( m_endOfMessageReached )
		{
			m_moreButton.interactable = false;

			if ( m_currentNoticeId < m_latestNoticeId )
			{
				m_nextButton.Select();
			}
		}
		else
		{
			m_moreButton.interactable = true;

			m_moreButton.Select();
		}
	}

	// call this to show the next line of the current message
	void ShowNextLine()
	{
		// get access to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get the current notice
		Notice currentNotice = gameData.m_noticeList[ m_currentNoticeId ];

		// check if we are displaying the first line
		if ( m_currentLine == 0 )
		{
			// clear the text and add the date
			DateTime messageDate = DateTime.ParseExact( currentNotice.m_stardate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );
			m_messageText.text = messageDate.ToLongDateString();

			// remember the newest notice read
			if ( string.Compare( currentNotice.m_stardate, DataController.m_instance.m_playerData.m_starport.m_lastReadNoticeStardate ) > 0 )
			{
				DataController.m_instance.m_playerData.m_starport.m_lastReadNoticeStardate = currentNotice.m_stardate;
			}
		}

		// get the current line of the message
		string[] subStrings = currentNotice.m_message.Split( new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );

		if ( m_currentLine < subStrings.Length )
		{
			m_messageText.text += "\r\n\r\n" + subStrings[ m_currentLine ];
			m_currentLine++;

			// check if we have reached the end of this message
			if ( m_currentLine == subStrings.Length )
			{
				m_endOfMessageReached = true;
			}
		}
	}
}
