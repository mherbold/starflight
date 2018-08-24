
using UnityEngine;

public class SystemController : MonoBehaviour
{
	// constants
	public const int c_maxNumPlanets = 8;

	// our planet controllers
	public PlanetController[] m_planetController;

	// private stuff we don't want the editor to see
	private int m_currentStarId;

	// convenient access to the spaceflight controller
	private SpaceflightController m_spaceflightController;

	// this is called by unity before start
	private void Awake()
	{
		m_planetController = new PlanetController[ c_maxNumPlanets ];
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

		// update the planets
		for ( int i = 0; i < c_maxNumPlanets; i++ )
		{
			m_planetController[ i ].Update();
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

		// turn off all the planets
		for ( int i = 0; i < c_maxNumPlanets; i++ )
		{
			m_planetController[ i ].Change( -1 );
		}

		// turn on planets in this system
		for ( int i = 0; i < gameData.m_planetList.Length; i++ )
		{
			PlanetGameData planetGameData = gameData.m_planetList[ i ];

			if ( planetGameData.m_starId == starId )
			{
				m_planetController[ planetGameData.m_orbitPosition ].Change( i );
			}
		}

		// update the system display
		m_spaceflightController.m_displayController.m_systemDisplay.ChangeSystem( m_currentStarId );
	}
}
