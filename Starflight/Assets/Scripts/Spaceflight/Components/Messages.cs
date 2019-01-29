
using UnityEngine;
using TMPro;

public class Messages : MonoBehaviour
{
	// the time it takes to fully open or close
	public float m_slideDuration;

	// the frame
	public RectTransform m_frame;

	// text elements
	public TextMeshProUGUI m_messages;

	// the current state of the dock (out or in)
	bool m_isOut;

	// do we want to slide the dock?
	bool m_isSliding;

	// the slide time
	float m_slideTime;

	// unity awake
	void Awake()
	{
		// dock is in
		m_isOut = false;

		// dock is not sliding
		m_isSliding = false;
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
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

	// call this to change the message text
	public void ChangeText( string newMessage )
	{
		m_messages.text = newMessage;

		m_messages.ForceMeshUpdate();

		RectTransform rectTransform = GetComponent<RectTransform>();

		var rectHeight = rectTransform.rect.height;

		m_messages.alignment = ( m_messages.renderedHeight > rectHeight ) ? TextAlignmentOptions.BottomLeft : TextAlignmentOptions.TopLeft;
	}

	// call this to change the message text
	public void AddText( string newMessage )
	{
		ChangeText( m_messages.text + "\n" + newMessage );
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
