
using System;

[Serializable]

public class GameData
{
	public enum Race
	{
		None = 0,
		Elowan = 1,
		Gazurtoid = 2,
		Mechan = 3,
		Mysterion = 4,
		NomadProbe = 5,
		Spemin = 6,
		Thrynn = 7,
		Velox = 8,
		Uhlek = 9,
		VeloxiDrone = 10,
		Minstrel = 11,
		Human = 16,
		TheEnterprise = 18,
		NoahDerelict = 19,
		Debris = 20,
		InterstelPolice = 21,
		Android = 255,
	}

	public GD_Misc m_misc;
	public GD_Notice[] m_noticeList;
	public GD_CrewRace[] m_crewRaceList;
	public GD_Engines[] m_enginesList;
	public GD_Sheilding[] m_shieldingList;
	public GD_Armor[] m_armorList;
	public GD_MissileLauncher[] m_missileLauncherList;
	public GD_LaserCannon[] m_laserCannonList;
	public GD_Artifact[] m_artifactList;
	public GD_Element[] m_elementList;
	public GD_Star[] m_starList;
	public GD_Planet[] m_planetList;
	public GD_PlanetType[] m_planetTypeList;
	public GD_Atmosphere[] m_atmosphereList;
	public GD_AtmosphereDensity[] m_atmosphereDensityList;
	public GD_Hydrosphere[] m_hydrosphereList;
	public GD_Surface[] m_surfaceList;
	public GD_Temperature[] m_temperatureList;
	public GD_Weather[] m_weatherList;
	public GD_Flux[] m_fluxList;
	public GD_Territory[] m_territoryList;
	public GD_Nebula[] m_nebulaList;
	public GD_SpectralClass[] m_spectralClassList;
	public GD_Encounter[] m_encounterList;
	public GD_Vessel[] m_vesselList;
	public GD_Comm[] m_commList;

	public void Initialize()
	{
		// go through each planet
		foreach ( GD_Planet planet in m_planetList )
		{
			// initialize it
			planet.Initialize();
		}

		// go through each star
		foreach ( GD_Star star in m_starList )
		{
			// initialize it
			star.Initialize( this );
		}

		// go through each flux
		foreach ( GD_Flux flux in m_fluxList )
		{
			// initialize it
			flux.Initialize();
		}

		// go through each territory
		foreach ( GD_Territory territory in m_territoryList )
		{
			// initialize it
			territory.Initialize();
		}

		// go through each nebula
		foreach ( GD_Nebula nebula in m_nebulaList )
		{
			// initialize it
			nebula.Initialize();
		}
	}

	// this finds the element in the list by its name
	public int FindElementId( string name )
	{
		for ( int elementId = 0; elementId < m_elementList.Length; elementId++ )
		{
			if ( m_elementList[ elementId ].m_name == name )
			{
				return elementId;
			}
		}

		return -1;
	}
}
