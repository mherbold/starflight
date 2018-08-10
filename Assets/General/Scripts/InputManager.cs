
using UnityEngine;

public class InputManager : MonoBehaviour
{
	// private stuff we don't want the editor to see
	public bool m_submit { get; private set; }
	public bool m_cancel { get; private set; }

	private bool m_submitDown;
	private bool m_cancelDown;

	public bool m_submitUp { get; private set; }
	public bool m_cancelUp { get; private set; }

	public bool m_northWest { get; private set; }
	public bool m_north { get; private set; }
	public bool m_northEast { get; private set; }
	public bool m_east { get; private set; }
	public bool m_southEast { get; private set; }
	public bool m_south { get; private set; }
	public bool m_southWest { get; private set; }
	public bool m_west { get; private set; }

	public float m_x { get; private set; }
	public float m_y { get; private set; }

	public bool m_debounceNextUpdate;

	// this is called by unity every frame
	void Update()
	{
		// update directional movement
		m_northWest = Input.GetButton( "NW" );
		m_north = Input.GetButton( "N" );
		m_northEast = Input.GetButton( "NE" );
		m_east = Input.GetButton( "E" );
		m_southEast = Input.GetButton( "SE" );
		m_south = Input.GetButton( "S" );
		m_southWest = Input.GetButton( "SW" );
		m_west = Input.GetButton( "W" );

		if ( m_northWest || m_west || m_southWest )
		{
			m_x = -1.0f;
		}
		else if ( m_northEast || m_east || m_southEast )
		{
			m_x = 1.0f;
		}
		else
		{
			m_x = 0.0f;
		}

		if ( m_northWest || m_north || m_northEast )
		{
			m_y = 1.0f;
		}
		else if ( m_southWest || m_south || m_southEast )
		{
			m_y = -1.0f;
		}
		else
		{
			m_y = 0.0f;
		}

		// update buttons
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
