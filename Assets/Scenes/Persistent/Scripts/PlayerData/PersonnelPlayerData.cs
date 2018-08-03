
using System;
using System.Collections.Generic;

[Serializable]

public class PersonnelPlayerData
{
	[Serializable]

	public class Personnel
	{
		public int m_fileId;
		public int m_raceIndex;

		public string m_name;

		public int m_science;
		public int m_navigation;
		public int m_engineering;
		public int m_communications;
		public int m_medicine;

		public float m_vitality;

		public Personnel( PersonnelPlayerData personnelPlayerData )
		{
			// set the id of this personnel file
			m_fileId = personnelPlayerData.m_nextFileId;

			// increment the id number
			personnelPlayerData.m_nextFileId++;
		}

		// this gets the current skill points for the selected skill
		public int GetSkill( int skillIndex )
		{
			switch ( skillIndex )
			{
				case 0: return m_science;
				case 1: return m_navigation;
				case 2: return m_engineering;
				case 3: return m_communications;
				case 4: return m_medicine;
			}

			throw new IndexOutOfRangeException();
		}

		// this sets the skill points for the selected skill
		public void SetSkill( int skillIndex, int newValue )
		{
			switch ( skillIndex )
			{
				case 0: m_science = newValue; break;
				case 1: m_navigation = newValue; break;
				case 2: m_engineering = newValue; break;
				case 3: m_communications = newValue; break;
				case 4: m_medicine = newValue; break;
				default:
				{
					throw new IndexOutOfRangeException();
				}
			}
		}
	}

	public int m_nextFileId;
	public List<Personnel> m_personnelList;

	// reset the personnel player data
	public void Reset()
	{
		// reset the file id numbering
		m_nextFileId = 0;

		// reset the personnel list
		m_personnelList = new List<PersonnelPlayerData.Personnel>();
	}

	// call this to create and add a new personnel to the list
	public Personnel CreateNewPersonnel()
	{
		return new Personnel( this );
	}

	// this gets the personnel id from the file id
	public int GetPersonnelId( int fileId )
	{
		for ( int personnelId = 0; personnelId < m_personnelList.Count; personnelId++ )
		{
			if ( m_personnelList[ personnelId ].m_fileId == fileId )
			{
				return personnelId;
			}
		}

		throw new ArgumentException();
	}

	// this gets the personnel file using the file id
	public Personnel GetPersonnel( int fileId )
	{
		int personnelId = GetPersonnelId( fileId );

		return m_personnelList[ personnelId ];
	}

	// this returns true if there are any living crewmember in personnel, and false if not
	public bool AnyLiving()
	{
		for ( int personnelId = 0; personnelId < m_personnelList.Count; personnelId++ )
		{
			if ( m_personnelList[ personnelId ].m_vitality > 0 )
			{
				return true;
			}
		}

		return false;
	}
}
