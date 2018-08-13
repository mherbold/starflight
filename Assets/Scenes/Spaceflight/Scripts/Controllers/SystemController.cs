
using UnityEngine;

public class SystemController : MonoBehaviour
{
	// constants
	public const int c_maxNumOrbits = 8;

	// planet orbit angles
	public float[] m_planetOrbitAngle;

	// planet rotation angles
	public float[] m_planetRotationAngle;

	// orbit number to orbit position map
	public int[] m_orbitNumberToPosition;

	// private stuff we don't want the editor to see
	private int m_currentStarId;

	// convenient access to the spaceflight controller
	private SpaceflightController m_spaceflightController;

	// this is called by unity before start
	private void Awake()
	{
		m_planetOrbitAngle = new float[ c_maxNumOrbits ];
		m_planetRotationAngle = new float[ c_maxNumOrbits ];
		m_orbitNumberToPosition = new int[ c_maxNumOrbits ];
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

			// update the orbit angle
			m_planetOrbitAngle[ i ] = ( playerData.m_starflight.m_gameTime + 1000.0f + ( m_currentStarId * 4 ) ) / daysPerYear * 360.0f;

			// update the rotation angle
			m_planetRotationAngle[ i ] = ( playerData.m_starflight.m_gameTime + i ) * 360.0f;
		}
	}

	// call this to change the current star system
	public void ChangeSystem( int starId )
	{
		// update the current star id
		m_currentStarId = starId;

		// get to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// get to the star data
		StarGameData star = gameData.m_starList[ starId ];

		// create map of orbit number to orbit position
		int currentNumber = 0;

		if ( star.m_hasOrbitalPosition1 )
		{
			m_orbitNumberToPosition[ currentNumber++ ] = 1;
		}

		if ( star.m_hasOrbitalPosition2 )
		{
			m_orbitNumberToPosition[ currentNumber++ ] = 2;
		}

		if ( star.m_hasOrbitalPosition3 )
		{
			m_orbitNumberToPosition[ currentNumber++ ] = 3;
		}

		if ( star.m_hasOrbitalPosition4 )
		{
			m_orbitNumberToPosition[ currentNumber++ ] = 4;
		}

		if ( star.m_hasOrbitalPosition5 )
		{
			m_orbitNumberToPosition[ currentNumber++ ] = 5;
		}

		if ( star.m_hasOrbitalPosition6 )
		{
			m_orbitNumberToPosition[ currentNumber++ ] = 6;
		}

		if ( star.m_hasOrbitalPosition7 )
		{
			m_orbitNumberToPosition[ currentNumber++ ] = 7;
		}

		if ( star.m_hasOrbitalPosition8 )
		{
			m_orbitNumberToPosition[ currentNumber++ ] = 8;
		}

		while ( currentNumber < c_maxNumOrbits )
		{
			m_orbitNumberToPosition[ currentNumber++ ] = -1;
		}

		// update the system display
		m_spaceflightController.m_displayController.m_systemDisplay.ChangeSystem( m_currentStarId );
	}
}
