
using System;

[Serializable]

public class GameData : GameDataFile
{
	public Misc m_misc;
	public Starport m_starport;
	public Notice[] m_noticeList;
	public Race[] m_raceList;
	public Ship m_ship;
	public Engines[] m_enginesList;
	public Sheilding[] m_shieldingList;
	public Armor[] m_armorList;
	public MissileLauncher[] m_missileLauncherList;
	public LaserCannon[] m_laserCannonList;
	public Artifact[] m_artifactList;
	public Element[] m_elementList;
	public Star[] m_starList;
	public Planet[] m_planetList;
	public PlanetType[] m_planetTypeList;

	public void Initialize()
	{
		// go through each star
		foreach ( Star star in m_starList )
		{
			// initialize it
			star.Initialize( this );
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
