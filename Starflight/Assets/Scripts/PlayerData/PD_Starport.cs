
using System;

[Serializable]

public class PD_Starport
{
	public string m_lastReadNoticeStardate;
	public PD_ArtifactStorage m_artifactStorage;

	public void Reset()
	{
		// reset the last read notice stardate
		m_lastReadNoticeStardate = "0000-00-00";

		// create and reset the starport artifact storage to initial game state
		m_artifactStorage = new PD_ArtifactStorage();

		m_artifactStorage.Reset();

		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// add the artifacts that are available to buy in starport at the beginning of the game
		for ( var artifactId = 0; artifactId < gameData.m_artifactList.Length; artifactId++ )
		{
			var artifactGameData = gameData.m_artifactList[ artifactId ];

			if ( artifactGameData.m_availableInStarport )
			{
				m_artifactStorage.Add( artifactId );
			}
		}
	}
}
