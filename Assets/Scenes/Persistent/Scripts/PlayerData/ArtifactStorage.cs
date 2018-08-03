
using System;
using System.Collections.Generic;

[Serializable]

public class ArtifactStorage
{
	public int m_volumeUsed;
	public List<ArtifactReference> m_artifactList;

	public void Reset()
	{
		// reset the volume used
		m_volumeUsed = 0;

		// create a new artifact list
		m_artifactList = new List<ArtifactReference>();
	}

	public void Add( int artifactId )
	{
		// create the reference to the artifact game data
		ArtifactReference artifactReference = new ArtifactReference( artifactId );

		// add the artifact to the storage
		m_artifactList.Add( artifactReference );

		// update the volume used
		m_volumeUsed += artifactReference.GetVolume();
	}

	public ArtifactReference Find( int artifactId )
	{
		foreach ( ArtifactReference artifactReference in m_artifactList )
		{
			if ( artifactReference.m_artifactId == artifactId )
			{
				return artifactReference;
			}
		}

		return null;
	}

	public void Remove( int artifactId )
	{
		ArtifactReference artifactReference = Find( artifactId );

		m_artifactList.Remove( artifactReference );
	}
}
