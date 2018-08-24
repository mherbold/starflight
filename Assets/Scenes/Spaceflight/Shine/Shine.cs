
using UnityEngine;

public class Shine : MonoBehaviour
{
	const int c_numVertices = 256;

	public float m_speed = 1.0f;
    public float m_size = 5.0f;

	private Vector3[] m_positions;
	private Vector2[] m_uv;
	private int[] m_triangles;

	private Mesh m_mesh;
	private MeshFilter m_meshFilter;
	private MeshRenderer m_meshRenderer;
	private Material m_material;

	private float m_lerpTime;

	private void Start()
	{
		m_mesh = new Mesh();
		m_mesh.MarkDynamic();

		m_meshFilter = GetComponent<MeshFilter>();
		m_meshFilter.mesh = m_mesh;

		m_meshRenderer = GetComponent<MeshRenderer>();
		m_material = m_meshRenderer.material;

		m_positions = new Vector3[ c_numVertices ];
		m_uv = new Vector2[ c_numVertices ];
		m_triangles = new int[ ( c_numVertices - 1 ) * 3 ];

		InitializeMesh();

		m_mesh.vertices = m_positions;
		m_mesh.uv = m_uv;
		m_mesh.triangles = m_triangles;
        m_mesh.bounds = new Bounds( new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( m_size, m_size, 0.0f ) );

		m_lerpTime = 0.0f;
	}

	private void Update()
	{
		m_lerpTime += Time.deltaTime * m_speed;

		if ( m_lerpTime >= Mathf.PI * 2.0f )
		{
			m_lerpTime -= Mathf.PI * 2.0f;
		}

		m_material.SetFloat( "_LerpTime", m_lerpTime );
	}

	private void InitializeMesh()
	{
		m_positions[ 0 ] = new Vector3( 0.0f, 0.0f, 0.0f );
		m_uv[ 0 ] = new Vector2( 0.0f, 0.0f );

		for ( int i = 1; i < c_numVertices - 1; i++ )
		{
			int offset = ( i - 1 ) % ( c_numVertices - 2 );

			float angle = ( ( Mathf.PI * 2.0f ) / (float) ( c_numVertices - 2 ) ) * (float) offset;

			m_positions[ i ] = new Vector3( angle, Random.Range( 0.0f, Mathf.PI * 2.0f ), 1.0f );
			m_uv[ i ] = new Vector2( 1.0f, 0.0f );
		}

		m_positions[ c_numVertices - 1 ] = m_positions[ 1 ];
		m_uv[ c_numVertices - 1 ] = m_uv[ 1 ];

		int vertexIndex = 0;

		for ( int i = 1; i < c_numVertices; i++ )
		{
			m_triangles[ vertexIndex++ ] = 0;
			m_triangles[ vertexIndex++ ] = i;
			m_triangles[ vertexIndex++ ] = ( i + 1 ) % c_numVertices;
		}
	}
}
