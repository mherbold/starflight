
using UnityEngine;

public class SystemDisplay : Display
{
	private GameObject[] m_orbitGameObject;

	public SystemDisplay( GameObject rootGameObject ) : base( rootGameObject )
	{
		m_orbitGameObject = new GameObject[ SystemController.c_maxNumOrbits ];

		for ( int i = 0; i < SystemController.c_maxNumOrbits; i++ )
		{
			Transform transform = m_rootGameObject.transform.Find( "GameObject/Orbit-" + ( i + 1 ) );

			m_orbitGameObject[ i ] = transform.gameObject;
		}
	}

	public override string GetLabel()
	{
		return "System";
	}

	public override void Start()
	{
		// turn on the system display
		m_rootGameObject.SetActive( true );
	}

	public override void Update()
	{
		// update the positions of the planets
		for ( int i = 0; i < SystemController.c_maxNumOrbits; i++ )
		{
			float angle = m_spaceflightController.m_systemController.m_planetOrbitAngle[ i ];

			Quaternion rotation = Quaternion.AngleAxis( angle, Vector3.forward );

			m_orbitGameObject[ i ].transform.rotation = rotation;
		}
	}

	public void ChangeSystem( int starId )
	{
		// get to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// get to the star data
		StarGameData star = gameData.m_starList[ starId ];

		// turn on or off each orbit
		m_orbitGameObject[ 0 ].SetActive( star.m_hasOrbitalPosition1 );
		m_orbitGameObject[ 1 ].SetActive( star.m_hasOrbitalPosition2 );
		m_orbitGameObject[ 2 ].SetActive( star.m_hasOrbitalPosition3 );
		m_orbitGameObject[ 3 ].SetActive( star.m_hasOrbitalPosition4 );
		m_orbitGameObject[ 4 ].SetActive( star.m_hasOrbitalPosition5 );
		m_orbitGameObject[ 5 ].SetActive( star.m_hasOrbitalPosition6 );
		m_orbitGameObject[ 6 ].SetActive( star.m_hasOrbitalPosition7 );
		m_orbitGameObject[ 7 ].SetActive( star.m_hasOrbitalPosition8 );
	}
}
