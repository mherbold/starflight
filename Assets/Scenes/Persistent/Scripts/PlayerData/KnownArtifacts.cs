
using System;
using System.Collections.Generic;

[Serializable]

public class KnownArtifacts
{
	public List<ArtifactReference> m_artifactList;

	public void Reset()
	{
		// create a new artifact list
		m_artifactList = new List<ArtifactReference>();

		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// add all artifacts that should be already known at the start of the game
		for ( int artifactId = 0; artifactId < gameData.m_artifactList.Length; artifactId++ )
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
		ArtifactReference artifactReference = new ArtifactReference( artifactId );

		// add the artifact to the storage
		m_artifactList.Add( artifactReference );
	}

	public void Remove( int artifactId )
	{
		// find the reference to the artifact game data
		ArtifactReference artifactReference = Find( artifactId );

		// remove it
		m_artifactList.Remove( artifactReference );
	}

	public ArtifactReference Find( int artifactId )
	{
		// search for a known artifact by the artifact id
		foreach ( ArtifactReference artifactReference in m_artifactList )
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
