
using UnityEngine;

using System;

[Serializable]

public class PD_Encounter : IComparable
{
	public int m_encounterId;

	public Vector3 m_coordinates;

	GD_Encounter m_encounter;

	PD_General.Location m_location;

	int m_starId;

	float m_currentDistance;

	public void Reset( int encounterId )
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// remember the encounter id
		m_encounterId = encounterId;

		// get access to the encounter
		m_encounter = gameData.m_encounterList[ encounterId ];

		// set the location (translated)
		switch ( m_encounter.m_location )
		{
			case 0: m_location = PD_General.Location.Hyperspace; break;
			case 1: m_location = PD_General.Location.StarSystem; break;
			case 2: m_location = PD_General.Location.InOrbit; break;
		}

		if ( m_location != PD_General.Location.Hyperspace )
		{
			foreach ( var star in gameData.m_starList )
			{
				if ( star.m_xCoordinate == m_encounter.m_xCoordinate )
				{
					if ( star.m_yCoordinate == m_encounter.m_yCoordinate )
					{
						m_starId = star.m_id;
						break;
					}
				}
			}
		}

		// reset the position
		if ( m_location == PD_General.Location.Hyperspace )
		{
			m_coordinates = Tools.GameToWorldCoordinates( new Vector3( m_encounter.m_xCoordinate, 0.0f, m_encounter.m_yCoordinate ) );
		}
		else if ( m_location == PD_General.Location.StarSystem )
		{
			var randomPosition = UnityEngine.Random.insideUnitCircle * ( 8192.0f - 512.0f );

			m_coordinates = new Vector3( randomPosition.x, 0.0f, randomPosition.y );
		}
		else
		{
			m_coordinates = Vector3.zero;
		}
	}

	public PD_General.Location GetLocation()
	{
		return m_location;
	}

	public int GetStarId()
	{
		return m_starId;
	}

	public int CompareTo( object obj )
	{
		if ( obj is PD_Encounter )
		{
			int compareTo = m_currentDistance.CompareTo( ( obj as PD_Encounter ).m_currentDistance );

			if ( compareTo == 0 )
			{
				return ( obj as PD_Encounter ).m_currentDistance.CompareTo( m_currentDistance );
			}
			else
			{
				return compareTo;
			}
		}

		throw new ArgumentException( "Object is not a PD_Encounter" );
	}

	public void Update( PD_General.Location location, int starId, Vector3 coordinates )
	{
		// update distance from the player to the encounter
		if ( ( location != m_location ) || ( m_location == PD_General.Location.InOrbit ) || ( ( m_location == PD_General.Location.StarSystem ) && ( starId != m_starId ) ) )
		{
			m_currentDistance = float.MaxValue;
		}
		else
		{
			m_currentDistance = Vector3.Distance( coordinates, m_coordinates );
		}
	}

	public float GetDistance()
	{
		return m_currentDistance;
	}
}
