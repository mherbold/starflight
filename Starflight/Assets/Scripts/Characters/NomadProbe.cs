
using UnityEngine;

public class NomadProbe : MonoBehaviour
{
	public float m_blinkInterval1 = 3.5f;
	public float m_blinkInterval2 = 0.5f;
	public float m_blinkDuration = 0.25f;

	public GameObject[] m_lights;

	bool m_pingPong;
	float m_timer;

	void Start()
	{
	}

	void Update()
	{
		m_timer += Time.deltaTime;

		var lightsOn = false;

		if ( m_pingPong )
		{
			if ( m_timer >= m_blinkInterval2 )
			{
				if ( m_timer >= m_blinkInterval2 + m_blinkDuration )
				{
					m_timer -= m_blinkInterval2 + m_blinkDuration;

					m_pingPong = false;
				}
				else
				{
					lightsOn = true;
				}
			}
		}
		else
		{
			if ( m_timer >= m_blinkInterval1 )
			{
				if ( m_timer >= m_blinkInterval1 + m_blinkDuration )
				{
					m_timer -= m_blinkInterval1 + m_blinkDuration;

					m_pingPong = true;
				}
				else
				{
					lightsOn = true;
				}
			}
		}

		SetActive( lightsOn );
	}

	void SetActive( bool active )
	{
		foreach ( var light in m_lights )
		{
			if ( light.activeInHierarchy != active )
			{
				light.SetActive( active );
			}
		}
	}
}
