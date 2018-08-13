
using UnityEngine;
using UnityEngine.UI;

public class SystemDisplay : Display
{
	private GameObject[] m_orbitGameObject;
	private Image [] m_planetImage;
	private GameObject m_arthGameObject;
	private Image m_sunImage;
	private GameObject m_shipGameObject;

	public SystemDisplay( GameObject rootGameObject ) : base( rootGameObject )
	{
		Transform transform;

		// allocate arrays
		m_orbitGameObject = new GameObject[ SystemController.c_maxNumOrbits ];
		m_planetImage = new Image[ SystemController.c_maxNumOrbits ];

		// get to the orbits
		for ( int i = 0; i < SystemController.c_maxNumOrbits; i++ )
		{
			// get the orbit game object
			transform = m_rootGameObject.transform.Find( "GameObject/Orbit-" + ( i + 1 ) );
			m_orbitGameObject[ i ] = transform.gameObject;

			// get the planet image
			transform = m_orbitGameObject[ i ].transform.Find( "Planet" );
			m_planetImage[ i ] = transform.GetComponent<Image>();
		}

		// get to the arth game object
		transform = m_orbitGameObject[ 3 ].transform.Find( "Arth" );
		m_arthGameObject = transform.gameObject;

		// get to the sun image
		transform = m_rootGameObject.transform.Find( "GameObject/Star" );
		m_sunImage = transform.GetComponent<Image>();

		// get to the ship game object
		transform = m_rootGameObject.transform.Find( "GameObject/Ship" );
		m_shipGameObject = transform.gameObject;
	}

	public override string GetLabel()
	{
		return "System Map";
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

		// update the position of the ship
		Vector3 position = m_spaceflightController.m_camera.transform.position * 0.05f;
		m_shipGameObject.transform.localPosition = new Vector3( position.x, position.z );
	}

	public void ChangeSystem( int starId )
	{
		// get to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// get to the star data
		StarGameData star = gameData.m_starList[ starId ];

		// turn off the arth game object
		m_arthGameObject.SetActive( false );

		// enable the 4th planet game object (in case it was previously disabled)
		m_planetImage[ 3 ].gameObject.SetActive( true );

		// change the color of the star based on the class
		Color color;

		switch ( star.m_class )
		{
			case "M": color = new Color( 1.0f, 0.0f, 0.0f ); break;
			case "K": color = new Color( 1.0f, 0.4f, 0.0f ); break;
			case "G": color = new Color( 1.0f, 1.0f, 0.0f ); break;
			case "F": color = new Color( 1.0f, 1.0f, 1.0f ); break;
			case "A": color = new Color( 0.0f, 1.0f, 0.0f ); break;
			case "B": color = new Color( 0.4f, 0.4f, 1.0f ); break;
			case "O": color = new Color( 0.0f, 0.0f, 0.8f ); break;
			default: color = new Color( 1.0f, 0.5f, 1.0f ); break;
		}

		m_sunImage.color = color;

		// turn on or off each orbit
		m_orbitGameObject[ 0 ].SetActive( star.m_hasOrbitalPosition1 );
		m_orbitGameObject[ 1 ].SetActive( star.m_hasOrbitalPosition2 );
		m_orbitGameObject[ 2 ].SetActive( star.m_hasOrbitalPosition3 );
		m_orbitGameObject[ 3 ].SetActive( star.m_hasOrbitalPosition4 );
		m_orbitGameObject[ 4 ].SetActive( star.m_hasOrbitalPosition5 );
		m_orbitGameObject[ 5 ].SetActive( star.m_hasOrbitalPosition6 );
		m_orbitGameObject[ 6 ].SetActive( star.m_hasOrbitalPosition7 );
		m_orbitGameObject[ 7 ].SetActive( star.m_hasOrbitalPosition8 );

		// change the color of the planets
		for ( int planetId = 0; planetId < gameData.m_planetList.Length; planetId++ )
		{
			PlanetGameData planet = gameData.m_planetList[ planetId ];

			if ( planet.m_starId == starId )
			{
				// check if this is the arth station (special case)
				if ( planet.m_planetTypeId == 57 )
				{
					// yep - hide the planet object and show the arth station instead
					m_planetImage[ 3 ].gameObject.SetActive( false );
					m_arthGameObject.SetActive( true );
				}
				else
				{
					PlanetTypeGameData planetType = gameData.m_planetTypeList[ planet.m_planetTypeId ];

					switch ( planetType.m_color )
					{
						case 0: color = new Color( 1.0f, 0.0f, 0.0f ); break;
						case 1: color = new Color( 0.4f, 0.2f, 0.0f ); break;
						case 2: color = new Color( 0.0f, 0.0f, 1.0f ); break;
						case 3: color = new Color( 1.0f, 1.0f, 1.0f ); break;
						case 4: color = new Color( 1.0f, 0.0f, 1.0f ); break;
						default: color = new Color( 0.0f, 1.0f, 0.0f ); break;
					}

					int orbitalPosition = m_spaceflightController.m_systemController.m_orbitNumberToPosition[ planet.m_orbitNumber - 1 ];

					m_planetImage[ orbitalPosition - 1 ].color = color;
				}
			}
		}
	}
}
