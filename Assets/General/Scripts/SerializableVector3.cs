
using UnityEngine;
using System;

[Serializable]

public class SerializableVector3
{
	public float m_x;
	public float m_y;
	public float m_z;

	public SerializableVector3( Vector3 vector )
	{
		m_x = vector.x;
		m_y = vector.y;
		m_z = vector.z;
	}

	public static implicit operator Vector3( SerializableVector3 vector )
	{
		return new Vector3( vector.m_x, vector.m_y, vector.m_z );
	}

	public static implicit operator SerializableVector3( Vector3 vector )
	{
		return new SerializableVector3( vector );
	}
}
