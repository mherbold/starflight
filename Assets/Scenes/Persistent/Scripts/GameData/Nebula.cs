
using UnityEngine;
using System;

[Serializable]

public class Nebula : IComparable
{
	public int m_xCoordinate;
	public int m_yCoordinate;
	public int m_radius;

	public SerializableVector3 m_center;
	public float m_size;
	public float m_currentDistance;
	public float m_penetrationDistance;

	public void Initialize()
	{
		m_center = Tools.GameToWorldCoordinates( new Vector3( m_xCoordinate, 0.0f, m_yCoordinate ) );

		m_size = m_radius * 256.0f;
	}

	public int CompareTo( object obj )
	{
		if ( obj is Nebula )
		{
			return m_currentDistance.CompareTo( ( obj as Nebula ).m_currentDistance );
		}

		throw new ArgumentException( "Object is not a Nebula" );
	}
}
