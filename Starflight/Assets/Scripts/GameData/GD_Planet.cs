
using UnityEngine;
using System;

[Serializable]

public class GD_Planet
{
	public const int c_mapWidth = 48;
	public const int c_mapHeight = 24;

	public int m_id;
	public int m_starId;
	public int m_planetTypeId;
	public int m_orbitPosition;
	public int m_mass;
	public int m_bioDensity;
	public int m_mineralDensity;
	public int m_atmosphereIdA;
	public int m_atmosphereIdB;
	public int m_atmosphereIdC;
	public int m_atmosphereDensityId;
	public int m_hydrosphereId;
	public bool m_hasEthanol;
	public int m_elementIdA;
	public int m_elementIdB;
	public int m_elementIdC;
	public int m_surfaceId;
	public int m_gravity;
	public int m_temperatureIdA;
	public int m_temperatureIdB;
	public int m_weatherId;

	public GD_Planet()
	{
		m_id = -1;
	}

	public void Initialize()
	{
		// make sure we don't go below mass of 6
		if ( m_mass < 6 )
		{
			m_mass = 6;
		}
	}

	// call this to get the surface of the planet
	public GD_Surface GetSurface()
	{
		var gameData = DataController.m_instance.m_gameData;

		return gameData.m_surfaceList[ m_surfaceId ];
	}

	// call this to get the orbit angle (in degrees) of the planet
	public float GetOrbitAngle()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// calculate number of days per year for each planet based on orbit number - orbit 3 should = 366 days like earth
		var daysPerYear = 122 * ( m_orbitPosition + 1 );

		// update the orbit angle
		return ( ( playerData.m_general.m_gameTime + 1000.0f + ( m_starId * 4 ) ) / daysPerYear ) * 360.0f;
	}

	// call this to get the position of the planet (in world coordinates)
	public Vector3 GetPosition()
	{
		// get the current orbit angle and convert to radians
		var orbitAngle = GetOrbitAngle() * ( Mathf.PI / 180.0f );

		// calculate the distance of the planet from the sun
		var distance = Mathf.Lerp( 50.0f, 225.0f, m_orbitPosition / 7.0f ) * ( 8192.0f / 256.0f );

		// calculate the position of the planet
		return new Vector3( -Mathf.Sin( orbitAngle ) * distance, 0.0f, Mathf.Cos( orbitAngle ) * distance );
	}

	// get the scale for this planet (in world units)
	public Vector3 GetScale()
	{
		// scale the planet based on its mass
		var scale = Mathf.Lerp( 32.0f, 256.0f, Mathf.Sqrt( ( m_mass - 6.0f ) / 500000.0f ) );

		// return the scale as a vector
		return new Vector3( scale, scale, scale );
	}

	// whether or not this planet is a gas giant
	public bool IsGasGiant()
	{
		return ( m_surfaceId == 1 );
	}

	// whether or not this planet is molten
	public bool IsMolten()
	{
		return ( m_surfaceId == 3 );
	}

	// whether or not this planet has an atmosphere
	public bool HasAtmosphere()
	{
		return !IsGasGiant() && ( m_atmosphereDensityId != 0 );
	}

	// return the atmosphere density as a float
	public float GetAtmosphereDensity()
	{
		return ( m_atmosphereDensityId - 1.0f ) / 4.0f;
	}

	// get the atmosphere text
	public string GetAtmosphereText()
	{
		var gameData = DataController.m_instance.m_gameData;

		var text = gameData.m_atmosphereList[ m_atmosphereIdA ].m_name;

		if ( m_atmosphereIdB != -1 )
		{
			text += ", " + gameData.m_atmosphereList[ m_atmosphereIdB ].m_name;
		}

		if ( m_atmosphereIdC != -1 )
		{
			text += ", " + gameData.m_atmosphereList[ m_atmosphereIdC ].m_name;
		}

		return text;
	}

	// get the hydrosphere text
	public string GetHydrosphereText()
	{
		var gameData = DataController.m_instance.m_gameData;

		return gameData.m_hydrosphereList[ m_hydrosphereId ].m_name;
	}

	// get the lithosphere text
	public string GetLithosphereText()
	{
		var gameData = DataController.m_instance.m_gameData;

		var text = gameData.m_elementList[ m_elementIdA ].m_name;
		text += ", " + gameData.m_elementList[ m_elementIdB ].m_name;
		text += ", " + gameData.m_elementList[ m_elementIdC ].m_name;

		return text;
	}

	// get the predominant surface text
	public string GetSurfaceText()
	{
		var gameData = DataController.m_instance.m_gameData;

		return gameData.m_surfaceList[ m_surfaceId ].m_name;
	}

	// get the gravity text
	public string GetGravityText()
	{
		return ( m_gravity / 100 ) + "." + ( m_gravity % 100 ) + " G";
	}

	// get the atmospheric density text
	public string GetAtmosphericDensityText()
	{
		var gameData = DataController.m_instance.m_gameData;

		return gameData.m_atmosphereDensityList[ m_atmosphereDensityId ].m_name;
	}

	// get the temperature text
	public string GetTemperatureText()
	{
		var gameData = DataController.m_instance.m_gameData;

		return gameData.m_temperatureList[ m_temperatureIdA ].m_name + " to " + gameData.m_temperatureList[ m_temperatureIdB ].m_name;
	}

	// get the global weather text
	public string GetGlobalWeatherText()
	{
		var gameData = DataController.m_instance.m_gameData;

		return gameData.m_weatherList[ m_weatherId ].m_name;
	}
}
