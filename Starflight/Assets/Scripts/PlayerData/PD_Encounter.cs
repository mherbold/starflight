
using UnityEngine;

using System;

[Serializable]

public class PD_Encounter
{
	public int m_encounterId;

	public Vector3 m_coordinates;

	GD_Encounter m_encounter;

	PD_General.Location m_location;

	int m_starId;

	public void Reset( int encounterId )
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// remember the encounter id
		m_encounterId = encounterId;

		// get access to the encounter
		var encounter = gameData.m_encounterList[ encounterId ];

		// reset the position
		m_coordinates = new Vector3( encounter.m_xCoordinate, 0.0f, encounter.m_yCoordinate );
	}

	public void Initialize()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get access to the encounter
		m_encounter = gameData.m_encounterList[ m_encounterId ];

		// return the location (translated)
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
	}

	public PD_General.Location GetLocation()
	{
		return m_location;
	}

	public int GetStarId()
	{
		return m_starId;
	}
}
