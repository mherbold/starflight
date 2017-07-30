
using System;

[Serializable]

public class ShipConfigurationPlayerData
{
	public string m_name;
	public int m_numCargoPods;
	public int m_enginesClass;
	public int m_sheildingClass;
	public int m_armorClass;
	public int m_missileLauncherClass;
	public int m_laserCannonClass;

	public ShipConfigurationPlayerData()
	{
		// reset the ship name
		m_name = "";

		// reset the number of cargo pods
		m_numCargoPods = 0;

		// reset the levels of each ship component
		m_enginesClass = 1;
		m_sheildingClass = 0;
		m_armorClass = 0;
		m_missileLauncherClass = 0;
		m_laserCannonClass = 0;
	}

	public string GetClassString( int classNumber )
	{
		if ( classNumber == 0 )
		{
			return "None";
		}
		else
		{
			return "Class " + classNumber;
		}
	}

	public string GetEnginesClassString()
	{
		return GetClassString( m_enginesClass );
	}

	public string GetSheildingClassString()
	{
		return GetClassString( m_sheildingClass );
	}

	public string GetArmorClassString()
	{
		return GetClassString( m_armorClass );
	}

	public string GetMissileLauncherClassString()
	{
		return GetClassString( m_missileLauncherClass );
	}

	public string GetLaserCannonClassString()
	{
		return GetClassString( m_laserCannonClass );
	}
}
