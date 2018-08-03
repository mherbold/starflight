
using System;

[Serializable]

public class ShipConfigurationPlayerData
{
	public string m_name;
	public int m_numCargoPods;
	public int m_enginesClass;
	public int m_shieldingClass;
	public int m_armorClass;
	public int m_missileLauncherClass;
	public int m_laserCannonClass;
	public int m_mass;
	public int m_volume;
	public int m_acceleration;

	public void Reset()
	{
		// reset the ship name
		m_name = "";

		// reset the number of cargo pods
		m_numCargoPods = 0;

		// reset the levels of each ship component
		m_enginesClass = 1;
		m_shieldingClass = 0;
		m_armorClass = 0;
		m_missileLauncherClass = 0;
		m_laserCannonClass = 0;

		// recalculate the mass of the ship
		RecalculateMass();

		// recalculate the volume of the ship
		RecalculateVolume();

		// recalculate the acceleration of the ship
		RecalculateAcceleration();
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
		return GetClassString( m_shieldingClass );
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

	public void RecalculateMass()
	{
		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// start with the base ship mass
		m_mass = gameData.m_shipGameData.m_baseShipMass;

		// add in the mass of all the add on ship components
		m_mass += gameData.m_shipGameData.m_cargoPodMass * m_numCargoPods;
		m_mass += gameData.m_enginesList[ m_enginesClass ].m_mass;
		m_mass += gameData.m_shieldingList[ m_shieldingClass ].m_mass;
		m_mass += gameData.m_armorList[ m_armorClass ].m_mass;
		m_mass += gameData.m_missileLauncherList[ m_missileLauncherClass ].m_mass;
		m_mass += gameData.m_laserCannonList[ m_laserCannonClass ].m_mass;
	}

	public void RecalculateVolume()
	{
		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// start with the base ship mass
		m_volume = gameData.m_shipGameData.m_baseShipVolume;

		// add in the volume of each cargo pod
		m_volume += gameData.m_shipGameData.m_cargoPodVolume * m_numCargoPods;
	}

	public void RecalculateAcceleration()
	{
		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// get access to our engines
		EnginesGameData engines = gameData.m_enginesList[ m_enginesClass ];

		// this formula closely matches the original starflight game
		m_acceleration = engines.m_baseAcceleration - Convert.ToInt32( Math.Sqrt( m_mass - engines.m_accelerationMass ) * engines.m_accelerationScale );
	}

	public void AddCargoPod()
	{
		// add one cargo pod
		m_numCargoPods++;

		// recalculate ship metrics
		RecalculateMass();
		RecalculateVolume();
		RecalculateAcceleration();
	}

	public void RemoveCargoPod()
	{
		// remove one cargo pod
		m_numCargoPods--;

		// recalculate ship metrics
		RecalculateMass();
		RecalculateVolume();
		RecalculateAcceleration();
	}

	public int GetRemainingVolme()
	{
		// get access to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// calculate and return the amount of space remaining in the cargo hold
		return m_volume - playerData.m_shipCargoPlayerData.m_volumeUsed;
	}
}
