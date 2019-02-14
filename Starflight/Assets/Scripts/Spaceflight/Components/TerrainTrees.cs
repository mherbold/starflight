
using UnityEngine;

public class TerrainTrees : TerrainGridPopulator
{
	// the tree templates game object
	public GameObject[] m_treeTemplates;

	// the maximum number of trees we can place (based on 100% bio density)
	public int m_maxNumTrees;

	// populate this planet with trees
	public void Initialize( PlanetGenerator planetGenerator, float elevationScale, int randomSeed )
	{
		// get to this planet
		var planet = planetGenerator.GetPlanet();

		// calculate the number of trees to place
		var numTrees = ( planet.m_bioDensity * m_maxNumTrees ) / 100;

		// place them
		Initialize( elevationScale, m_treeTemplates, numTrees, randomSeed, false, 0.67f, 1.0f );
	}
}
