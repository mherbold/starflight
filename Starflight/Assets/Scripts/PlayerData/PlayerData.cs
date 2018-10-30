
using System;

[Serializable]

public class PlayerData
{
	// the current save game version - increment this to invalidate obsolete save game files
	const int c_currentVersion = 6;

	// the current version of this player data file
	public int m_version;

	// is this the current game?
	public bool m_isCurrentGame;
	
	// all the parts of the player data
	public PD_General m_starflight;
	public PD_Starport m_starport;
	public PD_Personnel m_personnel;
	public PD_CrewAssignment m_crewAssignment;
	public PD_Bank m_bank;
	public PD_PlayerShip m_ship;
	public PD_KnownArtifacts m_knownArtifacts;
	public PD_Encounter[] m_encounterList;

	// this resets our player progress to the new game state
	public void Reset()
	{
		var gameData = DataController.m_instance.m_gameData;

		m_version = c_currentVersion;
		m_isCurrentGame = false;

		m_starflight = new PD_General();
		m_starport = new PD_Starport();
		m_personnel = new PD_Personnel();
		m_crewAssignment = new PD_CrewAssignment();
		m_bank = new PD_Bank();
		m_ship = new PD_PlayerShip();
		m_knownArtifacts = new PD_KnownArtifacts();
		m_encounterList = new PD_Encounter[ gameData.m_encounterList.Length ];

		m_starflight.Reset();
		m_starport.Reset();
		m_personnel.Reset();
		m_crewAssignment.Reset();
		m_bank.Reset();
		m_ship.Reset();
		m_knownArtifacts.Reset();

		for ( var i = 0; i < gameData.m_encounterList.Length; i++ )
		{
			m_encounterList[ i ] = new PD_Encounter();

			m_encounterList[ i ].Reset( i );
		}
	}

	// returns true if the player data version is current
	public bool IsCurrentVersion()
	{
		return ( m_version == c_currentVersion );
	}
}
