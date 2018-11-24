
using UnityEngine;

public class InputController : MonoBehaviour
{
	// static reference to this instance
	public static InputController m_instance;

	// submit (fire1) and cancel (fire2) inputs
	public bool m_submit { get; private set; }
	public bool m_cancel { get; private set; }

	// directional arrow inputs
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

	// true if we want to ignore the inputs until everything is released
	bool m_debounce;

	// unity awake
	void Awake()
	{
		// remember this instance to this
		m_instance = this;
	}

	// unity update
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

		// update buttons
		m_submit = Input.GetButton( "Submit" );
		m_cancel = Input.GetButton( "Cancel" );

		// check if we want to debounce the buttons
		if ( m_debounce )
		{
			if ( !m_submit && !m_cancel && !m_north && !m_northEast && !m_east && !m_southEast && !m_south && !m_southWest && !m_west && !m_northWest )
			{
				m_debounce = false;
			}
			else
			{
				Debounce();
			}
		}

		// set x direction
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

		// set y direction
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
	}

	// call this to ignore inputs until everything has been let go
	public void Debounce()
	{
		m_debounce = true;

		m_submit = false;
		m_cancel = false;

		m_north = false;
		m_northEast = false;
		m_east = false;
		m_southEast = false;
		m_south = false;
		m_southWest = false;
		m_west = false;
		m_northWest = false;
	}
}
