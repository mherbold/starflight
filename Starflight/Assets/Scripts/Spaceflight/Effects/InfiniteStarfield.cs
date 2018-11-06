
using UnityEngine;

public class InfiniteStarfield : MonoBehaviour
{
	public float m_maximumDistanceOfStars = 10.0f;

	private ParticleSystem m_particleSystem;
	private Material m_material;
	private ParticleSystem.Particle[] m_particles;

	void Start()
	{
		m_particleSystem = GetComponent<ParticleSystem>();

		m_material = m_particleSystem.GetComponent<ParticleSystemRenderer>().material;

		m_particles = new ParticleSystem.Particle[ m_particleSystem.main.maxParticles ];

		for ( int i = 0; i < m_particles.Length; i++ )
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

		float speed = playerData.m_general.m_currentSpeed;

		float alpha = Mathf.Lerp( 0.0f, 1.0f, speed / playerData.m_general.m_currentMaximumSpeed );

		var color = m_material.GetColor( "SF_AlbedoColor" );

		color.a = alpha;

		m_material.SetColor( "SF_AlbedoColor", color );

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
