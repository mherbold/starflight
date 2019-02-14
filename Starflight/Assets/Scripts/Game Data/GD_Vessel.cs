using System;

[Serializable]

public class GD_Vessel
{
	public int m_id;

	public string m_name;
	public string m_object;
	public string m_type;
	public GameData.Race m_race;

	public int m_mineralDensity;
	public int m_bioDensity;
	public int m_mass;

	public int m_moveDelay;
	public int m_fireDelay;

	public int m_armor;
	public int m_shields;

	public float m_armorClass;
	public int m_sheildClass;

	public bool m_immuneToMissiles;
	public bool m_immuneToLasers;

	public int m_laserClass;
	public int m_missileClass;

	public bool m_hasPlasmaBolts;

	public int m_elementIdA;
	public int m_elementVolumeA;

	public int m_elementIdB;
	public int m_elementVolumeB;

	public int m_elementIdC;
	public int m_elementVolumeC;

	public int m_enduriumVolume;
}
