
using UnityEngine;
using UnityEngine.UI;

public class SystemDisplay : ShipDisplay
{
	// the orbit rings
	public GameObject[] m_orbit;

	// the planets
	public SVGImage[] m_planet;

	// arth starport
	public GameObject m_arth;

	// the star
	public SVGImage m_star;

	// the ship
	public GameObject m_player;

	// unity update
	public override void Update()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the star data
		GD_Star star = gameData.m_starList[ playerData.m_general.m_currentStarId ];

		// update the positions of the planets
		var planetList = star.GetPlanetList();

		foreach ( var planet in planetList )
		{
			if ( ( planet != null ) && ( planet.m_id != -1 ) )
			{
				// get the orbit angle
				float orbitAngle = planet.GetOrbitAngle();

				// calculate the game object rotation
				Quaternion rotation = Quaternion.AngleAxis( orbitAngle, Vector3.forward );

				// set the new rotation on the orbit game object
				m_orbit[ planet.m_orbitPosition ].transform.rotation = rotation;
			}
		}

		// get the position of the ship and convert it to map coordinates
		Vector3 position = m_spaceflightController.m_player.GetPosition() * 256.0f / 8192.0f;

		// set the new position of the ship game object
		m_player.transform.localPosition = new Vector3( position.x, position.z );
	}

	// the system map display label
	public override string GetLabel()
	{
		return "System Map";
	}

	// call this to change the system currently being displayed
	public void ChangeSystem()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the star data
		GD_Star star = gameData.m_starList[ playerData.m_general.m_currentStarId ];

		// turn off the arth game object
		m_arth.SetActive( false );

		// enable the 4th planet game object (in case it was previously disabled)
		m_planet[ 3 ].gameObject.SetActive( true );

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

		m_star.color = color;

		// update each planet in the system
		for ( int i = 0; i < GD_Star.c_maxNumPlanets; i++ )
		{
			GD_Planet planet = m_spaceflightController.m_starSystem.m_planetController[ i ].m_planet;

			// is there a planet?
			if ( ( planet == null ) || ( planet.m_id == -1 ) )
			{
				// no - disable the orbit game object
				m_orbit[ i ].SetActive( false );
			}
			else
			{
				// yes - enable the orbit game object
				m_orbit[ i ].SetActive( true );

				// check if this is the arth station (special case)
				if ( planet.m_id == gameData.m_misc.m_arthPlanetId )
				{
					// yep - hide the planet object 
					m_planet[ planet.m_orbitPosition ].gameObject.SetActive( false );

					// and show the arth station instead
					m_arth.SetActive( true );
				}
				else
				{
					// show the planet object (it may have been hidden by the arth orbit)
					m_planet[ planet.m_orbitPosition ].gameObject.SetActive( true );

					// change the display color for this planet
					GD_Surface surface = planet.GetSurface();
					color = new Color( surface.m_colorR / 255.0f, surface.m_colorG / 255.0f, surface.m_colorB / 255.0f );
					m_planet[ planet.m_orbitPosition ].color = color;
					// Debug.Log( "Setting display color for planet " + planet.m_orbitPosition + " to R:" + surface.m_colorR + " G:" + surface.m_colorG + " B:" + surface.m_colorB );
				}
			}
		}
	}
}
