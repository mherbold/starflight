
using System;
using System.Collections.Generic;

[Serializable]

public class PD_ArtifactStorage
{
	public int m_volumeUsed;
	public List<PD_ArtifactReference> m_artifactList;

	public void Reset()
	{
		// reset the volume used
		m_volumeUsed = 0;

		// create a new artifact list
		m_artifactList = new List<PD_ArtifactReference>();
	}

	public void Add( int artifactId )
	{
		// create the reference to the artifact game data
		var artifactReference = new PD_ArtifactReference( artifactId );

		// add the artifact to the storage
		m_artifactList.Add( artifactReference );

		// update the volume used
		m_volumeUsed += artifactReference.GetVolume();
	}

	public void Remove( int artifactId )
	{
		// find the reference to the artifact game data
		var artifactReference = Find( artifactId );

		// remove it
		m_artifactList.Remove( artifactReference );

		// update the volume used
		m_volumeUsed -= artifactReference.GetVolume();
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
}
