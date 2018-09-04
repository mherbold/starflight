
using UnityEngine;
using System;

[Serializable]

public class Territory : IComparable
{
	public int m_xCoordinate;
	public int m_yCoordinate;
	public int m_radius;
	public string m_name;

	public SerializableVector3 m_center;
	public float m_size;
	public float m_currentDistance;
	public float m_penetrationDistance;

	public void Initialize()
	{
		m_center = Tools.GameToWorldCoordinates( new Vector3( m_xCoordinate, 0.0f, m_yCoordinate ) );

		m_size = m_radius * 128.0f; // should be 256.0f - need to fix bug in data
	}

	public int CompareTo( object obj )
	{
		if ( obj is Territory )
		{
			return m_currentDistance.CompareTo( ( obj as Territory ).m_currentDistance );
		}

		throw new ArgumentException( "Object is not a Territory" );
	}
}
