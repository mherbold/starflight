
using System;

[Serializable]

public class GD_CrewRace
{
	public int m_id;
	public string m_name;
	public GameData.Race m_race;

	public int m_scienceInitial;
	public int m_navigationInitial;
	public int m_engineeringInitial;
	public int m_communicationsInitial;
	public int m_medicineInitial;

	public int m_scienceMax;
	public int m_navigationMax;
	public int m_engineeringMax;
	public int m_communicationsMax;
	public int m_medicineMax;

	public string m_type;
	public string m_averageHeight;
	public string m_averageWeight;
	public int m_learningRate;
	public int m_durability;

	public int GetInitialSkill( int skillIndex )
	{
		switch ( skillIndex )
		{
			case 0: return m_scienceInitial;
			case 1: return m_navigationInitial;
			case 2: return m_engineeringInitial;
			case 3: return m_communicationsInitial;
			case 4: return m_medicineInitial;
		}

		throw new IndexOutOfRangeException();
	}

	public int GetMaximumSkill( int skillIndex )
	{
		switch ( skillIndex )
		{
			case 0: return m_scienceMax;
			case 1: return m_navigationMax;
			case 2: return m_engineeringMax;
			case 3: return m_communicationsMax;
			case 4: return m_medicineMax;
		}

		throw new IndexOutOfRangeException();
	}
}
