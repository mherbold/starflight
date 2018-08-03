
using System;

[Serializable]

public class Starflight
{
	public string m_currentStardate;

	public void Reset()
	{
		// reset the current stardate
		m_currentStardate = "4620-01-01";
	}
}
