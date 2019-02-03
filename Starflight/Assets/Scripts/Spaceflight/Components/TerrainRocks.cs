
using UnityEngine;

using System.Diagnostics;

public class TerrainRocks : MonoBehaviour
{
	const int c_maxNumRocks = 10000;

	// the rock template parent gameobject
	public GameObject m_rockTemplates;

	// initialize the rocks for a planet
	public void Initialize( PlanetGenerator planetGenerator, float elevationScale )
	{
		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		// get to the current planet we are on
		var planet = planetGenerator.GetPlanet();

		// figure out how many rocks we want to place (based on mineral density of the planet)
		var numRocks = Mathf.RoundToInt( Mathf.Lerp( 0, c_maxNumRocks, planet.m_mineralDensity / 100.0f ) );

		// get the number of template rocks we have to clone from
		var numTemplateRocks = m_rockTemplates.transform.childCount;

		// reset random number generator to a deterministic value (so rocks are always in the same place for each planet)
		Random.InitState( planet.m_id );

		// start placing rocks
		for ( var i = 0; i < numRocks; i++ )
		{
			// grab the rock template for this rock
			var rockTemplate = m_rockTemplates.transform.GetChild( i % numTemplateRocks );

			// we'll figure out what values to use for these in the loop
			var mapX = 0.0f;
			var mapY = 0.0f;

			var elevation = 0.0f;

			// maximum of 10 tries to put this rock somewhere decent
			for ( var j = 0; j < 10; j++ )
			{
				// pick a random place on the planet surface
				mapX = Random.Range( 0.0f, planetGenerator.m_textureMapWidth );
				mapY = Random.Range( planetGenerator.m_textureMapHeight * 0.125f, planetGenerator.m_textureMapHeight * 0.875f );

				// get the surface normal
				var normal = planetGenerator.GetBilinearSmoothedNormal( mapX, mapY, elevationScale * 0.125f );

				// is the surface steep (more that 45 degree slope)?
				if ( normal.y < 0.707f )
				{
					// yes - let's try not to put a rock here
					continue;
				}

				// get the surface elevation
				elevation = planetGenerator.GetBilinearSmoothedElevation( mapX, mapY );

				// get a random minimum elevation
				var minimumElevation = Random.Range( planetGenerator.m_waterElevation, planetGenerator.m_snowElevation );

				// is the rock above the minimum elevation?
				if ( elevation >= minimumElevation )
				{
					// yes - we found a good spot - stop now
					break;
				}
			}

			// get the surface normal
			var terrainNormal = planetGenerator.GetBilinearSmoothedNormal( mapX, mapY, elevationScale * 0.125f );
			
			// give the rock a random rotation and align it with the surface normal
			var rockRotation = Quaternion.FromToRotation( Vector3.up, terrainNormal ) * Quaternion.Euler( -90.0f, Random.Range( 0.0f, 360.0f ), 0.0f );

			// compute the world position of this rock
			var rockPosition = Tools.MapToWorldCoordinates( mapX, mapY, planetGenerator.m_textureMapWidth, planetGenerator.m_textureMapHeight );

			rockPosition.y = elevation * elevationScale;

			// tell unity to clone a rock from the selected rock template with the position and rotation we want it to have
			var clonedRock = Instantiate( rockTemplate, rockPosition, rockRotation, transform );
		}

		// UnityEngine.Debug.Log( "Time to populate rocks: " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}
}
