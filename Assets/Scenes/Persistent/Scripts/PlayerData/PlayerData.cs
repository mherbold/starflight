
using System;

[Serializable]

public class PlayerData
{
	const int c_currentVersion = 4;

	public int m_version;

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
