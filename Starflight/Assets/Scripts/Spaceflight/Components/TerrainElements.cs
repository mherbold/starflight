
using UnityEngine;

public class TerrainElements : MonoBehaviour
{
	const int c_maxNumElements = 5000;

	// the element templates
	public GameObject[] m_elementTemplates;

	// initialize the elements for a planet
	public void Initialize( PlanetGenerator planetGenerator, float elevationScale )
	{
		// delete all of the existing elements
		Tools.DestroyChildrenOf( gameObject );

		// get to the current planet we are on
		var planet = planetGenerator.GetPlanet();

		// build our list of element templates for this planet
		var elementTemplates = new GameObject[ 3 ];

		elementTemplates[ 0 ] = m_elementTemplates[ planet.m_elementIdA ];
		elementTemplates[ 1 ] = m_elementTemplates[ planet.m_elementIdB ];
		elementTemplates[ 2 ] = m_elementTemplates[ planet.m_elementIdC ];

		// figure out how many elements we want to place (based on mineral density of the planet)
		var numElements = Mathf.RoundToInt( Mathf.Lerp( 0, c_maxNumElements, planet.m_mineralDensity / 100.0f ) );

		// get the number of template elements we have to clone from
		var numTemplateElements = elementTemplates.Length;

		// reset random number generator to a deterministic value (so elements are always in the same place for each planet)
		Random.InitState( planet.m_id + 1 );

		// start placing elements
		for ( var i = 0; i < numElements; i++ )
		{
			// grab the element template for this element
			var elementTemplate = elementTemplates[ i % numTemplateElements ];
			// var elementTemplate = m_elementTemplates[ i % m_elementTemplates.Length ];

			// we'll figure out what values to use for these in the loop
			var mapX = 0.0f;
			var mapY = 0.0f;

			var elevation = 0.0f;

			// maximum of 10 tries to put this element somewhere decent
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
					// yes - let's try not to put a element here
					continue;
				}

				// get the surface elevation
				elevation = planetGenerator.GetBilinearSmoothedElevation( mapX, mapY );

				// get a random minimum elevation
				var minimumElevation = Random.Range( planetGenerator.m_waterElevation, planetGenerator.m_snowElevation );

				// is this spot above the minimum elevation?
				if ( elevation >= minimumElevation )
				{
					// yes - is there another object taking up this space?
					var position = Tools.MapToWorldCoordinates( mapX, mapY, planetGenerator.m_textureMapWidth, planetGenerator.m_textureMapHeight );

					position.y = elevation * elevationScale;

					var acceptThisSpot = true;

					var colliders = Physics.OverlapSphere( position, 15.0f );

					foreach ( var collider in colliders )
					{
						// yes - is it the terrain vehicle?
						if ( collider.gameObject.tag != "Terrain Vehicle" )
						{
							// nope - let's not put it here
							acceptThisSpot = false;
							break;
						}
					}

					if ( acceptThisSpot )
					{
						break;
					}
				}
			}

			// get the surface normal
			var terrainNormal = planetGenerator.GetBilinearSmoothedNormal( mapX, mapY, elevationScale * 0.125f );

			// give the element a random rotation and align it with the surface normal
			var elementRotation = Quaternion.FromToRotation( Vector3.up, terrainNormal ) * Quaternion.Euler( -90.0f, Random.Range( 0.0f, 360.0f ), 0.0f );

			// compute the world position of this element
			var elementPosition = Tools.MapToWorldCoordinates( mapX, mapY, planetGenerator.m_textureMapWidth, planetGenerator.m_textureMapHeight );

			elementPosition.y = elevation * elevationScale;

			// tell unity to clone a element from the selected element template with the position and rotation we want it to have
			var clonedElement = Instantiate( elementTemplate, elementPosition, elementRotation, transform );

			clonedElement.name = "Element #" + ( i + 1 ) + " (from " + elementTemplate.name + ")";
		}
	}
}
