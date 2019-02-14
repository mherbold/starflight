
using UnityEngine;

public class TerrainRocks : TerrainGridPopulator
{
	// the rock templates game object
	public GameObject[] m_rockTemplates;

	// the maximum number of rocks we can place (based on 100% mineral density)
	public int m_maxNumRocks;

	// populate this planet with rocks
	public void Initialize( PlanetGenerator planetGenerator, float elevationScale, int randomSeed )
	{
		// get to this planet
		var planet = planetGenerator.GetPlanet();

		// calculate the number of rocks to place
		var numRocks = ( planet.m_mineralDensity * m_maxNumRocks ) / 100;

		// place them
		Initialize( elevationScale, m_rockTemplates, numRocks, randomSeed, true, 1.0f, 1.0f );
	}
}
