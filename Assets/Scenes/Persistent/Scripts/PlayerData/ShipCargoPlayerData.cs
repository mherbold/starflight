
using System;

[Serializable]

public class ShipCargoPlayerData
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

		// reset the volume used up in the cargo hold
		RecalculateVolumeUsed();
	}

	public void RecalculateVolumeUsed()
	{
		// get total volume used up for artifacts and elements
		m_volumeUsed = m_artifactStorage.m_volumeUsed + m_elementStorage.m_volumeUsed;
	}
}
