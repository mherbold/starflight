
using UnityEngine;
using UnityEngine.UI;

public class SystemDisplay : ShipDisplay
{
	// the orbit rings
	public GameObject[] m_orbitGameObject;

	// the planets
	public Image [] m_planetImage;

	// arth starport
	public GameObject m_arthGameObject;

	// the star
	public Image m_starImage;

	// the ship
	public GameObject m_shipGameObject;

	// unity update
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
		Vector3 position = m_spaceflightController.m_player.GetPosition() * 256.0f / 8192.0f;
		m_shipGameObject.transform.localPosition = new Vector3( position.x, position.z );
	}

	// the system map display label
	public override string GetLabel()
	{
		return "System Map";
	}

	// call this to change the system currently being displayed
	public void ChangeSystem()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the star data
		Star star = gameData.m_starList[ playerData.m_starflight.m_currentStarId ];

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

		m_starImage.color = color;

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
					// show the planet object (it may have been hidden by the arth orbit)
					m_planetImage[ planet.m_orbitPosition ].gameObject.SetActive( true );

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
