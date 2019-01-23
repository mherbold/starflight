
using UnityEngine;

using System.Collections.Generic;

public class TerrainGrid : MonoBehaviour
{
	public int m_numLevels = 3;
	public int m_baseSize = 100;
	public int m_detail = 9;
	public float m_elevationScale = 100.0f;
	public float m_elevationOffset = 16.0f;

	Mesh m_mesh;
	MeshFilter m_meshFilter;
	MeshRenderer m_meshRenderer;
	Material m_material;

	List<Vector3> m_vertices;

	float m_gridSize;
	float m_gridOffset;

	PlanetGenerator m_planetGenerator;

	bool m_initialized;

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
	}

	// hide the terrain
	public void Hide()
	{
		gameObject.SetActive( false );
	}

	// show the terrain
	public void Show()
	{
		gameObject.SetActive( true );
	}

	// initialize the terrain
	void Initialize()
	{
		if ( m_initialized )
		{
			return;
		}

		// create a mesh
		m_mesh = new Mesh();

		// get the mesh filter component
		m_meshFilter = GetComponent<MeshFilter>();

		// set our mesh into the mesh filter
		m_meshFilter.mesh = m_mesh;

		// get the mesh renderer component
		m_meshRenderer = GetComponent<MeshRenderer>();

		// create an instance of the material
		m_material = new Material( m_meshRenderer.material );

		// put our instance on the mesh renderer
		m_meshRenderer.material = m_material;

		// generate the mesh
		GenerateTerrain();

		m_initialized = true;
	}

	// call this to update the landing grid with the selected landing coordinates
	public void SetLandingCoordinates( float latitude, float longitude, PlanetGenerator planetGenerator )
	{
		// initialize the terrain grid mesh
		Initialize();

		// set the planet generator
		m_planetGenerator = planetGenerator;

		// convert from -180,180 to 0,1
		var x = ( latitude + 180.0f ) / 360.0f;

		// convert from -90,90 to 0.125,0.875
		var y = Mathf.Lerp( 0.125f, 0.875f, ( longitude + 90.0f ) / 180.0f );

		// constant scale factors
		var zoom = 2.0f;

		var xScale = 0.5f / zoom;
		var yScale = 1.0f / zoom;

		var xOffset = x - xScale / 2.0f;
		var yOffset = y - yScale / 2.0f;

		// calculate scale and offset
		var scaleOffset = new Vector4( xScale, yScale, xOffset, yOffset );

		m_material.SetVector( "SF_BaseScaleOffset", scaleOffset );

		// get the maximum height of the area we are landing on top of
		var centerGridCount = m_detail * m_detail;

		var centerRadiusSquared = Mathf.Pow( m_baseSize * 0.25f, 2.0f );

		var maxElevation = float.MinValue;

		for ( var i = 0; i < centerGridCount; i++ )
		{
			var position = GetVertexPosition( i, xScale, yScale, xOffset, yOffset );

			if ( ( ( position.x * position.x ) + ( position.z * position.z ) ) < centerRadiusSquared )
			{
				if ( position.y > maxElevation )
				{
					maxElevation = position.y;
				}
			}
		}

		maxElevation += m_elevationOffset;
		 
		// update the vertices with the elevation data
		for ( var i = 0; i < m_vertices.Count; i++ )
		{
			var position = GetVertexPosition( i, xScale, yScale, xOffset, yOffset );

			m_vertices[ i ] = new Vector3( position.x, position.y - maxElevation, position.z );
		}

		// update the mesh
		m_mesh.SetVertices( m_vertices );

		// recalculate the bounds
		m_mesh.RecalculateBounds();

		// update the textures on the material
		m_material.SetTexture( "SF_AlbedoMap", planetGenerator.m_albedoTexture );
		m_material.SetTexture( "SF_SpecularMap", planetGenerator.m_specularTexture );
		m_material.SetTexture( "SF_NormalMap", planetGenerator.m_normalTexture );
		m_material.SetTexture( "SF_WaterMaskMap", planetGenerator.m_waterMaskTexture );
	}

	Vector3 GetVertexPosition( int i, float xScale, float yScale, float xOffset, float yOffset )
	{
		var vertex = m_vertices[ i ];

		var x = ( vertex.x + m_gridOffset ) / m_gridSize * xScale + xOffset;
		var z = ( vertex.z + m_gridOffset ) / m_gridSize * yScale + yOffset;

		x *= m_planetGenerator.m_textureMapWidth;
		z *= m_planetGenerator.m_textureMapHeight;

		var iX = Mathf.FloorToInt( x );
		var iZ = Mathf.FloorToInt( z );

		var x0 = iX - 1;
		var x1 = x0 + 1;
		var x2 = x1 + 1;
		var x3 = x2 + 1;

		var z0 = iZ - 1;
		var z1 = z0 + 1;
		var z2 = z1 + 1;
		var z3 = z2 + 1;

		var h00 = GetElevation( x0, z0 );
		var h01 = GetElevation( x1, z0 );
		var h02 = GetElevation( x2, z0 );
		var h03 = GetElevation( x3, z0 );

		var h0 = InterpolateHermite( h00, h01, h02, h03, x - iX );

		var h10 = GetElevation( x0, z1 );
		var h11 = GetElevation( x1, z1 );
		var h12 = GetElevation( x2, z1 );
		var h13 = GetElevation( x3, z1 );

		var h1 = InterpolateHermite( h10, h11, h12, h13, x - iX );

		var h20 = GetElevation( x0, z2 );
		var h21 = GetElevation( x1, z2 );
		var h22 = GetElevation( x2, z2 );
		var h23 = GetElevation( x3, z2 );

		var h2 = InterpolateHermite( h20, h21, h22, h23, x - iX );

		var h30 = GetElevation( x0, z3 );
		var h31 = GetElevation( x1, z3 );
		var h32 = GetElevation( x2, z3 );
		var h33 = GetElevation( x3, z3 );

		var h3 = InterpolateHermite( h30, h31, h32, h33, x - iX );

		var elevation = InterpolateHermite( h0, h1, h2, h3, z - iZ );

		return new Vector3( vertex.x, elevation * m_elevationScale, vertex.z );
	}

	float GetElevation( float x, float z )
	{
		var ix = Mathf.FloorToInt( x );
		var iz = Mathf.FloorToInt( z );

		ix = ( ix + m_planetGenerator.m_textureMapWidth ) & ( m_planetGenerator.m_textureMapWidth - 1 );

		if ( iz < 0 )
		{
			iz = 0;
		}
		else if ( iz >= m_planetGenerator.m_textureMapHeight )
		{
			iz = m_planetGenerator.m_textureMapHeight - 1;
		}

		var elevation = m_planetGenerator.m_elevation[ iz, ix ];

		if ( elevation < m_planetGenerator.m_waterHeight )
		{
			elevation = m_planetGenerator.m_waterHeight;
		}

		return elevation;
	}

	float InterpolateHermite( float v0, float v1, float v2, float v3, float t )
	{
		var a = 2.0f * v1;
		var b = v2 - v0;
		var c = 2.0f * v0 - 5.0f * v1 + 4.0f * v2 - v3;
		var d = -v0 + 3.0f * v1 - 3.0f * v2 + v3;

		return 0.5f * ( a + ( b * t ) + ( c * t * t ) + ( d * t * t * t ) );
	}

	// generates our terrain mesh
	void GenerateTerrain()
	{
		// calculate exclusion zone range
		var exZoneA = m_detail / 4;
		var exZoneB = m_detail / 4 + m_detail / 2;

		// create our vertex list
		m_vertices = new List<Vector3>();

		// create our texcoord list
		var texCoords = new List<Vector2>();

		// create our normal list
		var normals = new List<Vector3>();

		// allocate temp array
		var vertexMap = new int[ m_numLevels, m_detail, m_detail ];

		// calculate the starting offset
		var offset = 0.0f;

		var currentSize = m_baseSize;

		for ( var i = 0; i < m_numLevels - 1; i++ )
		{
			offset += currentSize * 0.5f;

			currentSize *= 2;
		}

		// save our physical grid size
		m_gridSize = currentSize;
		m_gridOffset = m_gridSize * 0.5f;

		// generate the vertex list
		var vertexIndex = 0;

		currentSize = m_baseSize;

		for ( var i = 0; i < m_numLevels; i++ )
		{
			for ( var y = 0; y < m_detail; y++ )
			{
				var py = y * (float) currentSize / ( m_detail - 1 ) + offset;

				for ( var x = 0; x < m_detail; x++ )
				{
					if ( ( i > 0 ) && ( x >= exZoneA ) && ( x <= exZoneB ) && ( y >= exZoneA ) && ( y <= exZoneB ) )
					{
						continue;
					}

					var px = x * (float) currentSize / ( m_detail - 1 ) + offset;

					m_vertices.Add( new Vector3( px - m_gridOffset, 0.0f, py - m_gridOffset ) );

					texCoords.Add( new Vector2( px / m_gridSize, py / m_gridSize ) );

					normals.Add( Vector3.up );

					vertexMap[ i, y, x ] = vertexIndex++;
				}
			}

			offset -= currentSize * 0.5f;

			currentSize *= 2;
		}

		// set the vertices on the mesh
		m_mesh.SetVertices( m_vertices );

		m_mesh.SetUVs( 0, texCoords );

		m_mesh.SetNormals( normals );

		// create our triangle list
		var triangles = new List<int>();

		// generate the triangle list
		for ( var i = 0; i < m_numLevels; i++ )
		{
			for ( var y = 0; y < m_detail - 1; y++ )
			{
				for ( var x = 0; x < m_detail - 1; x++ )
				{
					if ( i == 0 )
					{
						if ( ( ( x + y ) & 1 ) == 0 )
						{
							triangles.Add( vertexMap[ i, y, x ] );
							triangles.Add( vertexMap[ i, y + 1, x ] );
							triangles.Add( vertexMap[ i, y + 1, x + 1 ] );

							triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
							triangles.Add( vertexMap[ i, y, x + 1 ] );
							triangles.Add( vertexMap[ i, y, x ] );
						}
						else
						{
							triangles.Add( vertexMap[ i, y, x ] );
							triangles.Add( vertexMap[ i, y + 1, x ] );
							triangles.Add( vertexMap[ i, y, x + 1 ] );

							triangles.Add( vertexMap[ i, y, x + 1 ] );
							triangles.Add( vertexMap[ i, y + 1, x ] );
							triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
						}
					}
					else if ( ( x < exZoneA - 1 ) || ( x > exZoneB ) || ( y < exZoneA - 1 ) || ( y > exZoneB ) )
					{
						if ( ( ( x + y ) & 1 ) == 0 )
						{
							triangles.Add( vertexMap[ i, y, x ] );
							triangles.Add( vertexMap[ i, y + 1, x ] );
							triangles.Add( vertexMap[ i, y + 1, x + 1 ] );

							triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
							triangles.Add( vertexMap[ i, y, x + 1 ] );
							triangles.Add( vertexMap[ i, y, x ] );
						}
						else
						{
							triangles.Add( vertexMap[ i, y, x ] );
							triangles.Add( vertexMap[ i, y + 1, x ] );
							triangles.Add( vertexMap[ i, y, x + 1 ] );

							triangles.Add( vertexMap[ i, y, x + 1 ] );
							triangles.Add( vertexMap[ i, y + 1, x ] );
							triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
						}
					}
					else
					{
						if ( x < exZoneA )
						{
							if ( y < exZoneA )
							{
								triangles.Add( vertexMap[ i, y, x ] );
								triangles.Add( vertexMap[ i, y + 1, x ] );
								triangles.Add( vertexMap[ i - 1, 0, 0 ] );

								triangles.Add( vertexMap[ i - 1, 0, 0 ] );
								triangles.Add( vertexMap[ i, y, x + 1 ] );
								triangles.Add( vertexMap[ i, y, x ] );
							}
							else if ( y >= exZoneB )
							{
								triangles.Add( vertexMap[ i, y + 1, x ] );
								triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
								triangles.Add( vertexMap[ i - 1, m_detail - 1, 0 ] );

								triangles.Add( vertexMap[ i - 1, m_detail - 1, 0 ] );
								triangles.Add( vertexMap[ i, y, x ] );
								triangles.Add( vertexMap[ i, y + 1, x ] );
							}
							else
							{
								var y2 = ( y - exZoneA ) * 2;

								triangles.Add( vertexMap[ i, y, x ] );
								triangles.Add( vertexMap[ i - 1, y2 + 1, 0 ] );
								triangles.Add( vertexMap[ i - 1, y2, 0 ] );

								triangles.Add( vertexMap[ i, y, x ] );
								triangles.Add( vertexMap[ i, y + 1, x ] );
								triangles.Add( vertexMap[ i - 1, y2 + 1, 0 ] );

								triangles.Add( vertexMap[ i, y + 1, x ] );
								triangles.Add( vertexMap[ i - 1, y2 + 2, 0 ] );
								triangles.Add( vertexMap[ i - 1, y2 + 1, 0 ] );
							}
						}
						else if ( x >= exZoneB )
						{
							if ( y < exZoneA )
							{
								triangles.Add( vertexMap[ i, y, x + 1 ] );
								triangles.Add( vertexMap[ i, y, x ] );
								triangles.Add( vertexMap[ i - 1, 0, m_detail - 1 ] );

								triangles.Add( vertexMap[ i - 1, 0, m_detail - 1 ] );
								triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
								triangles.Add( vertexMap[ i, y, x + 1 ] );
							}
							else if ( y >= exZoneB )
							{
								triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
								triangles.Add( vertexMap[ i, y, x + 1 ] );
								triangles.Add( vertexMap[ i - 1, m_detail - 1, m_detail - 1 ] );

								triangles.Add( vertexMap[ i - 1, m_detail - 1, m_detail - 1 ] );
								triangles.Add( vertexMap[ i, y + 1, x ] );
								triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
							}
							else
							{
								var y2 = ( y - exZoneA ) * 2;

								triangles.Add( vertexMap[ i, y, x + 1 ] );
								triangles.Add( vertexMap[ i - 1, y2, m_detail - 1 ] );
								triangles.Add( vertexMap[ i - 1, y2 + 1, m_detail - 1 ] );

								triangles.Add( vertexMap[ i, y, x + 1 ] );
								triangles.Add( vertexMap[ i - 1, y2 + 1, m_detail - 1 ] );
								triangles.Add( vertexMap[ i, y + 1, x + 1 ] );

								triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
								triangles.Add( vertexMap[ i - 1, y2 + 1, m_detail - 1 ] );
								triangles.Add( vertexMap[ i - 1, y2 + 2, m_detail - 1 ] );
							}
						}
						else if ( y < exZoneA )
						{
							var x2 = ( x - exZoneA ) * 2;

							triangles.Add( vertexMap[ i, y, x ] );
							triangles.Add( vertexMap[ i - 1, 0, x2 ] );
							triangles.Add( vertexMap[ i - 1, 0, x2 + 1 ] );

							triangles.Add( vertexMap[ i, y, x ] );
							triangles.Add( vertexMap[ i - 1, 0, x2 + 1 ] );
							triangles.Add( vertexMap[ i, y, x + 1 ] );

							triangles.Add( vertexMap[ i, y, x + 1 ] );
							triangles.Add( vertexMap[ i - 1, 0, x2 + 1 ] );
							triangles.Add( vertexMap[ i - 1, 0, x2 + 2 ] );
						}
						else if ( y >= exZoneB )
						{
							var x2 = ( x - exZoneA ) * 2;

							triangles.Add( vertexMap[ i, y + 1, x ] );
							triangles.Add( vertexMap[ i - 1, m_detail - 1, x2 + 1 ] );
							triangles.Add( vertexMap[ i - 1, m_detail - 1, x2 ] );

							triangles.Add( vertexMap[ i, y + 1, x ] );
							triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
							triangles.Add( vertexMap[ i - 1, m_detail - 1, x2 + 1 ] );

							triangles.Add( vertexMap[ i, y + 1, x + 1 ] );
							triangles.Add( vertexMap[ i - 1, m_detail - 1, x2 + 2 ] );
							triangles.Add( vertexMap[ i - 1, m_detail - 1, x2 + 1 ] );
						}
					}
				}
			}
		}

		// set the triangles on the mesh
		m_mesh.SetTriangles( triangles, 0, true );

		// generate tangent vectors
		m_mesh.RecalculateTangents();

		// recalculate the bounds
		m_mesh.RecalculateBounds();
	}
}
