
using System;

[Serializable]

public class NoticePlayerData
{
	public string m_lastReadStardate;

	public void Reset()
	{
		// reset the last read notice stardate
		m_lastReadStardate = "0000-00-00";
	}
}
