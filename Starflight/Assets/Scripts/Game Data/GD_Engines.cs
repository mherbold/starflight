using System;

[Serializable]

public class GD_Engines : GD_ShipPart
{
	public int m_minimumAcceleration;
	public int m_maximumAcceleration;
	public float m_powerCurve;
	public float m_powerScale;
	public float m_fuelUsedPerCoordinate;
}
