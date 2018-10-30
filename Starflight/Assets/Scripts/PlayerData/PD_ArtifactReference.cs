
using System;

[Serializable]

public class PD_ArtifactReference
{
	public int m_artifactId;

	// constructor
	public PD_ArtifactReference( int artifactId )
	{
		m_artifactId = artifactId;
	}

	public GD_Artifact GetArtifactGameData()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// return the artifact game data
		return gameData.m_artifactList[ m_artifactId ];
	}

	public int GetVolume()
	{
		// get the number of cubic meters this artifact takes up
		return GetArtifactGameData().m_volume;
	}
}
