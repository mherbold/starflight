
using System;

[Serializable]

public class ArtifactReference
{
	public int m_artifactId;

	// constructor
	public ArtifactReference( int artifactId )
	{
		m_artifactId = artifactId;
	}

	public ArtifactGameData GetArtifactGameData()
	{
		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// return the artifact game data
		return gameData.m_artifactList[ m_artifactId ];
	}

	// get the number of cubic meters this artifact takes up
	public int GetVolume()
	{
		return GetArtifactGameData().m_volume;
	}
}
