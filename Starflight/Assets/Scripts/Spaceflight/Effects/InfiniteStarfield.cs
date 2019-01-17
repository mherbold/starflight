
using UnityEngine;

public class InfiniteStarfield : MonoBehaviour
{
	public float m_maximumDistanceOfStars = 10.0f;

	ParticleSystem m_particleSystem;

	Material m_material;

	ParticleSystem.Particle[] m_particles;

	Vector3 m_lastPosition;

	void Start()
	{
		m_particleSystem = GetComponent<ParticleSystem>();

		m_material = m_particleSystem.GetComponent<ParticleSystemRenderer>().material;

		m_particles = new ParticleSystem.Particle[ m_particleSystem.main.maxParticles ];

		for ( var i = 0; i < m_particles.Length; i++ )
		{
			m_particles[ i ].position = ( new Vector3( Random.value, Random.value, Random.value ) * 2.0f - Vector3.one ) * m_maximumDistanceOfStars + transform.position;
			m_particles[ i ].startColor = m_particleSystem.main.startColor.Evaluate( 0.0f );
			m_particles[ i ].startSize = m_particleSystem.main.startSize.Evaluate( 0.0f );
			m_particles[ i ].velocity = Vector3.zero;
		}

		m_particleSystem.SetParticles( m_particles, m_particles.Length );
	}

	void Update()
	{
		var playerData = DataController.m_instance.m_playerData;

		float speed = Vector3.Magnitude( transform.position - m_lastPosition ) / Time.deltaTime;

		m_lastPosition = transform.position;

		float alpha = Mathf.Clamp( Mathf.Pow( speed / playerData.m_general.m_currentMaximumSpeed, 0.4f ), 0.0f, 1.0f );

		Tools.SetOpacity( m_material, alpha );

		if ( speed != 0.0f )
		{
			Vector3 centerToCorner = Vector3.one * m_maximumDistanceOfStars;

			Vector3 min = transform.position - centerToCorner;
			Vector3 max = transform.position + centerToCorner;

			bool somethingChanged = false;

			for ( int i = 0; i < m_particles.Length; i++ )
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
		}
	}
}
