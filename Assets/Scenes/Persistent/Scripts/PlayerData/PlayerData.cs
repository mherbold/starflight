
using System;
using System.Collections.Generic;

[Serializable]

public class PlayerData
{
	public StarflightPlayerData m_starflightPlayerData;
	public NoticePlayerData m_noticePlayerData;
	public PersonnelPlayerData m_personnelPlayerData;
	public CrewAssignmentPlayerData m_crewAssignmentPlayerData;
	public BankPlayerData m_bankPlayerData;

	// this resets our player progress to the new game state
	public void Reset()
	{
		// allocate all of the player data parts
		m_starflightPlayerData = new StarflightPlayerData();
		m_noticePlayerData = new NoticePlayerData();
		m_personnelPlayerData = new PersonnelPlayerData();
		m_crewAssignmentPlayerData = new CrewAssignmentPlayerData();
		m_bankPlayerData = new BankPlayerData();
	}
}
