
using UnityEngine;

public class Mysterion : MonoBehaviour
{
	public float m_animationSpeed = 1.0f;
	public float m_originalPositionScale = 1.0f;
	public float m_simplexNoiseFrequency = 1.0f;
	public float m_positionScaleForScaleNoise = 1.0f;
	public float m_positionScaleForPositionNoise = 1.0f;
	public float m_outputPositionOffsetScale = 1.0f;

	public Material[] m_materials;

	float m_timer;

	FastNoise m_fastNoise;

	class Orb
	{
		public Transform m_transform;
		public Vector3 m_originalPosition;
	};

	Orb[] m_orbs;

	void Start()
	{
		m_orbs = new Orb[ transform.childCount ];

		for ( var i = 0; i < m_orbs.Length; i++ )
		{
			m_orbs[ i ] = new Orb();

			m_orbs[ i ].m_transform = transform.GetChild( i );
			m_orbs[ i ].m_originalPosition = m_orbs[ i ].m_transform.localPosition;

			var materialIndex = Random.Range( 0, m_materials.Length );

			var renderer = m_orbs[ i ].m_transform.gameObject.GetComponent<MeshRenderer>();

			renderer.material = m_materials[ materialIndex ];
		}

		m_fastNoise = new FastNoise();

		m_fastNoise.SetNoiseType( FastNoise.NoiseType.SimplexFractal );
	}

	void Update()
	{
		m_timer += Time.deltaTime * m_animationSpeed;

		m_fastNoise.SetFrequency( m_simplexNoiseFrequency );

		foreach ( var orb in m_orbs )
		{
			var xIn = orb.m_originalPosition.x * m_positionScaleForScaleNoise + m_timer;
			var yIn = orb.m_originalPosition.y * m_positionScaleForScaleNoise + m_timer;
			var zIn = orb.m_originalPosition.z * m_positionScaleForScaleNoise + m_timer;

			var scale = m_fastNoise.GetSimplex( xIn, yIn, zIn );

			scale = scale * 0.4f + 0.6f;

			orb.m_transform.localScale = new Vector3( scale, scale, scale );

			xIn = orb.m_originalPosition.x * m_positionScaleForPositionNoise + m_timer;
			yIn = orb.m_originalPosition.y * m_positionScaleForPositionNoise + m_timer;
			zIn = orb.m_originalPosition.z * m_positionScaleForPositionNoise + m_timer;

			var xOffset = m_fastNoise.GetSimplex( xIn, yIn, zIn );
			var yOffset = m_fastNoise.GetSimplex( yIn, zIn, xIn );
			var zOffset = m_fastNoise.GetSimplex( zIn, xIn, yIn );

			orb.m_transform.localPosition = orb.m_originalPosition * m_originalPositionScale + new Vector3( xOffset, yOffset, zOffset ) * m_outputPositionOffsetScale;
		}
	}
}
