
using UnityEngine;
using UnityEngine.UI;

public class TerrainMapDisplay : ShipDisplay
{
	// the map
	public MeshRenderer m_map;

	// the legends
	public Image[] m_legendImages;

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
		var planetController = SpaceflightController.m_instance.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

		// switch to the planet controller's material
		m_map.material = planetController.GetMaterial();

		// hide all the legends
		foreach ( var legendImage in m_legendImages )
		{
			legendImage.enabled = false;
		}
		
		// get the surface id from the planet controller
		var surfaceId = planetController.GetSurfaceId();

		// enable the correct legend
		m_legendImages[ surfaceId ].enabled = true;
	}

	// hide
	public override void Hide()
	{
		// call base hide
		base.Hide();
	}
}
