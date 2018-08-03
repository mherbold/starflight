
using System;

[Serializable]

public class ShipCargo
{
	public int m_volumeUsed;
	public ArtifactStorage m_artifactStorage;
	public ElementStorage m_elementStorage;

	public void Reset()
	{
		// create and reset the cargo hold to initial game state
		m_artifactStorage = new ArtifactStorage();
		m_elementStorage = new ElementStorage();

		m_artifactStorage.Reset();
		m_elementStorage.Reset();

		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// fill the ship cargo with all of the elements the player gets at the start of the game
		for ( int elementId = 0; elementId < gameData.m_elementList.Length; elementId++ )
		{
			ElementGameData elementGameData = gameData.m_elementList[ elementId ];

			if ( elementGameData.m_initialVolume > 0 )
			{
				m_elementStorage.Add( elementId, elementGameData.m_initialVolume );
			}
		}

		// recalculate the used up space in the cargo hold
		RecalculateVolumeUsed();
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
