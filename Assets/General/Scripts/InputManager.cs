
using UnityEngine;

public class InputManager : MonoBehaviour
{
	// private stuff we don't want the editor to see
	public float m_x { get; private set; }
	public float m_y { get; private set; }

	public float m_xRaw { get; private set; }
	public float m_yRaw { get; private set; }

	public bool m_submit { get; private set; }
	public bool m_cancel { get; private set; }

	private bool m_submitDown;
	private bool m_cancelDown;

	public bool m_submitUp { get; private set; }
	public bool m_cancelUp { get; private set; }

	public bool m_debounceNextUpdate;

	// this is called by unity every frame
	void Update()
	{
		// update axis
		m_x = Input.GetAxis( "Horizontal" );
		m_y= Input.GetAxis( "Vertical" );

		// update axis raw
		m_xRaw = Input.GetAxisRaw( "Horizontal" );
		m_yRaw = Input.GetAxisRaw( "Vertical" );

		// update button
		m_submit = Input.GetButton( "Submit" );
		m_cancel = Input.GetButton( "Cancel" );

		// update button down
		m_submitDown = Input.GetButtonDown( "Submit" );
		m_cancelDown = Input.GetButtonDown( "Cancel" );

		// update button up
		m_submitUp = Input.GetButtonUp( "Submit" );
		m_cancelUp = Input.GetButtonUp( "Cancel" );

		// check if we want to debounce the buttons
		if ( m_debounceNextUpdate )
		{
			m_debounceNextUpdate = false;

			m_submitDown = false;
			m_cancelDown = false;

			m_submitUp = false;
			m_cancelUp = false;
		}
	}

	// get the submit down button (with optional debounce)
	public bool GetSubmitDown( bool debounce = true )
	{
		bool submitDown = m_submitDown;

		if ( debounce )
		{
			m_submitDown = false;
		}

		return submitDown;
	}

	// get the cancel down button (with optional debounce)
	public bool GetCancelDown( bool debounce = true )
	{
		bool cancelDown = m_cancelDown;

		if ( debounce )
		{
			m_cancelDown = false;
		}

		return cancelDown;
	}
}
