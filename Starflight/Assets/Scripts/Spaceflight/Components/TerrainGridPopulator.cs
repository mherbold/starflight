
using UnityEngine;

using System.Collections.Generic;

public class TerrainGridPopulator : MonoBehaviour
{
	const float c_minDistanceBetweenObjects = 8.0f;

	const int c_spawnListWidth = 32;
	const int c_spawnListHeight = 16;

	static PlanetGenerator m_planetGenerator;

	static List<Vector3>[,] m_spawnLists;

	public static void ResetSpawnLists( PlanetGenerator planetGenerator )
	{
		// remember the planet generator
		m_planetGenerator = planetGenerator;

		// create the spawn list (stupid unity physics wont work here because they dont update colliders until fixedupate and there is no way to force an update)
		m_spawnLists = new List<Vector3>[ c_spawnListHeight, c_spawnListWidth ];

		for ( var y = 0; y < c_spawnListHeight; y++ )
		{
			for ( var x = 0; x < c_spawnListWidth; x++ )
			{
				m_spawnLists[ y, x ] = new List<Vector3>();
			}
		}
	}

	// populate the planet
	protected void Initialize( float elevationScale, GameObject[] templates, int numObjects, int randomSeed, bool favorHigherElevations, float minScale, float maxScale )
	{
		Debug.Log( "Placing " + numObjects + " objects under " + transform.name + " with using a random seed of " + randomSeed + "..." );

		// remove all of the current objects
		Tools.DestroyChildrenOf( gameObject );

		// if we don't want to place anything then stop here now
		if ( numObjects == 0 )
		{
			return;
		}

		// reset random number generator to a deterministic value (so objects are always in the same place for each planet)
		Random.InitState( randomSeed );

		// get the number of template objects we have to clone from
		var numTemplateObjects = templates.Length;

		// calculate the minimum elevation we'd like to place objects at
		var minimumElevation = ( m_planetGenerator.m_maximumElevation - m_planetGenerator.m_waterElevation ) * 0.05f + m_planetGenerator.m_waterElevation;

		// we'll keep track of the surface normal and position for this object
		var normal = Vector3.zero;
		var position = Vector3.zero;

		// start placing objects
		for ( var i = 0; i < numObjects; i++ )
		{
			// grab the template for the next object
			var template = templates[ i % numTemplateObjects ];

			// we'll figure out what values to use for these in the loop
			var mapX = 0.0f;
			var mapY = 0.0f;

			var elevation = 0.0f;

			// maximum of 10 tries to put this object somewhere decent
			var spotFound = false;

			for ( var j = 0; j < 10; j++ )
			{
				// pick a random place on the planet surface
				mapX = Random.Range( 0.0f, m_planetGenerator.m_textureMapWidth );
				mapY = Random.Range( m_planetGenerator.m_textureMapHeight * 0.125f, m_planetGenerator.m_textureMapHeight * 0.875f );

				// Debug.Log( "Object #" + ( i + 1 ) + ", trying mapX=" + mapX + ", mapY=" + mapY + "..." );

				// get the surface normal
				normal = m_planetGenerator.GetBilinearSmoothedNormal( mapX, mapY, elevationScale * 0.125f );

				// is the surface steep (more that 45 degree slope)?
				if ( normal.y < 0.707f )
				{
					// Debug.Log( "...too steep!" );

					// yes - let's not put it here
					continue;
				}

				// get the elevation
				elevation = m_planetGenerator.GetBilinearSmoothedElevation( mapX, mapY );

				// is the elevation is below the minimum elevation
				if ( elevation < minimumElevation )
				{
					// Debug.Log( "...below minimum elevation!" );

					// yes - let's not put it here
					continue;
				}

				// pick a random elevation somewhere between the minimum and the maximum elevation
				var randomElevation = Random.Range( minimumElevation, m_planetGenerator.m_maximumElevation );

				// do we want to favor the higher elevations?
				if ( favorHigherElevations )
				{
					// yes - are we below the random elevation?
					if ( elevation <= randomElevation )
					{
						// Debug.Log( "...below random elevation!" );

						// yes - let's not put it here
						continue;
					}
				}
				else
				{
					// no - are we above the random elevation?
					if ( elevation >= randomElevation )
					{
						// Debug.Log( "...above random elevation!" );

						// yes - let's not put it here
						continue;
					}
				}

				// calculate the position of this object in world coordinates
				position = Tools.MapToWorldCoordinates( mapX, mapY, m_planetGenerator.m_textureMapWidth, m_planetGenerator.m_textureMapHeight );

				position.y = elevation * elevationScale;

				// check if this object is too close to another object we've already placed

				if ( OverlapsSomething( position ) )
				{
					// Debug.Log( "...overlaps another object!" );

					// nope - let's not put it here
					continue;
				}

				// yay! let's put it here
				spotFound = true;
				break;
			}

			// did we find a good spot to put this object?
			if ( !spotFound )
			{
				// no - ok let's skip this one
				continue;
			}

			// give the object a random rotation and align it with the surface normal
			var rotation = Quaternion.FromToRotation( Vector3.up, normal ) * Quaternion.Euler( 0.0f, Random.Range( 0.0f, 360.0f ), 0.0f ) * template.transform.localRotation;

			// tell unity to clone an object from the selected template with the position and rotation we want it to have
			var clonedObject = Instantiate( template, position, rotation, transform );

			clonedObject.name = "Clone #" + ( i + 1 ) + " (from " + template.name + ")";

			// give the cloned object a random scale
			clonedObject.transform.localScale = Vector3.one * Random.Range( minScale, maxScale );

			// add object to the spawn list
			AddToSpawnList( position );
		}
	}

	static void AddToSpawnList( Vector3 position )
	{
		Tools.WorldToMapCoordinates( position, out var mapX, out var mapY, m_planetGenerator.m_textureMapWidth, m_planetGenerator.m_textureMapHeight );

		var listX = Mathf.FloorToInt( (float) mapX / (float) m_planetGenerator.m_textureMapWidth * (float) c_spawnListWidth );
		var listY = Mathf.FloorToInt( (float) mapY / (float) m_planetGenerator.m_textureMapHeight * (float) c_spawnListHeight );

		m_spawnLists[ listY, listX ].Add( position );
	}

	static bool OverlapsSomething( Vector3 position )
	{
		Tools.WorldToMapCoordinates( position, out var mapX, out var mapY, m_planetGenerator.m_textureMapWidth, m_planetGenerator.m_textureMapHeight );

		var listX = Mathf.FloorToInt( (float) mapX / (float) m_planetGenerator.m_textureMapWidth * (float) c_spawnListWidth );
		var listY = Mathf.FloorToInt( (float) mapY / (float) m_planetGenerator.m_textureMapHeight * (float) c_spawnListHeight );

		for ( var y = -1; y <= 1; y++ )
		{
			var listYOffset = listY + y;

			if ( ( listY < 0 ) || ( listY >= c_spawnListHeight ) )
			{
				continue;
			}

			for ( var x = -1; x <= 1; x++ )
			{
				var listXOffset = ( listX + x + c_spawnListWidth ) & ( c_spawnListWidth - 1 );

				foreach ( var otherPosition in m_spawnLists[ listYOffset, listXOffset ] )
				{
					var distance = Vector3.Magnitude( otherPosition - position );

					if ( distance < c_minDistanceBetweenObjects )
					{
						return true;
					}
				}
			}
		}

		return false;
	}
}
