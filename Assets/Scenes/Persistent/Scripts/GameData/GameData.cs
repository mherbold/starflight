
using System;

[Serializable]

public class GameData : GameDataFile
{
	public NoticeGameData[] m_noticeList;
	public RaceGameData[] m_raceList;
	public ShipGameData m_shipGameData;
	public EnginesGameData[] m_enginesList;
	public SheildingGameData[] m_shieldingList;
	public ArmorGameData[] m_armorList;
	public MissileLauncherGameData[] m_missileLauncherList;
	public LaserCannonGameData[] m_laserCannonList;
}
