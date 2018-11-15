
using UnityEngine;

using System;

[Serializable]

public class PD_AlienShip
{
	// the encounter type id
	public int m_encounterTypeId;

	// the current position of the alien
	public Vector3 m_coordinates;

	// current target coordinates
	public Vector3 m_targetCoordinates;

	// the current direction of the alien
	public Vector3 m_currentDirection;

	// keep track of the last direction (for banking the ship)
	public Vector3 m_lastDirection;

	// keep track of the last banking angle (for interpolation)
	public float m_currentBankingAngle;

	// number of seconds since the last time we updated the target coordinates
	public float m_timeSinceLastTargetCoordinateChange;

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

		// reset some important stuff
		m_coordinates = Vector3.zero;
		m_targetCoordinates = Vector3.zero;
		m_currentDirection = Vector3.forward;
		m_lastDirection = Vector3.forward;
		m_currentBankingAngle = 0.0f;
		m_timeSinceLastTargetCoordinateChange = 0.0f;
		m_isDead = false;
		m_addedToEncounter = false;
	}
}
