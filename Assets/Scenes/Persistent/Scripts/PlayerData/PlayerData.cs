
using System;

[Serializable]

public class PlayerData
{
	const int c_currentVersion = 2;

	public int m_playerDataVersion;

	public StarflightPlayerData m_starflightPlayerData;
	public NoticePlayerData m_noticePlayerData;
	public PersonnelPlayerData m_personnelPlayerData;
	public CrewAssignmentPlayerData m_crewAssignmentPlayerData;
	public BankPlayerData m_bankPlayerData;
	public ShipConfigurationPlayerData m_shipConfigurationPlayerData;
	public ShipCargoPlayerData m_shipCargoPlayerData;
	public StarportCargoPlayerData m_starportCargoPlayerData;

	// this resets our player progress to the new game state
	public void Reset()
	{
		m_playerDataVersion = c_currentVersion;

		m_starflightPlayerData = new StarflightPlayerData();
		m_noticePlayerData = new NoticePlayerData();
		m_personnelPlayerData = new PersonnelPlayerData();
		m_crewAssignmentPlayerData = new CrewAssignmentPlayerData();
		m_bankPlayerData = new BankPlayerData();
		m_shipConfigurationPlayerData = new ShipConfigurationPlayerData();
		m_shipCargoPlayerData = new ShipCargoPlayerData();
		m_starportCargoPlayerData = new StarportCargoPlayerData();

		m_starflightPlayerData.Reset();
		m_noticePlayerData.Reset();
		m_personnelPlayerData.Reset();
		m_crewAssignmentPlayerData.Reset();
		m_bankPlayerData.Reset();
		m_shipConfigurationPlayerData.Reset();
		m_shipCargoPlayerData.Reset();
		m_starportCargoPlayerData.Reset();
	}

	// returns true if the player data version is current
	public bool IsCurrentVersion()
	{
		return ( m_playerDataVersion == c_currentVersion );
	}
}
