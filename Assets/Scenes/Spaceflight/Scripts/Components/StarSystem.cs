
using UnityEngine;

public class StarSystem : MonoBehaviour
{
	// our planet controllers
	public PlanetController[] m_planetController;

	// the nebula overlay
	public GameObject m_nebula;

	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// the shine script (so we can change the color)
	public Shine m_shine;

	// the planet controller for the planet we could orbit around
	PlanetController m_orbitPlanetController;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the star data
		Star star = gameData.m_starList[ playerData.m_starflight.m_currentStarId ];

		// calculate the time to next flare (in FP days)
		float timeToFlare = star.m_daysToNextFlare - playerData.m_starflight.m_gameTime;

		// did we flare already?
		if ( timeToFlare <= 0.0f )
		{
			// yes - the sun is stable again
			m_shine.SetSize( 128.0f, 129.0f );
		}
		else if ( timeToFlare <= 1.0f ) // are we flaring NOW?
		{
			// TODO: fuck up the player
		}
		else
		{
			float size = 1.0f / timeToFlare;

			float minSize = 128.0f + size * 64.0f;
			float maxSize = 129.0f + size * 128.0f;

			m_shine.SetSize( minSize, maxSize );

			//Debug.Log( "The star will flare in " + timeToFlare + " days - minSize = " + minSize + ", maxSize = " + maxSize );
		}

		// yes - did we just leave it?
		if ( Vector3.Magnitude( playerData.m_starflight.m_systemCoordinates ) >= 8192.0f )
		{
			Debug.Log( "Player leaving the star system - switching to the hyperspace location." );

			// yes - calculate the position of the ship in hyperspace
			Vector3 newPosition = playerData.m_starflight.m_systemCoordinates;

			newPosition.Normalize();

			newPosition *= star.GetBreachDistance() + 16.0f;
			newPosition += star.m_worldCoordinates;

			// update the player position
			m_spaceflightController.m_player.transform.position = newPosition;

			// save the player's position in the player data
			playerData.m_starflight.m_hyperspaceCoordinates = newPosition;

			// limit the ship speed to hyperspace speed
			playerData.m_starflight.m_currentSpeed = Mathf.Min( playerData.m_starflight.m_currentSpeed, m_spaceflightController.m_maximumShipSpeedHyperspace );

			// switch modes now
			m_spaceflightController.SwitchLocation( Starflight.Location.Hyperspace );
		}
		else
		{
			// get the nearest planet controller to the player
			m_orbitPlanetController = GetNearestPlanetController();

			// did we get a planet controller?
			if ( m_orbitPlanetController != null )
			{
				// yes - get the distance of the player is to the planet
				float distanceToPlanet = m_orbitPlanetController.GetDistanceToPlayer();

				// are we close enough to orbit the planet?
				if ( distanceToPlanet <= m_orbitPlanetController.m_scale )
				{
					// yes - let the player know
					m_spaceflightController.m_spaceflightUI.m_messages.text = "Ship is within orbital range.";
				}
				else
				{
					// no - forget this planet
					m_orbitPlanetController = null;

					// display the spectral class and ecosphere
					m_spaceflightController.m_spaceflightUI.m_messages.text = "<u>Stellar Parameters</u>\nSpectral Class: <#4FEDED>" + star.m_class + "</color>\nEcosphere: <#4FEDED>" + star.m_spectralClass.m_ecosphereMin + " - " + star.m_spectralClass.m_ecosphereMax + "</color>";
				}
			}
		}
	}

	// call this to hide the starsystem objects
	public void Hide()
	{
		Debug.Log( "Hiding the star system scene." );

		// hide the starsystem
		gameObject.SetActive( false );
	}

	// call this to show the starsystem objects
	public void Show()
	{
		// if we are already active then don't do it again
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the star system scene." );

		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the star data
		Star star = gameData.m_starList[ playerData.m_starflight.m_currentStarId ];

		// show the starsystem
		gameObject.SetActive( true );

		// make sure the camera is at the right height above the zero plane
		m_spaceflightController.m_player.DollyCamera( 1024.0f );

		// move the player object
		m_spaceflightController.m_player.transform.position = playerData.m_starflight.m_systemCoordinates;

		// calculate the new rotation of the player
		Quaternion newRotation = Quaternion.LookRotation( playerData.m_starflight.m_currentDirection, Vector3.up );

		// update the player rotation
		m_spaceflightController.m_player.m_ship.rotation = newRotation;

		// unfreeze the player
		m_spaceflightController.m_player.Unfreeze();

		// configure the infinite starfield system to become fully visible at higher speeds
		m_spaceflightController.m_player.SetStarfieldFullyVisibleSpeed( 15.0f );

		// fade in the map
		m_spaceflightController.m_spaceflightUI.FadeMap( 1.0f, 2.0f );

		// show / hide the nebula depending on if we are in one
		m_nebula.SetActive( star.m_insideNebula );

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.StarSystem );

		// change the color of the sun
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

		m_shine.SetColor( color );

		// turn off all the planets
		for ( int i = 0; i < Star.c_maxNumPlanets; i++ )
		{
			m_planetController[ i ].DisablePlanet();
		}

		// turn on planets in this system
		foreach ( Planet planet in star.m_planetList )
		{
			if ( ( planet != null ) && ( planet.m_id != -1 ) )
			{
				m_planetController[ planet.m_orbitPosition ].EnablePlanet( planet );
			}
		}

		// update the system display
		m_spaceflightController.m_displayController.m_systemDisplay.ChangeSystem();
	}

	// call this to get th nearest planet controller to the player
	public PlanetController GetNearestPlanetController()
	{
		float nearestDistanceToPlayer = 0.0f;

		PlanetController nearestPlanetController = null;

		foreach ( PlanetController planetController in m_planetController )
		{
			if ( planetController.m_planet != null )
			{
				float distanceToPlayer = planetController.GetDistanceToPlayer();

				if ( ( nearestPlanetController == null ) || ( distanceToPlayer < nearestDistanceToPlayer ) )
				{
					nearestDistanceToPlayer = distanceToPlayer;
					nearestPlanetController = planetController;
				}
			}
		}

		return nearestPlanetController;
	}

	// call this to get the planet we could be orbiting around
	public PlanetController GetOrbitPlanetController()
	{
		return m_orbitPlanetController;
	}
}
