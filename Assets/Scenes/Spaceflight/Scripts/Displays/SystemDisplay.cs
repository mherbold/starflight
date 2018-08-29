
using UnityEngine;
using UnityEngine.UI;

public class SystemDisplay : ShipDisplay
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
		m_orbitGameObject = new GameObject[ Star.c_maxNumPlanets ];
		m_planetImage = new Image[ Star.c_maxNumPlanets ];

		// get to the orbits
		for ( int i = 0; i < Star.c_maxNumPlanets; i++ )
		{
			// get the orbit game object
			transform = m_rootGameObject.transform.Find( "Orbit-" + ( i + 1 ) );
			m_orbitGameObject[ i ] = transform.gameObject;

			// get the planet image
			transform = m_orbitGameObject[ i ].transform.Find( "Planet" );
			m_planetImage[ i ] = transform.GetComponent<Image>();
		}

		// get to the arth game object
		transform = m_orbitGameObject[ 2 ].transform.Find( "Arth" );
		m_arthGameObject = transform.gameObject;

		// get to the sun image
		transform = m_rootGameObject.transform.Find( "Star" );
		m_sunImage = transform.GetComponent<Image>();

		// get to the ship game object
		transform = m_rootGameObject.transform.Find( "Ship" );
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
		for ( int i = 0; i < Star.c_maxNumPlanets; i++ )
		{
			float angle = m_spaceflightController.m_systemController.m_planetController[ i ].m_orbitAngle * 360.0f;

			Quaternion rotation = Quaternion.AngleAxis( angle, Vector3.forward );

			m_orbitGameObject[ i ].transform.rotation = rotation;
		}

		// update the position of the ship
		Vector3 position = m_spaceflightController.m_camera.transform.position * 0.01f;
		m_shipGameObject.transform.localPosition = new Vector3( position.x, position.z );
	}

	public void ChangeSystem( int starId )
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the star data
		Star star = gameData.m_starList[ starId ];

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

		// update each planet in the system
		for ( int i = 0; i < Star.c_maxNumPlanets; i++ )
		{
			Planet planet = m_spaceflightController.m_systemController.m_planetController[ i ].m_planet;

			if ( planet == null )
			{
				m_orbitGameObject[ i ].SetActive( false );
			}
			else
			{
				m_orbitGameObject[ i ].SetActive( true );

				// check if this is the arth station (special case)
				if ( planet.m_planetTypeId == 57 )
				{
					// yep - hide the planet object and show the arth station instead
					m_planetImage[ planet.m_orbitPosition ].gameObject.SetActive( false );
					m_arthGameObject.SetActive( true );
				}
				else
				{
					// change the display color for this planet
					Surface surface = planet.GetSurface();
					color = new Color( surface.m_colorR / 255.0f, surface.m_colorG / 255.0f, surface.m_colorB / 255.0f );
					m_planetImage[ planet.m_orbitPosition ].color = color;
					// Debug.Log( "Setting display color for planet " + planet.m_orbitPosition + " to R:" + surface.m_colorR + " G:" + surface.m_colorG + " B:" + surface.m_colorB );
				}
			}
		}
	}
}
