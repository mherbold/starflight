
using UnityEngine;

using System;
using System.Collections.Generic;

[Serializable] public class PD_ShipsLog
{
	public enum AlienComm
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
		public string m_header;
		public string m_message;

		public Entry( int id, string stardate, string header, string message )
		{
			m_id = id;
			m_stardate = stardate;
			m_header = header;
			m_message = message;
		}
	}

	[SerializeField] public List<Entry> m_starportNotices;
	[SerializeField] public List<Entry> m_foundMessages;
	[SerializeField] public List<Entry>[] m_alienComms;

	public void Reset()
	{
		// allocate memory
		m_starportNotices = new List<Entry>();
		m_foundMessages = new List<Entry>();

		m_alienComms = new List<Entry>[ (int) AlienComm.Count ];

		for ( var i = AlienComm.First; i <= AlienComm.Last; i++ )
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

		// convert the stardate into human readable format
		DateTime messageDate = DateTime.ParseExact( noticeToAdd.m_stardate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );
		var header = messageDate.ToLongDateString();
		var stardate = messageDate.ToShortDateString();

		// add this new notice to the ships log
		var entry = new Entry( noticeId, stardate, header, noticeToAdd.m_message );

		m_starportNotices.Add( entry );
	}

	public void AddAlienComm( GD_Comm comm, string message )
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// which subject?
		var alienComms = m_alienComms[ (int) comm.m_subject - (int) GD_Comm.Subject.Themselves ];

		// go through the alien comms the player has already seen
		for ( var i = 0; i < alienComms.Count; i++ )
		{
			// is this the same alien comm?
			if ( alienComms[ i ].m_id == comm.m_id )
			{
				// yes - is the message the same? (could be different due to garbling and comm officer skill level)
				if ( alienComms[ i ].m_message == message )
				{
					// yes - we've already added it to the ship log so don't worry about it
					return;
				}
			}
		}

		// get the stardate (and hour)
		var stardate = playerData.m_general.m_currentStardateDHMY;

		// build the header
		var header = "Alien Species #" + (int) comm.m_race;

		// add this new notice to the ships log
		var entry = new Entry( comm.m_id, stardate, header, message );

		alienComms.Add( entry );
	}
}
