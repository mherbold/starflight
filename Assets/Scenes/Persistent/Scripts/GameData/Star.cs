
using UnityEngine;
using System;

[Serializable]

public class Star
{
	public const int c_maxNumPlanets = 8;

	public int m_id;
	public int m_xCoordinate;
	public int m_yCoordinate;
	public string m_class;
	public int m_daysToNextFlare;
	public int m_daysSincePreviousFlare;
	public int m_yearOfNextFlare;
	public int m_monthOfNextFlare;
	public int m_dayOfNextFlare;

	public Planet[] m_planetList;
	public float m_scale;
	public SerializableVector3 m_gameCoordinates;
	public SerializableVector3 m_worldCoordinates;

	public void Initialize( GameData gameData )
	{
		// allocate planets array
		m_planetList = new Planet[ 8 ];

		// initialize planets array with null (means no planet in that orbit)
		for ( int i = 0; i < c_maxNumPlanets; i++ )
		{
			m_planetList[ i ] = null;
		}

		// find planets and put them in the right spot in the planets array
		foreach ( Planet planet in gameData.m_planetList )
		{
			if ( planet.m_starId == m_id )
			{
				m_planetList[ planet.m_orbitPosition ] = planet;
			}
		}

		// calculate the scale of this star based on the system class
		m_scale = 0.0f;

		switch ( m_class )
		{
			case "M": m_scale = 0.0f; break;
			case "K": m_scale = 0.1f; break;
			case "G": m_scale = 0.2f; break;
			case "F": m_scale = 0.3f; break;
			case "A": m_scale = 0.4f; break;
			case "B": m_scale = 0.5f; break;
			case "O": m_scale = 0.6f; break;
		}

		// generate coordinate vectors
		m_gameCoordinates = new Vector3( m_xCoordinate, 0.0f, m_yCoordinate );
		m_worldCoordinates = Tools.GameToWorldCoordinates( m_gameCoordinates );
	}

	// call this to get the breach distance
	public float GetBreachDistance()
	{
		return ( 1.0f + m_scale / 0.6f ) * 48.0f;
	}
}
