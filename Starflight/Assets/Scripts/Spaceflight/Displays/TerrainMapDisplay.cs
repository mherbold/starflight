
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainMapDisplay : ShipDisplay
{
	// the planet
	public Image m_image;

	// unity start
	public override void Start()
	{
	}

	// unity update
	public override void Update()
	{
	}

	// the display label
	public override string GetLabel()
	{
		return "Terrain Map";
	}

	// show
	public override void Show()
	{
		// call base show
		base.Show();

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the planet controller
		var planetController = m_spaceflightController.m_starSystem.GetPlanetController( playerData.m_starflight.m_currentPlanetId );

		// apply the material to the planet model
		m_image.material = new Material( planetController.GetMaterial() )
		{
			shader = Shader.Find( "Custom/PlanetSprite" )
		};
	}

	// hide
	public override void Hide()
	{
		// call base hide
		base.Hide();
	}
}
