
using UnityEngine;

using System;

[Serializable]

public class PD_AlienShip
{
	// the encounter type id
	public int m_encounterTypeId;

	// the current position of the alien
	public Vector3 m_coordinate;

	// the current direction of the alien
	public Vector3 m_direction;

	// is this ship dead?
	public bool m_isDead;

	// this is called when the player data is created
	public void Initialize( GD_Encounter encounter )
	{
		// encounter type is random out of the given choices
		int numEncounterTypes = 0;

		if ( encounter.m_encounterTypeIdA != 0 )
		{
			numEncounterTypes++;
		}

		if ( encounter.m_encounterTypeIdB != 0 )
		{
			numEncounterTypes++;
		}

		if ( encounter.m_encounterTypeIdC != 0 )
		{
			numEncounterTypes++;
		}

		var possibleEncounterTypes = new int[ numEncounterTypes ];

		var possibleEncounterTypeIndex = 0;

		if ( encounter.m_encounterTypeIdA != 0 )
		{
			possibleEncounterTypes[ possibleEncounterTypeIndex++ ] = encounter.m_encounterTypeIdA;
		}

		if ( encounter.m_encounterTypeIdB != 0 )
		{
			possibleEncounterTypes[ possibleEncounterTypeIndex++ ] = encounter.m_encounterTypeIdB;
		}

		if ( encounter.m_encounterTypeIdC != 0 )
		{
			possibleEncounterTypes[ possibleEncounterTypeIndex++ ] = encounter.m_encounterTypeIdC;
		}

		var randomIndex = UnityEngine.Random.Range( 0, possibleEncounterTypes.Length - 1 );

		m_encounterTypeId = possibleEncounterTypes[ randomIndex ];

		// put alien ship in a random position on a circle around the player
		var randomPosition = UnityEngine.Random.insideUnitCircle;

		m_coordinate = new Vector3( randomPosition.x, 0.0f, randomPosition.y );

		m_coordinate = Vector3.Normalize( m_coordinate ) * 2048.0f;

		// make alien ship face the center of the encounter space
		m_direction = -Vector3.Normalize( m_coordinate );

		// the ship is not dead
		m_isDead = false;
	}
}
