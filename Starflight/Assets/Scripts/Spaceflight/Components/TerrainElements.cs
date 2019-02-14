
using UnityEngine;

public class TerrainElements : TerrainGridPopulator
{
	// the element templates
	public GameObject[] m_elementTemplates;

	// the maximum number of elements we can place (based on 100% mineral density)
	public int m_maxNumElements = 5000;

	// populate this planet with elements
	public void Initialize( PlanetGenerator planetGenerator, float elevationScale, int randomSeed )
	{
		// get to this planet
		var planet = planetGenerator.GetPlanet();

		// calculate the number of elements to place
		var numElements = ( planet.m_mineralDensity * m_maxNumElements ) / 100;

		// build the list of element templates for this planet
		var elementTemplates = new GameObject[ 3 ];

		elementTemplates[ 0 ] = m_elementTemplates[ planet.m_elementIdA ];
		elementTemplates[ 1 ] = m_elementTemplates[ planet.m_elementIdB ];
		elementTemplates[ 2 ] = m_elementTemplates[ planet.m_elementIdC ];

		// place them
		Initialize( elevationScale, elementTemplates, numElements, randomSeed, true, 1.0f, 1.0f );
	}
}
