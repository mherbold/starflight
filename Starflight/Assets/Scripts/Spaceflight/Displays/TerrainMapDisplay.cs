
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainMapDisplay : ShipDisplay
{
	// the map
	public Image m_map;

	// the legends
	public Image[] m_legendImages;

	// select site stuff
	public GameObject m_selectSite;

	// select site label
	public GameObject m_selectSiteLabel;

	// the crosshairs
	public GameObject m_horizontalCrosshair;
	public GameObject m_verticalCrosshair;

	// the longitude and latitude text
	public TextMeshProUGUI m_latitudeText;
	public TextMeshProUGUI m_longitudeText;

	// controls moving of the crosshair`
	public float m_maxCrosshairMoveVelocity;
	public float m_crosshairMoveAcceleration;

	// the map material
	Material m_mapMaterial;

	// crosshair move velocity
	float m_crosshairVelocity;

	// keep track of whether we have already initialized
	bool m_initialized;

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

		// initialize
		Initialize();

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the planet controller
		var planetController = SpaceflightController.m_instance.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

		// get to the planet controller's material
		var planetMaterial = planetController.GetMaterial();

		// copy some of the material maps over
		m_mapMaterial.SetTexture( "SF_WaterMaskMap", planetMaterial.GetTexture( "SF_WaterMaskMap" ) );
		m_mapMaterial.SetTexture( "SF_AlbedoMap", planetMaterial.GetTexture( "SF_AlbedoMap" ) );
		m_mapMaterial.SetTexture( "SF_SpecularMap", planetMaterial.GetTexture( "SF_SpecularMap" ) );
		m_mapMaterial.SetTexture( "SF_NormalMap", planetMaterial.GetTexture( "SF_NormalMap" ) );

		// hide all the legends
		foreach ( var legendImage in m_legendImages )
		{
			legendImage.enabled = false;
		}

		// get the surface id from the planet controller
		var surfaceId = planetController.GetSurfaceId();

		// enable the correct legend
		m_legendImages[ surfaceId ].enabled = true;

		// hide the select site stuff
		m_selectSite.SetActive( false );
	}

	// hide
	public override void Hide()
	{
		// call base hide
		base.Hide();
	}

	// initialize
	void Initialize()
	{
		if ( m_initialized )
		{
			return;
		}

		// make an instance of the material so we don't permanently change its settings
		m_mapMaterial = new Material( m_map.material );

		m_map.material = m_mapMaterial;

		// hide the crosshairs
		HideSelectSite();

		// remember that we have already initialized
		m_initialized = true;
	}

	// show the select site stuff
	public void ShowSelectSite()
	{
		m_selectSite.SetActive( true );

		m_selectSiteLabel.SetActive( true );
	}

	// hide the select site stuff
	public void HideSelectSite()
	{
		m_selectSiteLabel.SetActive( false );
	}

	// move the crosshairs
	public void MoveCrosshairs()
	{
		var playerData = DataController.m_instance.m_playerData;

		var crosshairsMoved = false;

		var x = InputController.m_instance.m_x;
		var y = InputController.m_instance.m_y;

		if ( x < -0.5f )
		{
			crosshairsMoved = true;

			playerData.m_general.m_selectedLatitude -= m_crosshairVelocity;

			playerData.m_general.m_selectedLatitude = Mathf.Max( -180.0f, playerData.m_general.m_selectedLatitude );
		}
		else if ( x > 0.5f )
		{
			crosshairsMoved = true;

			playerData.m_general.m_selectedLatitude += m_crosshairVelocity;

			playerData.m_general.m_selectedLatitude = Mathf.Min( 179.0f, playerData.m_general.m_selectedLatitude );
		}

		if ( y < -0.5f )
		{
			crosshairsMoved = true;

			playerData.m_general.m_selectedLongitude -= m_crosshairVelocity;

			playerData.m_general.m_selectedLongitude = Mathf.Max( -90.0f, playerData.m_general.m_selectedLongitude );
		}
		else if ( y > 0.5f )
		{
			crosshairsMoved = true;

			playerData.m_general.m_selectedLongitude += m_crosshairVelocity;

			playerData.m_general.m_selectedLongitude = Mathf.Min( 90.0f, playerData.m_general.m_selectedLongitude );
		}

		if ( crosshairsMoved )
		{
			m_crosshairVelocity += Time.deltaTime * m_crosshairMoveAcceleration;
		}
		else
		{
			m_crosshairVelocity = Time.deltaTime * m_crosshairMoveAcceleration;
		}

		m_latitudeText.text = Mathf.FloorToInt( Mathf.Abs( playerData.m_general.m_selectedLatitude ) ).ToString() + ( ( playerData.m_general.m_selectedLatitude < 0.0f ) ? "W" : "E" );
		m_longitudeText.text = Mathf.RoundToInt( Mathf.Abs( playerData.m_general.m_selectedLongitude ) ).ToString() + ( ( playerData.m_general.m_selectedLongitude < 0.0f ) ? "S" : "N" );

		x = playerData.m_general.m_selectedLatitude / 180.0f * 224.0f;
		y = playerData.m_general.m_selectedLongitude / 90.0f * 112.0f;

		m_verticalCrosshair.transform.localPosition = new Vector3( x - 16.0f, 0.0f, 0.0f );
		m_horizontalCrosshair.transform.localPosition = new Vector3( -16.0f, y, 0.0f );
	}
}
