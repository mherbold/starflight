
using UnityEngine;

public class InfiniteStarfield : MonoBehaviour
{
	public float m_starfieldCubeSize = 100.0f;

	ParticleSystem m_particleSystem;

	ParticleSystem.Particle[] m_particles;

	Vector3 m_lastPosition;

	void Start()
	{
		m_particleSystem = GetComponent<ParticleSystem>();

		m_particles = new ParticleSystem.Particle[ m_particleSystem.main.maxParticles ];

		for ( var i = 0; i < m_particles.Length; i++ )
		{
			m_particles[ i ].position = ( new Vector3( Random.value, Random.value, Random.value ) * 1.0f - Vector3.one * 0.5f ) * m_starfieldCubeSize + transform.position;
			m_particles[ i ].startColor = m_particleSystem.main.startColor.Evaluate( 0.0f );
			m_particles[ i ].startSize = m_particleSystem.main.startSize.Evaluate( 0.0f );
			m_particles[ i ].velocity = Vector3.zero;
		}

		m_particleSystem.SetParticles( m_particles, m_particles.Length );
	}

	void LateUpdate()
	{
		if ( m_lastPosition != transform.position )
		{
			var travelVector = transform.position - m_lastPosition;

			m_lastPosition = transform.position;

			var xMultiplier = Mathf.Ceil( Mathf.Abs( travelVector.x / m_starfieldCubeSize ) );
			var yMultiplier = Mathf.Ceil( Mathf.Abs( travelVector.y / m_starfieldCubeSize ) );
			var zMultiplier = Mathf.Ceil( Mathf.Abs( travelVector.z / m_starfieldCubeSize ) );

			var centerToCorner = Vector3.one * m_starfieldCubeSize * 0.5f;

			var min = transform.position - centerToCorner;
			var max = transform.position + centerToCorner;

			var somethingChanged = false;

			for ( var i = 0; i < m_particles.Length; i++ )
			{
				var position = m_particles[ i ].position;

				if ( position.x < min.x )
				{
					position.x += m_starfieldCubeSize * xMultiplier;

					if ( position.x > max.x )
					{
						position.x -= m_starfieldCubeSize;
					}

					somethingChanged = true;
				}
				else if ( position.x > max.x )
				{
					position.x -= m_starfieldCubeSize * xMultiplier;

					if ( position.x < min.x )
					{
						position.x += m_starfieldCubeSize;
					}

					somethingChanged = true;
				}

				if ( position.y < min.y )
				{
					position.y += m_starfieldCubeSize * yMultiplier;

					if ( position.y > max.y )
					{
						position.y -= m_starfieldCubeSize;
					}

					somethingChanged = true;
				}
				else if ( position.y > max.y )
				{
					position.y -= m_starfieldCubeSize * yMultiplier;

					if ( position.y < min.y )
					{
						position.y += m_starfieldCubeSize;
					}

					somethingChanged = true;
				}

				if ( position.z < min.z )
				{
					position.z += m_starfieldCubeSize * zMultiplier;

					if ( position.z > max.z )
					{
						position.z -= m_starfieldCubeSize;
					}

					somethingChanged = true;
				}
				else if ( position.z > max.z )
				{
					position.z -= m_starfieldCubeSize * zMultiplier;

					if ( position.z < min.z )
					{
						position.z += m_starfieldCubeSize;
					}

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
