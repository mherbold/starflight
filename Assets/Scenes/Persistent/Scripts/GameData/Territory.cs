
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

		m_size = m_radius * 256.0f;
	}

	public int CompareTo( object obj )
	{
		if ( obj is Territory )
		{
			int compareTo = m_currentDistance.CompareTo( ( obj as Territory ).m_currentDistance );

			if ( compareTo == 0 )
			{
				return ( obj as Territory ).m_penetrationDistance.CompareTo( m_penetrationDistance );
			}
			else
			{
				return compareTo;
			}
		}

		throw new ArgumentException( "Object is not a Territory" );
	}
}
