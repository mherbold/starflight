
using UnityEngine;

public class SpeminBreathe : MonoBehaviour
{
	public float m_speed;
	public float m_minScale;
	public float m_maxScale;

	float m_timer;
	Vector3 m_originalScale;

	void Start()
	{
		m_timer = 0.0f;
		m_originalScale = transform.localScale;
	}

	void Update()
	{
		m_timer += Time.deltaTime * m_speed;

		if ( m_timer >= Mathf.PI * 2.0f )
		{
			m_timer -= Mathf.PI * 2.0f;
		}

		var cycle = Mathf.Sin( m_timer ) * 0.5f + 0.5f;

		transform.localScale = m_originalScale + new Vector3( 0.0f, 0.0f, Mathf.SmoothStep( m_minScale, m_maxScale, cycle ) );
	}
}
