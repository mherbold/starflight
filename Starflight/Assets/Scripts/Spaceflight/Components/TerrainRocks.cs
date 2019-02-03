
using UnityEngine;

using System.Diagnostics;

public class TerrainRocks : MonoBehaviour
{
	// the rock template objects
	public GameObject[] m_rockTemplates;

	// our rocks
	GameObject[] m_rocks;

	// also for gizmo drawing
	Vector3[] m_debugPoints;

	// unity start
	void Start()
	{
		m_debugPoints = new Vector3[ 200 ];
	}

	// unity update
	void Update()
	{
	}

	public void Initialize( PlanetGenerator planetGenerator, float elevationScale )
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		Tools.DestroyChildrenOf( gameObject );

		var planet = planetGenerator.GetPlanet();

		var numRocks = planet.m_mineralDensity * 100;

		m_rocks = new GameObject[ numRocks ];

		Random.InitState( planet.m_id );

		for ( var i = 0; i < numRocks; i++ )
		{
			var randomNumber = Random.Range( 0, m_rockTemplates.Length - 1 );

			var rockTemplate = m_rockTemplates[ randomNumber ];

			var mapPosition = Vector3.zero;

			for ( var j = 0; j < 10; j++ )
			{
				mapPosition.x = Random.Range( 0.0f, planetGenerator.m_textureMapWidth );
				mapPosition.z = Random.Range( planetGenerator.m_textureMapHeight * 0.125f, planetGenerator.m_textureMapHeight * 0.875f );

				mapPosition.y = planetGenerator.GetBilinearSmoothedElevation( mapPosition.x, mapPosition.z );

				var minimumElevation = Random.Range( planetGenerator.m_waterElevation, planetGenerator.m_snowElevation );

				if ( mapPosition.y >= minimumElevation )
				{
					break;
				}
			}

			var terrainNormal = planetGenerator.GetBilinearSmoothedNormal( mapPosition.x, mapPosition.z, elevationScale * 0.125f );

			var rotation = Quaternion.FromToRotation( Vector3.up, terrainNormal ) * Quaternion.Euler( -90.0f, Random.Range( 0.0f, 360.0f ), 0.0f );

			var position = mapPosition;

			position.x = ( ( mapPosition.x + 0.5f ) / planetGenerator.m_textureMapWidth - 0.5f ) * 2048.0f * 4.0f;
			position.y = mapPosition.y * elevationScale;
			position.z = ( ( mapPosition.z + 0.5f ) / planetGenerator.m_textureMapHeight - 0.5f ) * 2048.0f * 2.0f;

			var clonedRock = Instantiate( rockTemplate, position, rotation, transform );

			clonedRock.name = "Rock #" + i;

			// UnityEngine.Debug.Log( "Placing rock " + i + " at " + position.x + ", " + position.y + ", " + position.z );

			m_rocks[ i ] = clonedRock;
		}

		UnityEngine.Debug.Log( "Time to populate rocks: " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

#if UNITY_EDITOR

	// draw gizmos to help debug the game
	void OnDrawGizmos()
	{
		if ( m_debugPoints != null )
		{
			Gizmos.color = Color.green;

			for ( var i = 0; i < m_debugPoints.Length; i++ )
			{
				if ( m_debugPoints[ i ] != null )
				{
					Gizmos.DrawCube( m_debugPoints[ i ], Vector3.one * 0.1f );
				}
			}
		}
	}

#endif
}
