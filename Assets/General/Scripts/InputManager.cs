
using UnityEngine;

class InputManager : MonoBehaviour
{
	// private stuff we don't want the editor to see
	private float m_x;
	private float m_y;
	private float m_xRaw;
	private float m_yRaw;
	private bool m_submit;
	private bool m_cancel;
	private bool m_submitDown;
	private bool m_cancelDown;
	private bool m_submitUp;
	private bool m_cancelUp;
	private bool m_debounceNextUpdate;

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

			DebounceNow();
		}
	}

	// get the x axis
	public float GetX()
	{
		return m_x;
	}

	// get the y axis
	public float GetY()
	{
		return m_y;
	}

	// get the raw x axis
	public float GetRawX()
	{
		return m_xRaw;
	}

	// get the raw y axis
	public float GetRawY()
	{
		return m_yRaw;
	}

	// get the submit button
	public bool Submit()
	{
		return m_submit;
	}

	// get the cancel button
	public bool Cancel()
	{
		return m_cancel;
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

	// get the submit up button
	public bool GetSubmitUp()
	{
		return m_submitUp;
	}

	// get the cancel up button
	public bool GetCancelUp()
	{
		return m_cancelUp;
	}

	// call this to reset all the buttons on the next frame
	public void DebounceNextUpdate()
	{
		m_debounceNextUpdate = true;
	}

	// call this to reset all the buttons now
	public void DebounceNow()
	{
		m_submitDown = false;
		m_cancelDown = false;

		m_submitUp = false;
		m_cancelUp = false;
	}
}
