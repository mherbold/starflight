
using UnityEngine;

using System;

[Serializable]

public class PD_Encounter
{
	public int m_encounterId;

	public Vector3 m_position;

	public void Reset( int encounterId )
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// remember the encounter id
		m_encounterId = encounterId;

		// get access to the encounter
		var encounter = gameData.m_encounterList[ encounterId ];

		// reset the position
		m_position = new Vector3( encounter.m_xCoordinate, 0.0f, encounter.m_yCoordinate );
	}
}
