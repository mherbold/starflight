using UnityEngine;
using Random = UnityEngine.Random;

public class InfiniteStarfield : MonoBehaviour
{
	[System.Serializable]
	public struct Starfield
	{
		public Material m_mat;
		public Color m_color;
		public int m_maxParticles;
		public float m_startSize;
		public float m_height;
		public float m_heightBounds;
		public float m_2dBounds;
	}

	[SerializeField] bool m_yIsUp;
	[SerializeField] Starfield[] m_starfields;
	
	ParticleSystem[] m_particleSystems;
	ParticleSystem.Particle[][] m_particles;
	Vector3 m_lastPosition;

	void Start()
	{
		InitStarField();
	}

	public void InitFromUnityButton()
	{
		InitStarField();
	}
	
	public void InitStarField(params Starfield[] starfields)
	{
		foreach ( Transform child in transform )
			Destroy( child.gameObject );

		if ( starfields.Length > 0 )
			m_starfields = starfields;

		var cameraPos = transform.position;
		var aspect = Camera.main.aspect;
		
		int count = m_starfields.Length;
		m_particleSystems = new ParticleSystem[ count ];
		m_particles = new ParticleSystem.Particle[ count ][];

		for ( int i = 0; i < count; i++ )
		{
			var starfield = m_starfields[ i ];
			var psHolder = new GameObject($"starfield {i}");
			psHolder.transform.SetParent(transform, false);
			var ps = m_particleSystems[ i ] = psHolder.AddComponent<ParticleSystem>();
			var emission = ps.emission;
			emission.enabled = false;
			var shape = ps.shape;
			shape.enabled = false;
			var main = ps.main;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
			main.playOnAwake = false;
			main.loop = false;
			main.maxParticles = starfield.m_maxParticles;
			main.startSize = starfield.m_startSize;
			main.startColor = starfield.m_color;
			var partRend = ps.GetComponent<Renderer>();
			partRend.material = starfield.m_mat;

			Vector3 startingPosition, bounds;
			if (m_yIsUp)
			{
				startingPosition = new Vector3( cameraPos.x, starfield.m_height, cameraPos.z );
				bounds = new Vector3( starfield.m_2dBounds * aspect, starfield.m_heightBounds, starfield.m_2dBounds );
			}
			else
			{
				startingPosition = new Vector3( cameraPos.x, cameraPos.y, starfield.m_height );
				bounds = new Vector3( starfield.m_2dBounds * aspect, starfield.m_2dBounds, starfield.m_heightBounds );
			}
			
			m_particles[ i ] = new ParticleSystem.Particle[ ( int )( starfield.m_maxParticles * aspect ) ];
			for ( var j = 0; j < m_particles[ i ].Length; j++ )
			{
				m_particles[ i ][ j ].position = startingPosition + Vector3.Scale( new Vector3( Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f ), bounds );
				m_particles[ i ][ j ].startColor = main.startColor.Evaluate( 0.0f );
				m_particles[ i ][ j ].startSize = main.startSize.Evaluate( 0.0f );
				m_particles[ i ][ j ].rotation = Random.value * 360f;
				m_particles[ i ][ j ].startLifetime = m_particles[ i ][ j ].remainingLifetime = float.MaxValue;
			}
			
			ps.SetParticles( m_particles[ i ],  m_particles[ i ].Length );
		}
	}
	
	void LateUpdate()
	{
		if ( m_lastPosition != transform.position )
		{
			var aspect = Camera.main.aspect;
			var cameraPos = transform.position;
			m_lastPosition = cameraPos;

			for ( int i = 0; i < m_particleSystems.Length; i++ )
			{
				var starfield = m_starfields[ i ];
				var ps = m_particleSystems[ i ];

				Vector3 startingPosition, bounds;
				if (m_yIsUp)
				{
					startingPosition = new Vector3( cameraPos.x, starfield.m_height, cameraPos.z );
					bounds = new Vector3( starfield.m_2dBounds * aspect, starfield.m_heightBounds, starfield.m_2dBounds );
				}
				else
				{
					startingPosition = new Vector3( cameraPos.x, cameraPos.y, starfield.m_height );
					bounds = new Vector3( starfield.m_2dBounds * aspect, starfield.m_2dBounds, starfield.m_heightBounds );
				}

				var minCorner = startingPosition + Vector3.Scale( new Vector3( -0.5f, -0.5f, -0.5f ), bounds );
				var maxCorner = startingPosition + Vector3.Scale( new Vector3( 0.5f, 0.5f, 0.5f ), bounds );
				
				for ( var j = 0; j < m_particles[ i ].Length; j++ )
				{
					var position = m_particles[ i ][ j ].position;

					if ( position.x < minCorner.x )
						position.x += bounds.x;
					else if ( position.x > maxCorner.x )
						position.x -= bounds.x;

					if ( !m_yIsUp )
					{
						if ( position.y < minCorner.y )
							position.y += bounds.y;
						else if ( position.x > maxCorner.x )
							position.y -= bounds.y;
					}
					else
					{
						if ( position.z < minCorner.z )
							position.z += bounds.z;
						else if ( position.z > maxCorner.z )
							position.z -= bounds.z;
					}
					
					m_particles[ i ][ j ].position = position;
				}
				
				ps.SetParticles( m_particles[ i ], m_particles[ i ].Length );
			}
		}
	}
}
