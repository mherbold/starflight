
using UnityEngine;
using System;

[Serializable]

public class PD_TerrainVehicle
{
	public int m_volumeUsed;

	public float m_fuelRemaining;

	public PD_ArtifactStorage m_artifactStorage;
	public PD_ElementStorage m_elementStorage;

	public void Reset()
	{
		// refuel the terrain vehicle
		Refuel();

		// create and reset the cargo hold to initial game state
		m_artifactStorage = new PD_ArtifactStorage();
		m_elementStorage = new PD_ElementStorage();

		m_artifactStorage.Reset();
		m_elementStorage.Reset();

		// recalculate the used up space in the cargo hold
		RecalculateVolumeUsed();
	}

	public void Refuel()
	{
		// reset fuel remaining to maximum
		m_fuelRemaining = 1.0f;
	}

	public int GetRemainingVolume()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// calculate and return the amount of space remaining in the cargo hold
		return gameData.m_misc.m_terrainVehicleVolume - m_volumeUsed;
	}

	public int GetPercentRemainingVolume()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get the remaining volume in cubic meters
		var remainingVolume = GetRemainingVolume();

		// return it as a percentage
		return Mathf.FloorToInt( (float) remainingVolume * 100.0f / (float) gameData.m_misc.m_terrainVehicleVolume );
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

	public void UseUpFuel( float amount )
	{
		// add to the running amount of fuel remaining
		m_fuelRemaining -= amount;
	}

	public int GetPercentFuelRemaining()
	{
		return Mathf.CeilToInt( m_fuelRemaining * 100.0f );
	}
}
