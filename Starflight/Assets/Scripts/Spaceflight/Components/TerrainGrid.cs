
using UnityEngine;

using System.Collections.Generic;

public class TerrainGrid : MonoBehaviour
{
	public int m_numLevels = 3;
	public int m_baseSize = 100;
	public int m_detail = 9;

	public float m_elevationScale = 100.0f;
	public float m_elevationOffset = 16.0f;

	public TerrainRocks m_terrainRocks;
	public TerrainElements m_terrainElements;

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
		// generate the base terrain grid
		Initialize();
	}

	// initialize the terrain
	public void Initialize()
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

	// call this to set an elevation texture and switch the mode to dynamic terrain grid
	public void SetElevationMap( Texture2D elevationTexture, PlanetGenerator planetGenerator )
	{
		// save the planet generator
		m_planetGenerator = planetGenerator;

		// make sure the terrain grid mesh is initialized
		Initialize();

		// set the new elevation map
		m_material.SetTexture( "SF_ElevationMap", elevationTexture );
		m_material.SetFloat( "SF_ElevationScale", m_elevationScale * 4.0f ); // multiply by 4 because R16 tex map has elevations divided by 4

		// force the bounds to be the maximum possible extents
		m_mesh.bounds = new Bounds( Vector3.zero, new Vector3( 1024.0f, 512.0f, 1024.0f ) );

		// update the textures on the material
		m_material.SetTexture( "_MainTex", planetGenerator.m_albedoTexture );
		m_material.SetTexture( "SF_SpecularMap", planetGenerator.m_specularTexture );
		m_material.SetTexture( "SF_NormalMap", planetGenerator.m_normalTexture );
		m_material.SetTexture( "SF_WaterMaskMap", planetGenerator.m_waterMaskTexture );

		// populate the rocks
		if ( m_terrainRocks != null )
		{
			m_terrainRocks.Initialize( m_planetGenerator, m_elevationScale );
		}

		// populate the elements
		if ( m_terrainElements != null )
		{
			m_terrainElements.Initialize( m_planetGenerator, m_elevationScale );
		}
	}

	// call this to bake in the elevation at the selected latitude and longitude
	public void BakeInElevation( float latitude, float longitude, PlanetGenerator planetGenerator )
	{
		// make sure the terrain grid mesh is initialized
		Initialize();

		// save the planet generator
		m_planetGenerator = planetGenerator;

		// convert from -180,180 to 0,1 (texture coordinates)
		var x = ( latitude + 180.0f ) / 360.0f;

		// convert from -90,90 to 0.125,0.875 (texture coordinates)
		var y = Mathf.Lerp( 0.125f, 0.875f, ( longitude + 90.0f ) / 180.0f );

		// constant scale factors
		var zoom = 2.0f;

		var xScale = 0.5f / zoom;
		var yScale = 1.0f / zoom;

		var xOffset = x - xScale / 2.0f;
		var yOffset = y - yScale / 2.0f;

		// calculate scale and offset
		var scaleOffset = new Vector4( xScale, yScale, xOffset, yOffset );

		m_material.SetVector( "_MainTex_ST", scaleOffset );
		m_material.SetVector( "SF_SpecularMap_ST", scaleOffset );
		m_material.SetVector( "SF_NormalMap_ST", scaleOffset );
		m_material.SetVector( "SF_EmissiveMap_ST", scaleOffset );
		m_material.SetVector( "SF_WaterMaskMap_ST", scaleOffset );

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
		m_material.SetTexture( "_MainTex", planetGenerator.m_albedoTexture );
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

		var elevation = m_planetGenerator.GetBicubicSmoothedElevation( x, z );

		if ( elevation < m_planetGenerator.m_waterElevation )
		{
			elevation = m_planetGenerator.m_waterElevation;
		}

		return new Vector3( vertex.x, elevation * m_elevationScale, vertex.z );
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
