
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

	// is this ship in the encounter already?
	public bool m_addedToEncounter;

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

		// the ship is not dead
		m_isDead = false;
	}

	public void SetCoordinate( Vector3 coordinate )
	{
		m_coordinate = coordinate;
	}

	public void SetDirection( Vector3 direction )
	{
		m_direction = direction;
	}
}
