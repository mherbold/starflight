
using UnityEngine;

using System;

[Serializable]

public class PD_AlienShip
{
	// the vessel id
	public int m_vesselId;

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
		// vessel is random out of the given choices
		int numVessels = 0;

		if ( encounter.m_vesselIdA != 0 )
		{
			numVessels++;
		}

		if ( encounter.m_vesselIdB != 0 )
		{
			numVessels++;
		}

		if ( encounter.m_vesselIdC != 0 )
		{
			numVessels++;
		}

		var possibleVessels = new int[ numVessels ];

		var possibleVesselIndex = 0;

		if ( encounter.m_vesselIdA != 0 )
		{
			possibleVessels[ possibleVesselIndex++ ] = encounter.m_vesselIdA;
		}

		if ( encounter.m_vesselIdB != 0 )
		{
			possibleVessels[ possibleVesselIndex++ ] = encounter.m_vesselIdB;
		}

		if ( encounter.m_vesselIdC != 0 )
		{
			possibleVessels[ possibleVesselIndex++ ] = encounter.m_vesselIdC;
		}

		// pick a vessel
		var randomIndex = UnityEngine.Random.Range( 0, possibleVessels.Length );

		m_vesselId = possibleVessels[ randomIndex ];

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
