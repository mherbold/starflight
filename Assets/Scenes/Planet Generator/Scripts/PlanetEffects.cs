
using UnityEngine;

public class PlanetEffects : MonoBehaviour
{
	public Renderer m_renderer;
	public float m_waterAnimationSpeed = 0.1f;
	public float m_waterAnimationRadius = 1.0f;
	public float m_waterScaleA = 4.0f;
	public float m_waterScaleB = 5.0f;

	Vector4 m_waterScale;
	Vector4 m_waterOffset;

	float m_angle1 = 0.0f;
	float m_angle2 = 2.0f;

	// unity update
	void Update()
	{
		m_angle1 += Time.deltaTime * m_waterAnimationSpeed;
		m_angle2 += Time.deltaTime * m_waterAnimationSpeed * 1.02f;

		if ( m_angle1 > ( 2.0f * Mathf.PI ) )
		{
			m_angle1 -= 2.0f * Mathf.PI;
		}

		if ( m_angle2 > ( 2.0f * Mathf.PI ) )
		{
			m_angle2 -= 2.0f * Mathf.PI;
		}

		m_waterScale = new Vector4( m_waterScaleA, m_waterScaleA, m_waterScaleB, m_waterScaleB );
		m_waterOffset = new Vector4( Mathf.Sin( m_angle1 ), Mathf.Cos( m_angle1 ), Mathf.Sin( m_angle2 ), Mathf.Cos( m_angle2 ) ) * m_waterAnimationRadius;

		m_renderer.material.SetVector( "_WaterScale", m_waterScale );
		m_renderer.material.SetVector( "_WaterOffset", m_waterOffset );
	}
}
