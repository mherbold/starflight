
using UnityEngine;
using TMPro;

public class Messages : MonoBehaviour
{
	// the time it takes to fully open or close
	public float m_slideDuration;

	// the frame
	public RectTransform m_frame;

	// text elements
	TextMeshProUGUI m_messagesUI;

	// the current state of the dock (out or in)
	bool m_isOut;

	// do we want to slide the dock?
	bool m_isSliding;

	// the slide time
	float m_slideTime;

	// keep track of whether or not we need to update the messages ui
	bool m_textChanged;

	// unity awake
	void Awake()
	{
		// dock is in
		m_isOut = false;

		// dock is not sliding
		m_isSliding = false;

		// get the text mesh pro component
		m_messagesUI = GetComponent<TextMeshProUGUI>();
	}

	// unity update
	void LateUpdate()
	{
		// update the display if the text has changed (and save the game)
		if ( m_textChanged )
		{
			m_textChanged = false;

			UpdateDisplay();
		}

		// update sliding animation
		if ( m_isSliding )
		{
			float from, to;

			if ( m_isOut )
			{
				from = -524.0f;
				to = -12.0f;
			}
			else
			{
				from = -12.0f;
				to = -524.0f;
			}

			m_slideTime += Time.deltaTime;

			if ( m_slideTime >= m_slideDuration )
			{
				m_slideTime = m_slideDuration;
				m_isSliding = false;
				m_isOut = !m_isOut;
			}

			var x = Mathf.SmoothStep( from, to, m_slideTime );

			var offsetMin = m_frame.offsetMin;

			offsetMin.x = x;

			m_frame.offsetMin = offsetMin;
		}
	}

	// update the display with the current message lines
	void UpdateDisplay()
	{
		var playerData = DataController.m_instance.m_playerData;

		m_messagesUI.text = "";

		for ( var i = 0; i < playerData.m_general.m_messageList.Count; i++ )
		{
			if ( i != 0 )
			{
				m_messagesUI.text += "\n<line-height=25%>\n</line-height>";
			}

			m_messagesUI.text += playerData.m_general.m_messageList[ i ];
		}

		m_messagesUI.ForceMeshUpdate();

		RectTransform rectTransform = GetComponent<RectTransform>();

		var rectHeight = rectTransform.rect.height;

		m_messagesUI.alignment = ( m_messagesUI.renderedHeight > rectHeight ) ? TextAlignmentOptions.BottomLeft : TextAlignmentOptions.TopLeft;
	}

	// call this to clear out all the messages
	public void Clear()
	{
		var playerData = DataController.m_instance.m_playerData;

		playerData.m_general.m_messageList.Clear();

		m_textChanged = true;
	}

	// call this to change the message text
	public void AddText( string newMessage )
	{
		var playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_general.m_messageList.Count == 10 )
		{
			playerData.m_general.m_messageList.RemoveAt( 0 );
		}

		playerData.m_general.m_messageList.Add( newMessage );

		m_textChanged = true;
	}

	// call this to force a refresh
	public void Refresh()
	{
		m_textChanged = true;
	}

	// call this to slide out the messages "dock"
	public void SlideOut()
	{
		if ( m_isOut )
		{ 
			if ( m_isSliding )
			{
				m_isOut = false;
				m_slideTime = m_slideDuration - m_slideTime;
			}
		}
		else
		{
			m_isSliding = true;
			m_slideTime = 0.0f;
		}
	}

	// call this to slide back in the messages "dock"
	public void SlideIn()
	{
		if ( !m_isOut )
		{
			if ( m_isSliding )
			{
				m_isOut = true;
				m_slideTime = m_slideDuration - m_slideTime;
			}
		}
		else
		{
			m_isSliding = true;
			m_slideTime = 0.0f;
		}
	}
}
