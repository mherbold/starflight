
using UnityEngine;
using System;

[Serializable]

public class GD_Flux
{
	public int m_id;

	public int m_x1;
	public int m_y1;

	public int m_x2;
	public int m_y2;

	Vector3 m_from;
	Vector3 m_to;

	public void Initialize()
	{
		m_from = Tools.GameToWorldCoordinates( new Vector3( m_x1, 0.0f, m_y1 ) );
		m_to = Tools.GameToWorldCoordinates( new Vector3( m_x2, 0.0f, m_y2 ) );
	}

	public Vector3 GetFrom()
	{
		return m_from;
	}

	public Vector3 GetTo()
	{
		return m_to;
	}

	public float GetBreachDistance()
	{
		return 96.0f;
	}
}
