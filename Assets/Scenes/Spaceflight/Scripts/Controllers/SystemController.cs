
using UnityEngine;

public class SystemController : MonoBehaviour
{
	// constants
	public const int c_maxNumOrbits = 8;

	// planet orbit angles
	public float[] m_planetOrbitAngle;

	// planet rotation angles
	public float[] m_planetRotationAngle;

	// private stuff we don't want the editor to see
	private int m_currentStarId;

	// convenient access to the spaceflight controller
	private SpaceflightController m_spaceflightController;

	// this is called by unity before start
	private void Awake()
	{
		m_planetOrbitAngle = new float[ c_maxNumOrbits ];
		m_planetRotationAngle = new float[ c_maxNumOrbits ];
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();

		// stop here if the spaceflight contoller has not started
		if ( !m_spaceflightController.m_started )
		{
			return;
		}

		// reset stuff
		m_currentStarId = 0;
	}

	// this is called by unity every frame
	private void Update()
	{
		// stop here if the spaceflight contoller has not started
		if ( !m_spaceflightController.m_started )
		{
			return;
		}

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		for ( int i = 0; i < c_maxNumOrbits; i++ )
		{
			// calculate number of days per year for each planet based on orbit number - orbit 3 should = 366 days like earth
			int daysPerYear = 122 * ( i + 1 );

			m_planetOrbitAngle[ i ] = playerData.m_starflight.m_gameTime / daysPerYear * 360.0f;
			m_planetRotationAngle[ i ] = playerData.m_starflight.m_gameTime * 360.0f;

			// apply a pseudo-random offset to the orbit angle based on star id and orbit number
			m_planetOrbitAngle[ i ] += m_currentStarId - i * ( 360.0f / c_maxNumOrbits ) * 2.0f;

			// apply an offset to the rotation angle based on the orbit number
			m_planetRotationAngle[ i ] += i * ( 360.0f / c_maxNumOrbits );
		}
	}

	// call this to change the current star system
	public void ChangeSystem( int starId )
	{
		// update the current star id
		m_currentStarId = starId;

		// update the system display
		m_spaceflightController.m_displayController.m_systemDisplay.ChangeSystem( m_currentStarId );
	}
}
