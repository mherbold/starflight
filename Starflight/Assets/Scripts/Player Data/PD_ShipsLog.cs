
using UnityEngine;

using System;
using System.Collections.Generic;

[Serializable] public class PD_ShipsLog
{
	enum AlienComm
	{
		Themselves,
		OtherRaces,
		GeneralInfo,
		OldEmpire,
		TheAncients,
		Count,
		First = Themselves,
		Last = TheAncients
	}

	[Serializable] public class Entry
	{
		public int m_id;
		public string m_stardate;
		public string m_message;

		public Entry( int id, string stardate, string message )
		{
			m_id = id;
			m_stardate = stardate;
			m_message = message;
		}
	}

	[SerializeField] List<Entry> m_starportNotices;
	[SerializeField] List<Entry> m_foundMessages;
	[SerializeField] List<Entry>[] m_alienComms;

	public void Reset()
	{
		// allocate memory
		m_starportNotices = new List<Entry>();
		m_foundMessages = new List<Entry>();

		m_alienComms = new List<Entry>[ (int) AlienComm.Count ];

		for ( var i = AlienComm.First; i < AlienComm.Last; i++ )
		{
			m_alienComms[ (int) i ] = new List<Entry>();
		}
	}

	public void AddStarportNotice( int noticeId )
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// go through the starport notices the player has already seen
		for ( var i = 0; i < m_starportNotices.Count; i++ )
		{
			// is this the same notice?
			if ( m_starportNotices[ i ].m_id == noticeId )
			{
				// yes - we've already added it to the ship log so don't worry about it
				return;
			}
		}

		// get the notice to add from the game data
		var noticeToAdd = gameData.m_noticeList[ noticeId ];

		// add this new notice to the ships log
		var entry = new Entry( noticeToAdd.m_id, noticeToAdd.m_stardate, noticeToAdd.m_message );

		m_starportNotices.Add( entry );
	}
}
