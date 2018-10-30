
using UnityEngine;
using System;

[Serializable]

public class GD_Territory : IComparable
{
	public int m_xCoordinate;
	public int m_yCoordinate;
	public int m_radius;
	public string m_name;

	Vector3 m_center;
	float m_size;
	float m_currentDistance;
	float m_penetrationDistance;

	public void Initialize()
	{
		m_center = Tools.GameToWorldCoordinates( new Vector3( m_xCoordinate, 0.0f, m_yCoordinate ) );

		m_size = m_radius * 256.0f;
	}

	public float GetCurrentDistance()
	{
		return m_currentDistance;
	}

	public float GetPenetrationDistance()
	{
		return m_penetrationDistance;
	}

	public int CompareTo( object obj )
	{
		if ( obj is GD_Territory )
		{
			int compareTo = m_currentDistance.CompareTo( ( obj as GD_Territory ).m_currentDistance );

			if ( compareTo == 0 )
			{
				return ( obj as GD_Territory ).m_penetrationDistance.CompareTo( m_penetrationDistance );
			}
			else
			{
				return compareTo;
			}
		}

		throw new ArgumentException( "Object is not a Territory" );
	}

	public void Update( Vector3 hyperspaceCoordinates )
	{
		m_currentDistance = Vector3.Distance( hyperspaceCoordinates, m_center );

		m_currentDistance -= m_size;

		m_penetrationDistance = Mathf.Max( 0.0f, -m_currentDistance );

		m_currentDistance = Mathf.Max( 0.0f, m_currentDistance );
	}
}
