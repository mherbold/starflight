
using System;

[Serializable]

public class StarportCargo
{
	public ArtifactStorage m_artifactStorage;

	public void Reset()
	{
		// create and reset the starport artifact storage to initial game state
		m_artifactStorage = new ArtifactStorage();

		m_artifactStorage.Reset();

		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// add the artifacts that are available to buy in starport at the beginning of the game
		for ( int artifactId = 0; artifactId < gameData.m_artifactList.Length; artifactId++ )
		{
			ArtifactGameData artifactGameData = gameData.m_artifactList[ artifactId ];

			if ( artifactGameData.m_availableInStarport )
			{
				m_artifactStorage.Add( artifactId );
			}
		}
	}
}
