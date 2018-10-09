
using System;

[Serializable]

public class CrewAssignment
{
	public enum Role
	{
		First = 0,
		Captain = 0,
		ScienceOfficer,
		Navigator,
		Engineer,
		CommunicationsOfficer,
		Doctor,
		Length
	};

	public int m_captainFileId;
	public int m_scienceOfficerFileId;
	public int m_navigatorFileId;
	public int m_engineerFileId;
	public int m_communicationsOfficerFileId;
	public int m_doctorFileId;

	public void Reset()
	{
		// unassign all crew member roles
		m_captainFileId = -1;
		m_scienceOfficerFileId = -1;
		m_navigatorFileId = -1;
		m_engineerFileId = -1;
		m_communicationsOfficerFileId = -1;
		m_doctorFileId = -1;
	}

	public int GetFileId( Role role )
	{
		switch ( role )
		{
			case Role.Captain: return m_captainFileId;
			case Role.ScienceOfficer: return m_scienceOfficerFileId;
			case Role.Navigator: return m_navigatorFileId;
			case Role.Engineer: return m_engineerFileId;
			case Role.CommunicationsOfficer: return m_communicationsOfficerFileId;
			case Role.Doctor: return m_doctorFileId;
		}

		return -1;
	}

	public bool IsAssigned( Role role )
	{
		int fileId = GetFileId( role );

		return ( fileId == -1 ) ? false : true;
	}

	public void Assign( Role role, int fileId )
	{
		switch ( role )
		{
			case Role.Captain: m_captainFileId = fileId; break;
			case Role.ScienceOfficer: m_scienceOfficerFileId = fileId; break;
			case Role.Navigator: m_navigatorFileId = fileId; break;
			case Role.Engineer: m_engineerFileId = fileId; break;
			case Role.CommunicationsOfficer: m_communicationsOfficerFileId = fileId; break;
			case Role.Doctor: m_doctorFileId = fileId; break;
		}
	}

	public Personnel.PersonnelFile GetPersonnelFile( Role role )
	{
		int fileId = GetFileId( role );

		PlayerData playerData = DataController.m_instance.m_playerData;

		return playerData.m_personnel.GetPersonnelFile( fileId );
	}
}
