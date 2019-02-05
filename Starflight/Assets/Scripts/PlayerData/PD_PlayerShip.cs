
using UnityEngine;
using System;

[Serializable]

public class PD_PlayerShip
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
	public int m_volumeUsed;
	public int m_acceleration;

	public float m_fuelUsed;

	public PD_ArtifactStorage m_artifactStorage;
	public PD_ElementStorage m_elementStorage;

	public bool m_shieldsAreUp;
	public bool m_weaponsAreArmed;

	public int m_shieldPoints;
	public int m_armorPoints;

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

		// reset fuel used to zero
		m_fuelUsed = 0.0f;

		// reset other stuff
		m_shieldsAreUp = false;
		m_weaponsAreArmed = false;

		m_shieldPoints = 0;
		m_armorPoints = 250;

		// recalculate the mass of the ship
		RecalculateMass();

		// recalculate the volume of the ship
		RecalculateVolume();

		// recalculate the acceleration of the ship
		RecalculateAcceleration();

		// create and reset the cargo hold to initial game state
		m_artifactStorage = new PD_ArtifactStorage();
		m_elementStorage = new PD_ElementStorage();

		m_artifactStorage.Reset();
		m_elementStorage.Reset();

		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// fill the ship cargo with all of the elements the player gets at the start of the game
		for ( var elementId = 0; elementId < gameData.m_elementList.Length; elementId++ )
		{
			var elementGameData = gameData.m_elementList[ elementId ];

			if ( elementGameData.m_initialVolume > 0 )
			{
				m_elementStorage.Add( elementId, elementGameData.m_initialVolume );
			}
		}

		// recalculate the used up space in the cargo hold
		RecalculateVolumeUsed();
	}

	public GD_Engines GetEngines()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// return the engines this ship has
		return gameData.m_enginesList[ m_enginesClass ];
	}

	public GD_Sheilding GetSheilding()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// return the engines this ship has
		return gameData.m_shieldingList[ m_shieldingClass ];
	}

	public GD_Armor GetArmor()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// return the engines this ship has
		return gameData.m_armorList[ m_armorClass ];
	}

	public GD_MissileLauncher GetMissileLauncher()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// return the engines this ship has
		return gameData.m_missileLauncherList[ m_missileLauncherClass ];
	}

	public GD_LaserCannon GetLaserCannon()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// return the engines this ship has
		return gameData.m_laserCannonList[ m_laserCannonClass ];
	}

	public void RecalculateMass()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// start with the base ship mass
		m_mass = gameData.m_misc.m_baseShipMass;

		// add in the mass of all the cargo pods
		m_mass += m_numCargoPods * gameData.m_misc.m_cargoPodMass;

		// add in the mass of all the add on ship components
		m_mass += GetEngines().m_mass;
		m_mass += GetSheilding().m_mass;
		m_mass += GetArmor().m_mass;
		m_mass += GetMissileLauncher().m_mass;
		m_mass += GetLaserCannon().m_mass;
	}

	public void RecalculateVolume()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// start with the base ship mass
		m_volume = gameData.m_misc.m_baseShipVolume;

		// add in the volume of each cargo pod
		m_volume += gameData.m_misc.m_cargoPodVolume * m_numCargoPods;
	}

	public void RecalculateAcceleration()
	{
		// get our engines
		var engines = GetEngines();

		// this formula closely matches the original starflight game
		m_acceleration = Mathf.RoundToInt( Mathf.Pow( ( 500.0f - m_mass ) / 500.0f, engines.m_powerCurve ) * engines.m_powerScale * engines.m_maximumAcceleration + engines.m_minimumAcceleration );
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
		var playerData = DataController.m_instance.m_playerData;

		// calculate and return the amount of space remaining in the cargo hold
		return m_volume - playerData.m_playerShip.m_volumeUsed;
	}

	public void AddElement( int elementId, int volume )
	{
		// add the element to storage
		m_elementStorage.Add( elementId, volume );

		// recalculate the used up space in the cargo hold
		RecalculateVolumeUsed();
	}

	public void RemoveElement( int elementId, int volume )
	{
		// remove the element from storage
		m_elementStorage.Remove( elementId, volume );

		// recalculate the used up space in the cargo hold
		RecalculateVolumeUsed();
	}

	public void AddArtifact( int artifactId )
	{
		// add the artifact to storage
		m_artifactStorage.Add( artifactId );

		// recalculate the used up space in the cargo hold
		RecalculateVolumeUsed();
	}

	public void RemoveArtifact( int artifactId )
	{
		// remove the artifact from storage
		m_artifactStorage.Remove( artifactId );

		// recalculate the used up space in the cargo hold
		RecalculateVolumeUsed();
	}

	public void RecalculateVolumeUsed()
	{
		// get total volume used up for artifacts and elements
		m_volumeUsed = m_artifactStorage.m_volumeUsed + m_elementStorage.m_volumeUsed;
	}
}
