
using System;

[Serializable]

public class GameData : GameDataFile
{
	public Notice[] m_noticeList;
	public Race[] m_raceList;
	public ShipBasics m_shipBasics;
	public Engines[] m_enginesList;
	public Sheilding[] m_sheildingList;
	public Armor[] m_armorList;
	public MissileLauncher[] m_missileLauncherList;
	public LaserCannon[] m_laserCannonList;
}
