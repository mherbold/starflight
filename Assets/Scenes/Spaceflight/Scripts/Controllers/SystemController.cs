
using UnityEngine;

public class SystemController : MonoBehaviour
{
	// our planet controllers
	public PlanetController[] m_planetController;

	// the star id of the system we are currently in
	private int m_currentStarId;

	// convenient access to the spaceflight controller
	private SpaceflightController m_spaceflightController;

	// this is called by unity before start
	private void Awake()
	{
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
	}

	// call this to change the current star system
	public void EnterSystem( int starId )
	{
		// update the current star id
		m_currentStarId = starId;

		// get to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// get to the star data
		Star star = gameData.m_starList[ starId ];

		// turn off all the planets
		for ( int i = 0; i < Star.c_maxNumPlanets; i++ )
		{
			m_planetController[ i ].SetPlanet( null );
		}

		// turn on planets in this system
		foreach ( Planet planet in star.m_planetList )
		{
			if ( planet != null )
			{
				m_planetController[ planet.m_orbitPosition ].SetPlanet( planet );
			}
		}

		// update the system display
		m_spaceflightController.m_displayController.m_systemDisplay.ChangeSystem( m_currentStarId );
	}
}
