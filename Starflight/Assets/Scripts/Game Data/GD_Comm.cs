using System;

[Serializable]

public class GD_Comm
{
	public enum Subject
	{
		None = 0,
		GreetingHail = 1,
		GreetingResponse = 2,
		Yes = 3,
		No = 4,
		NoMoreInformation = 5,
		WaitingForResponse = 6,
		Themselves = 7,
		OtherRaces = 8,
		GeneralInfo = 9,
		OldEmpire = 10,
		TheAncients = 11,
		Statement = 13,
		Question = 14,
		Terminate = 15,
		Custom = 16
	}

	public enum Stance
	{
		None = 0,
		Neutral = 1,
		Friendly = 2,
		Hostile = 3,
		Obsequious = 4
	};

	public int m_id;
	public GameData.Race m_race;
	public Subject m_subject;
	public Stance m_stance;
	public bool m_homeworld;
	public bool m_surrender;
	public string m_text;

	public GD_Comm( string text )
	{
		m_text = text;
	}
}
