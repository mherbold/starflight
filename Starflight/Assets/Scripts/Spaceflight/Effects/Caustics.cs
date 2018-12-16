
using UnityEngine;

public class Caustics : MonoBehaviour
{
	public Vector3 m_lightDirection = new Vector3( 0.0f, -1.0f, 0.0f );
	public Vector3 m_movementDirection = new Vector3( 1.0f, 0.0f, -1.0f );
	public float m_scale = 1.0f;
	public float m_framesPerSecond = 29.97f;
	public Texture[] m_causticsTextures;

	Renderer m_renderer;
	Material[] m_materials;
	float m_timer;
	Vector3 m_position;

	void Start()
	{
		m_renderer = GetComponent<Renderer>();

		m_materials = new Material[ m_renderer.materials.Length ];

		for ( var i = 0; i < m_renderer.materials.Length; i++ )
		{
			m_materials[ i ] = new Material( m_renderer.materials[ i ] );
		}

		m_renderer.materials = m_materials;

		m_timer = 0.0f;
		m_position = Vector3.zero;
	}

	void Update()
	{
		if ( ( m_causticsTextures != null ) && ( m_causticsTextures.Length > 1 ) )
		{
			var totalTime = m_causticsTextures.Length / m_framesPerSecond;

			m_timer += Time.deltaTime;

			if ( m_timer >= totalTime )
			{
				m_timer -= totalTime;
			}

			var index = Mathf.FloorToInt( m_timer * m_causticsTextures.Length / totalTime );

			for ( var i = 0; i < m_materials.Length; i++ )
			{
				m_materials[ i ].SetTexture( "SF_EmissiveMap", m_causticsTextures[ index ] );
			}
		}

		m_position += m_movementDirection * Time.deltaTime;

		var rotation = Quaternion.LookRotation( m_lightDirection, new Vector3( m_lightDirection.z, m_lightDirection.x, m_lightDirection.y ) );

		var projectionMatrix = Matrix4x4.Transpose( Matrix4x4.TRS( m_position, rotation, Vector3.one / m_scale ) );

		for ( var i = 0; i < m_materials.Length; i++ )
		{
			m_materials[ i ].SetMatrix( "SF_ProjectionMatrix", projectionMatrix );
		}
	}
}
