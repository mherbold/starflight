
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
	public Starflight m_starflight;
	public Starport m_starport;
	public Personnel m_personnel;
	public CrewAssignment m_crewAssignment;
	public Bank m_bank;
	public Ship m_ship;
	public KnownArtifacts m_knownArtifacts;

	// this resets our player progress to the new game state
	public void Reset()
	{
		m_version = c_currentVersion;
		m_isCurrentGame = false;

		m_starflight = new Starflight();
		m_starport = new Starport();
		m_personnel = new Personnel();
		m_crewAssignment = new CrewAssignment();
		m_bank = new Bank();
		m_ship = new Ship();
		m_knownArtifacts = new KnownArtifacts();

		m_starflight.Reset();
		m_starport.Reset();
		m_personnel.Reset();
		m_crewAssignment.Reset();
		m_bank.Reset();
		m_ship.Reset();
		m_knownArtifacts.Reset();
	}

	// returns true if the player data version is current
	public bool IsCurrentVersion()
	{
		return ( m_version == c_currentVersion );
	}
}
