
using UnityEngine;
using System;

[Serializable]

public class Planet
{
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

	public Planet()
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
	public Surface GetSurface()
	{
		GameData gameData = DataController.m_instance.m_gameData;

		return gameData.m_surfaceList[ m_surfaceId ];
	}

	// call this to get the orbit angle (in degrees) of the planet
	public float GetOrbitAngle()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// calculate number of days per year for each planet based on orbit number - orbit 3 should = 366 days like earth
		int daysPerYear = 122 * ( m_orbitPosition + 1 );

		// update the orbit angle
		return ( ( playerData.m_starflight.m_gameTime + 1000.0f + ( m_starId * 4 ) ) / daysPerYear ) * 360.0f;
	}

	// call this to get the position of the planet (in world coordinates)
	public Vector3 GetPosition()
	{
		// get the current orbit angle and convert to radians
		float orbitAngle = GetOrbitAngle() * ( Mathf.PI / 180.0f );

		// calculate the distance of the planet from the sun
		float distance = Mathf.Lerp( 50.0f, 225.0f, m_orbitPosition / 7.0f ) * ( 8192.0f / 256.0f );

		// calculate the position of the planet
		return new Vector3( -Mathf.Sin( orbitAngle ) * distance, 0.0f, Mathf.Cos( orbitAngle ) * distance );
	}

	// get the scale for this planet (in world units)
	public Vector3 GetScale()
	{
		// scale the planet based on its mass
		float scale = Mathf.Lerp( 32.0f, 320.0f, Mathf.Sqrt( ( m_mass - 6.0f ) / 500000.0f ) );

		// return the scale as a vector
		return new Vector3( scale, scale, scale );
	}
}
