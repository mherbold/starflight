
using System;

[Serializable]

public class CrewAssignment
{
	public int m_captainFileId;
	public int m_scienceOfficerFileId;
	public int m_navigatorFileId;
	public int m_engineerFileId;
	public int m_communicationsFileId;
	public int m_doctorFileId;

	public void Reset()
	{
		// unassign all crew member positions
		m_captainFileId = -1;
		m_scienceOfficerFileId = -1;
		m_navigatorFileId = -1;
		m_engineerFileId = -1;
		m_communicationsFileId = -1;
		m_doctorFileId = -1;
	}

	public int GetFileId( int positionIndex )
	{
		switch ( positionIndex )
		{
			case 0: return m_captainFileId;
			case 1: return m_scienceOfficerFileId;
			case 2: return m_navigatorFileId;
			case 3: return m_engineerFileId;
			case 4: return m_communicationsFileId;
			case 5: return m_doctorFileId;
		}

		throw new IndexOutOfRangeException();
	}

	public bool IsAssigned( int positionIndex )
	{
		int fileId = -1;

		switch ( positionIndex )
		{
			case 0: fileId = m_captainFileId; break;
			case 1: fileId = m_scienceOfficerFileId; break;
			case 2: fileId = m_navigatorFileId; break;
			case 3: fileId = m_engineerFileId; break;
			case 4: fileId = m_communicationsFileId; break;
			case 5: fileId = m_doctorFileId; break;
		}

		return ( fileId == -1 ) ? false : true;
	}

	public void Assign( int positionIndex, int fileId )
	{
		switch ( positionIndex )
		{
			case 0: m_captainFileId = fileId; break;
			case 1: m_scienceOfficerFileId = fileId; break;
			case 2: m_navigatorFileId = fileId; break;
			case 3: m_engineerFileId = fileId; break;
			case 4: m_communicationsFileId = fileId; break;
			case 5: m_doctorFileId = fileId; break;

			default:
			{
				throw new IndexOutOfRangeException();
			}
		}
	}
}
