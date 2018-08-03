
using System;

[Serializable]

public class PlayerData
{
	const int c_currentVersion = 2;

	public int m_version;

	public Starflight m_starflight;
	public Notice m_notice;
	public Personnel m_personnel;
	public CrewAssignment m_crewAssignment;
	public Bank m_bank;
	public ShipConfiguration m_shipConfiguration;
	public ShipCargo m_shipCargo;
	public StarportCargo m_starportCargo;
	public KnownArtifacts m_knownArtifacts;

	// this resets our player progress to the new game state
	public void Reset()
	{
		m_version = c_currentVersion;

		m_starflight = new Starflight();
		m_notice = new Notice();
		m_personnel = new Personnel();
		m_crewAssignment = new CrewAssignment();
		m_bank = new Bank();
		m_shipConfiguration = new ShipConfiguration();
		m_shipCargo = new ShipCargo();
		m_starportCargo = new StarportCargo();
		m_knownArtifacts = new KnownArtifacts();

		m_starflight.Reset();
		m_notice.Reset();
		m_personnel.Reset();
		m_crewAssignment.Reset();
		m_bank.Reset();
		m_shipConfiguration.Reset();
		m_shipCargo.Reset();
		m_starportCargo.Reset();
		m_knownArtifacts.Reset();
	}

	// returns true if the player data version is current
	public bool IsCurrentVersion()
	{
		return ( m_version == c_currentVersion );
	}
}
