
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

	public Artifact GetArtifactGameData()
	{
		// get access to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// return the artifact game data
		return gameData.m_artifactList[ m_artifactId ];
	}

	public int GetVolume()
	{
		// get the number of cubic meters this artifact takes up
		return GetArtifactGameData().m_volume;
	}
}
