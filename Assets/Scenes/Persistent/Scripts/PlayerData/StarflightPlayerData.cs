
using System;

[Serializable]

public class StarflightPlayerData
{
	public string m_currentStardate;

	public void Reset()
	{
		// reset the current stardate
		m_currentStardate = "4620-01-01";
	}
}
