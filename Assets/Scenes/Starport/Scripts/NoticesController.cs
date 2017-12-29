
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class NoticesController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public GameObject m_gameObject;
	public TextMeshProUGUI m_stardateText;
	public TextMeshProUGUI m_messageText;
	public float m_baseOffset;
	public float m_smoothScrollSpeed;

	// private stuff we don't want the editor to see
	private OperationsController m_operationsController;
	private int m_latestNoticeIndex;
	private int m_currentNoticeIndex;
	private int m_currentLine;
	private float m_currentOffset;
	private bool m_endOfMessageReached;
	// private bool m_haveFocus;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the operations controller
		m_operationsController = GetComponent<OperationsController>();

		// we don't have the focus
		//m_haveFocus = false;
	}

	// this is called by unity every frame
	private void Update()
	{
		// smooth scroll the displayed message
		UpdateSmoothScroll();
	}

	// call this to smooth scroll the displayed message
	private void UpdateSmoothScroll()
	{
		// get the height of the message shown
		float newOffset = m_messageText.renderedHeight + m_baseOffset;

		// smooth scroll the position (y)
		m_currentOffset = Mathf.Lerp( m_currentOffset, newOffset, m_smoothScrollSpeed * Time.deltaTime );

		// move up the message text game object
		m_messageText.rectTransform.offsetMax = new Vector3( 0.0f, m_currentOffset, 0.0f );
	}

	// call this to enter the notices screen
	public void Show()
	{
		// we have the controller focus now
		TakeFocus();

		// reset some variables
		m_latestNoticeIndex = 0;
		m_currentNoticeIndex = 0;

		// get access to the player progress
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// update the stardate text
		DateTime dateTime = DateTime.ParseExact( playerData.m_starflightPlayerData.m_currentStardate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );
		m_stardateText.text = "Today is " + dateTime.ToLongDateString();

		// figure out which notice we should be showing (current notice)
		string earliestNewNoticeStardate = "9999-12-31";

		for ( int i = 0; i < gameData.m_noticeList.Length; i++ )
		{
			NoticeGameData notice = gameData.m_noticeList[ i ];

			if ( string.Compare( playerData.m_starflightPlayerData.m_currentStardate, notice.m_stardate ) >= 0 )
			{
				m_latestNoticeIndex = i;

				if ( string.Compare( notice.m_stardate, playerData.m_noticePlayerData.m_lastReadStardate ) >= 0 )
				{
					if ( string.Compare( earliestNewNoticeStardate, notice.m_stardate ) >= 0 )
					{
						if ( notice.m_stardate != playerData.m_noticePlayerData.m_lastReadStardate )
						{
							earliestNewNoticeStardate = notice.m_stardate;
						}

						m_currentNoticeIndex = i;
					}
				}
			}
		}

		// show the current message
		ShowCurrentMessage();
	}

	// call this to return to the operations screen
	public void Hide()
	{
		// give focus to the operations controller
		m_operationsController.TakeFocus();
	}

	// call this to take control
	public void TakeFocus()
	{
		// we have the controller focus
		//m_haveFocus = true;

		// turn on controller navigation of the UI
		EventSystem.current.sendNavigationEvents = true;
	}

	// call this to give up control
	public void LoseFocus()
	{
		// we have the controller focus
		//m_haveFocus = false;

		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;
	}

	// this is called if we clicked on the notices button in the operations panel
	public void MoreClicked()
	{
		// show the next line of the notice
		ShowNextLine();

		// update the buttons
		UpdateButtons();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}

	// this is called if we clicked on the evaluation button in the operations panel
	public void PreviousClicked()
	{
		if ( m_currentNoticeIndex > 0 )
		{
			m_currentNoticeIndex--;

			ShowCurrentMessage();
		}

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}

	// this is called if we clicked on the exit button in the operations panel
	public void NextClicked()
	{
		if ( m_currentNoticeIndex < m_latestNoticeIndex )
		{
			m_currentNoticeIndex++;

			ShowCurrentMessage();
		}

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Update );
	}

	// this is called if we clicked on the exit button in the operations panel
	public void CloseClicked()
	{
		// leave the notices screen
		Hide();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Deactivate );
	}

	// call this to start displaying the current message
	private void ShowCurrentMessage()
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
	private void UpdateButtons()
	{
		Button moreButton = m_gameObject.transform.Find( "Panel/Buttons/More Button" ).gameObject.GetComponent<Button>();
		Button previousButton = m_gameObject.transform.Find( "Panel/Buttons/Previous Button" ).gameObject.GetComponent<Button>();
		Button nextButton = m_gameObject.transform.Find( "Panel/Buttons/Next Button" ).gameObject.GetComponent<Button>();
		Button quitButton = m_gameObject.transform.Find( "Panel/Buttons/Quit Button" ).gameObject.GetComponent<Button>();

		// select the quit button by default
		quitButton.Select();

		// enable / disable the previous and next buttons based on what our current notice index is
		previousButton.interactable = ( m_currentNoticeIndex > 0 );
		nextButton.interactable = ( m_currentNoticeIndex < m_latestNoticeIndex );

		// check if we have reached the end of the current notice
		if ( m_endOfMessageReached )
		{
			moreButton.interactable = false;

			if ( m_currentNoticeIndex < m_latestNoticeIndex )
			{
				nextButton.Select();
			}
		}
		else
		{
			moreButton.interactable = true;

			moreButton.Select();
		}
	}

	// call this to show the next line of the current message
	private void ShowNextLine()
	{
		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// get the current notice
		NoticeGameData currentNotice = gameData.m_noticeList[ m_currentNoticeIndex ];

		// check if we are displaying the first line
		if ( m_currentLine == 0 )
		{
			// clear the text and add the date
			DateTime messageDate = DateTime.ParseExact( currentNotice.m_stardate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );
			m_messageText.text = messageDate.ToLongDateString();

			// remember the newest notice read
			if ( string.Compare( currentNotice.m_stardate, PersistentController.m_instance.m_playerData.m_noticePlayerData.m_lastReadStardate ) > 0 )
			{
				PersistentController.m_instance.m_playerData.m_noticePlayerData.m_lastReadStardate = currentNotice.m_stardate;

				PersistentController.m_instance.SavePlayerData();
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
