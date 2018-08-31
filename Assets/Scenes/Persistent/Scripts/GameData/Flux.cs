
using UnityEngine;
using System;

[Serializable]

public class Flux
{
	public int m_x1;
	public int m_y1;
	public int m_x2;
	public int m_y2;

	public SerializableVector3 m_from;
	public SerializableVector3 m_to;

	public void Initialize()
	{
		m_from = Tools.GameToWorldCoordinates( new Vector3( m_x1, 0.0f, m_y1 ) );
		m_to = Tools.GameToWorldCoordinates( new Vector3( m_x2, 0.0f, m_y2 ) );
	}

	public float GetBreachDistance()
	{
		return 64.0f;
	}
}
