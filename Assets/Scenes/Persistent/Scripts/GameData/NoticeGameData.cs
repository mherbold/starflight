
using System;

[Serializable]

public class NoticeGameData : GameData
{
	[Serializable]

	public class Notice
	{
		public string m_stardate;
		public string m_message;
	}

	public Notice[] m_noticeList;
}
