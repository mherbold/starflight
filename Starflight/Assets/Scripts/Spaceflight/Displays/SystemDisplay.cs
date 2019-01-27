
using UnityEngine;
using UnityEngine.UI;

public class SystemDisplay : ShipDisplay
{
	// the orbit rings
	public GameObject[] m_orbitList;

	// the planets
	public Image[] m_planetList;

	// the star
	public Image m_star;

	// the ship
	public Image m_player;

	// remember whether or not we have been initialized
	bool m_alreadyInitialized;

	// unity start
	public override void Start()
	{
		Initialize();
	}

	// unity update
	public override void Update()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the star data
		var star = gameData.m_starList[ playerData.m_general.m_currentStarId ];

		// update the positions of the planets
		var planetList = star.GetPlanetList();

		foreach ( var planet in planetList )
		{
			if ( ( planet != null ) && ( planet.m_id != -1 ) )
			{
				// get the orbit angle
				float orbitAngle = planet.GetOrbitAngle();

				// calculate the game object rotation
				Quaternion rotation = Quaternion.Euler( 0.0f, 0.0f, orbitAngle );

				// set the new rotation on the orbit game object
				m_orbitList[ planet.m_orbitPosition - 1 ].transform.rotation = rotation;
			}
		}

		// get the position of the ship and convert it to map coordinates
		var position = SpaceflightController.m_instance.m_playerShip.GetPosition() * 256.0f / 8192.0f;

		// set the new position of the ship game object
		m_player.transform.localPosition = new Vector3( position.x, position.z, -30.0f );

		// hide the player dot if we are in orbit
		m_player.enabled = playerData.m_general.m_location != PD_General.Location.InOrbit;
	}

	// the system map display label
	public override string GetLabel()
	{
		return "System Map";
	}

	public void Initialize()
	{
		// have we been initialized yet?
		if ( !m_alreadyInitialized )
		{
			// no - clone materials so they don't get saved out when running in the editor
			m_star.material = new Material( m_star.material );

			foreach ( var planet in m_planetList )
			{
				planet.material = new Material( planet.material );
			}

			// remember that we have been initialized
			m_alreadyInitialized = true;
		}
	}

	// call this to change the system currently being displayed
	public void ChangeSystem()
	{
		// make sure we have been initialized
		Initialize();

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the star data
		var star = gameData.m_starList[ playerData.m_general.m_currentStarId ];

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

		m_star.material.SetColor( "SF_AlbedoColor", color );

		// update each planet in the system
		for ( var i = 0; i < GD_Star.c_maxNumPlanets; i++ )
		{
			var planet = SpaceflightController.m_instance.m_starSystem.m_planetController[ i ].m_planet;

			// is there a planet?
			if ( ( planet == null ) || ( planet.m_id == -1 ) )
			{
				// no - disable the orbit game object
				m_orbitList[ i ].SetActive( false );
			}
			else
			{
				// yes - enable the orbit game object
				m_orbitList[ i ].SetActive( true );

				// get the planet surface
				var surface = planet.GetSurface();

				// calculate the color of the planet from the surface color
				color = new Color( surface.m_colorR / 255.0f, surface.m_colorG / 255.0f, surface.m_colorB / 255.0f );

				// update the material with the new color
				m_planetList[ planet.m_orbitPosition - 1 ].material.SetColor( "SF_AlbedoColor", color );
			}
		}
	}
}
