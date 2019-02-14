
using System;
using System.Collections.Generic;

[Serializable]

public class PD_KnownArtifacts
{
	public List<PD_ArtifactReference> m_artifactList;

	public void Reset()
	{
		// create a new artifact list
		m_artifactList = new List<PD_ArtifactReference>();

		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// add all artifacts that should be already known at the start of the game
		for ( var artifactId = 0; artifactId < gameData.m_artifactList.Length; artifactId++ )
		{
			if ( gameData.m_artifactList[ artifactId ].m_knownAtStart )
			{
				Add( artifactId );
			}
		}
	}

	public void Add( int artifactId )
	{
		// create the reference to the artifact game data
		var artifactReference = new PD_ArtifactReference( artifactId );

		// add the artifact to the storage
		m_artifactList.Add( artifactReference );
	}

	public void Remove( int artifactId )
	{
		// find the reference to the artifact game data
		var artifactReference = Find( artifactId );

		// remove it
		m_artifactList.Remove( artifactReference );
	}

	public PD_ArtifactReference Find( int artifactId )
	{
		// search for a known artifact by the artifact id
		foreach ( var artifactReference in m_artifactList )
		{
			if ( artifactReference.m_artifactId == artifactId )
			{
				return artifactReference;
			}
		}

		// could not find it
		return null;
	}

	public bool IsKnown( int artifactId )
	{
		// return true if we know what this artifact is
		return Find( artifactId ) != null;
	}
}
