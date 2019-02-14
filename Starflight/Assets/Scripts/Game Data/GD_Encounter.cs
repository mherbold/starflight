using System;

[Serializable]

public class GD_Encounter
{
	public int m_id;

	public GameData.Race m_race;

	public int m_xCoordinate;
	public int m_yCoordinate;

	public int m_location;

	public int m_orbitPosition;

	public int m_maxNumShips;
	public int m_maxNumShipsAtOnce;

	public int m_vesselIdA;
	public int m_vesselIdB;
	public int m_vesselIdC;

	public bool m_unknown;
}
