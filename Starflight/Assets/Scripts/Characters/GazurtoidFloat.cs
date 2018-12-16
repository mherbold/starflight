
using UnityEngine;

public class GazurtoidFloat : MonoBehaviour
{
	public float m_speed = 1.0f;
	public float m_range = 45.0f;

	Vector3 m_originalPosition;
	float m_timer;

	void Start()
	{
		m_originalPosition = transform.localPosition;
	}

	void Update()
	{
		m_timer += Time.deltaTime * m_speed;

		if ( m_timer > 360.0f )
		{
			m_timer -= 360.0f;
		}

		var offset = Mathf.Sin( m_timer ) * m_range;

		transform.localPosition = m_originalPosition + new Vector3( 0.0f, offset, 0.0f );
	}
}
