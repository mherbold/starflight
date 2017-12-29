
using System;

[Serializable]

public class PlayerData
{
	public StarflightPlayerData m_starflightPlayerData;
	public NoticePlayerData m_noticePlayerData;
	public PersonnelPlayerData m_personnelPlayerData;
	public CrewAssignmentPlayerData m_crewAssignmentPlayerData;
	public BankPlayerData m_bankPlayerData;
	public ShipConfigurationPlayerData m_shipConfigurationPlayerData;

	// this resets our player progress to the new game state
	public void Reset()
	{
		m_starflightPlayerData.Reset();
		m_noticePlayerData.Reset();
		m_personnelPlayerData.Reset();
		m_crewAssignmentPlayerData.Reset();
		m_bankPlayerData.Reset();
		m_shipConfigurationPlayerData.Reset();
	}
}
