
using UnityEngine;
using System;

[Serializable]

public class GD_Nebula : IComparable
{
	public int m_xCoordinate;
	public int m_yCoordinate;
	public int m_radius;

	Vector3 m_center;
	float m_size;

	float m_currentDistance;
	// float m_penetrationDistance;

	public void Initialize()
	{
		m_center = Tools.GameToWorldCoordinates( new Vector3( m_xCoordinate, 0.0f, m_yCoordinate ) );

		m_size = m_radius * 256.0f;
	}

	public int CompareTo( object obj )
	{
		if ( obj is GD_Nebula )
		{
			return m_currentDistance.CompareTo( ( obj as GD_Nebula ).m_currentDistance );
		}

		throw new ArgumentException( "Object is not a Nebula" );
	}

	public void Update( Vector3 hyperspaceCoordinates )
	{
		m_currentDistance = Vector3.Distance( hyperspaceCoordinates, m_center );

		m_currentDistance -= m_size;

		// m_penetrationDistance = Mathf.Max( 0.0f, -m_currentDistance );

		m_currentDistance = Mathf.Max( 0.0f, m_currentDistance );
	}
}
