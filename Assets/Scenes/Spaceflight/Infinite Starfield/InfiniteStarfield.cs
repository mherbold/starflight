
using UnityEngine;

public class InfiniteStarfield : MonoBehaviour
{
	public int m_maximumNumberOfStars = 1000;
	public float m_sizeOfStars = 1.0f;
	public float m_maximumDistanceOfStars = 10.0f;
	public float m_fullyVisibleSpeed = 1.0f;
	public float m_fullyInvisibleSpeed = 0.0f;
	public Color32 m_colorOfStars = Color.white;

	private ParticleSystem m_particleSystem;
	private Material m_material;
	private ParticleSystem.Particle[] m_particles;
	private Vector3 m_lastPosition;

	void Start()
	{
		m_particleSystem = GetComponent<ParticleSystem>();

		m_material = m_particleSystem.GetComponent<ParticleSystemRenderer>().material;

		m_particles = new ParticleSystem.Particle[ m_maximumNumberOfStars ];

		for ( int i = 0; i < m_maximumNumberOfStars; i++ )
		{
			m_particles[ i ].position = ( new Vector3( Random.value, Random.value, Random.value ) * 2.0f - Vector3.one ) * m_maximumDistanceOfStars + transform.position;
			m_particles[ i ].startColor = m_colorOfStars;
			m_particles[ i ].startSize = m_sizeOfStars;
			m_particles[ i ].velocity = Vector3.zero;
		}

		m_particleSystem.SetParticles( m_particles, m_particles.Length );

		m_lastPosition = transform.position;
	}

	void Update()
	{
		Vector3 deltaPosition = m_lastPosition - transform.position;

		float speed = deltaPosition.magnitude;

		float alpha;

		if ( speed < m_fullyInvisibleSpeed )
		{
			alpha = 0.0f;
		}
		else if ( speed >= m_fullyVisibleSpeed )
		{
			alpha = 1.0f;
		}
		else
		{
			alpha = ( speed - m_fullyInvisibleSpeed ) / ( m_fullyVisibleSpeed - m_fullyInvisibleSpeed );
		}

		m_material.SetFloat( "_Alpha", alpha );

		if ( speed >= 0.0001f )
		{
			Vector3 centerToCorner = Vector3.one * m_maximumDistanceOfStars;

			Vector3 min = transform.position - centerToCorner;
			Vector3 max = transform.position + centerToCorner;

			bool somethingChanged = false;

			for ( int i = 0; i < m_maximumNumberOfStars; i++ )
			{
				Vector3 position = m_particles[ i ].position;

				if ( position.x < min.x )
				{
					position.x += m_maximumDistanceOfStars * 2.0f;

					somethingChanged = true;
				}
				else if ( position.x > max.x )
				{
					position.x -= m_maximumDistanceOfStars * 2.0f;

					somethingChanged = true;
				}

				if ( position.y < min.y )
				{
					position.y += m_maximumDistanceOfStars * 2.0f;

					somethingChanged = true;
				}
				else if ( position.y > max.y )
				{
					position.y -= m_maximumDistanceOfStars * 2.0f;

					somethingChanged = true;
				}

				if ( position.z < min.z )
				{
					position.z += m_maximumDistanceOfStars * 2.0f;

					somethingChanged = true;
				}
				else if ( position.z > max.z )
				{
					position.z -= m_maximumDistanceOfStars * 2.0f;

					somethingChanged = true;
				}

				m_particles[ i ].position = position;
			}

			if ( somethingChanged )
			{
				GetComponent<ParticleSystem>().SetParticles( m_particles, m_particles.Length );
			}

			m_lastPosition = transform.position;
		}
	}
}
