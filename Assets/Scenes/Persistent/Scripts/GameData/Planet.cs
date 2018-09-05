
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
}
