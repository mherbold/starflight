
using UnityEngine;
using UnityEngine.UI;

public class TerrainMapDisplay : ShipDisplay
{
	// the map
	public MeshRenderer m_map;

	// the legend
	public Image m_legendImage;

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
		var planetController = m_spaceflightController.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

		// switch to the planet controller's material
		m_map.material = planetController.GetMaterial();

		// create a new material for the legend
		var legendTexture = planetController.GetLegendTexture();

		var sprite = Sprite.Create( legendTexture, new Rect( 0.0f, 0.0f, legendTexture.width, legendTexture.height ), new Vector2( 0.5f, 0.5f ) );

		m_legendImage.sprite = sprite;
	}

	// hide
	public override void Hide()
	{
		// call base hide
		base.Hide();
	}
}
