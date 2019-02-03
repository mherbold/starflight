
using UnityEngine;

public class Disembarked : MonoBehaviour
{
	// the clouds
	public MeshRenderer m_clouds;

	// the terrain grid
	public TerrainGrid m_terrainGrid;

	// the terrain vehicle
	public GameObject m_terrainVehicle;

	// the arth ship
	public GameObject m_arthShip;

	// how high to put the arth ship and the ground scan details
	public float m_arthShipElevationAboveGround;
	public float m_arthShipGroundScanWidth;
	public float m_arthShipGroundScanHeight;
	public float m_arthShipGroundScanInterval;

	// the elements
	public GameObject[] m_elements;

	// the planet generator
	PlanetGenerator m_planetGenerator;

	Disembarked()
	{
	}

	void Update()
	{
		// the terrain grid always follow the terrain vehicle (but snap to integral positions to avoid vertex popping)
		var gridPosition = m_terrainVehicle.transform.localPosition;

		gridPosition.y = 0.0f;

		gridPosition.x = Mathf.FloorToInt( gridPosition.x / 8.0f ) * 8.0f;
		gridPosition.z = Mathf.FloorToInt( gridPosition.z / 8.0f ) * 8.0f;

		m_terrainGrid.transform.localPosition = gridPosition;
	}

	// call this to hide the in orbit objects
	public void Hide()
	{
		if ( !gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the disembarked location." );

		// hide this location
		gameObject.SetActive( false );

		// reset the buttons
		SpaceflightController.m_instance.m_buttonController.RestoreBridgeButtons();

		// forget the planet generator
		m_planetGenerator = null;
	}

	// call this to show this location
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the disembarked location." );

		// show the planetside objects
		gameObject.SetActive( true );

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the star data
		var star = gameData.m_starList[ playerData.m_general.m_currentStarId ];

		// move the player to where we are in this location
		playerData.m_general.m_coordinates = playerData.m_general.m_lastDisembarkedCoordinates;

		// follow the terrain vehicle
		SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( m_terrainVehicle, new Vector3( 0.0f, 75.0f, -75.0f ) );
		//SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( m_terrainVehicle, new Vector3( 0.0f, 20.0f, -20.0f ) );

		// get the planet controller
		var planetController = SpaceflightController.m_instance.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

		Debug.Log( "Showing planet number " + planetController.m_planet.m_id + "." );

		// freeze the player
		SpaceflightController.m_instance.m_playerShip.Freeze();

		// change the button panel label
		SpaceflightController.m_instance.m_buttonController.ChangeOfficerText( "Terrain Vehicle" );

		// change to the terrain vehicle buttons
		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.TerrainVehicle );

		// activate the move button
		SpaceflightController.m_instance.m_buttonController.SetSelectedButton( 1 );
		SpaceflightController.m_instance.m_buttonController.ActivateButton();

		// fade in the map
		SpaceflightController.m_instance.m_viewport.StartFade( 1.0f, 2.0f );

		// play the planetside music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Planetside );

		// set up the clouds and atmosphere
		planetController.SetupClouds( m_clouds, null, true, true );

		// turn skybox autorotate off
		StarflightSkybox.m_instance.m_autorotateSkybox = false;

		// blend completely towards planet atmosphere
		StarflightSkybox.m_instance.m_currentBlendFactor = 1.0f;

		// make sure skybox rotation is reset
		StarflightSkybox.m_instance.m_currentRotation = Quaternion.identity;

		// update the map coordinates
		SpaceflightController.m_instance.m_viewport.UpdateCoordinates();

		// update the messages
		SpaceflightController.m_instance.m_messages.AddText( "<color=white>Activating terrain vehicle.</color>" );
	}

	// this is called when the planet generator is ready
	public void UpdateTerrainGridNow()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the planet controller
		var planetController = SpaceflightController.m_instance.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

		// save the planet generator
		m_planetGenerator = planetController.GetPlanetGenerator();

		// create the elevation texture
		var elevationTexture = m_planetGenerator.CreateElevationTexture();

		m_terrainGrid.SetElevationMap( elevationTexture, m_planetGenerator );

		// place the arth ship at the selected landing coordinates
		var shipPosition = Tools.LatLongToWorldCoordinates( playerData.m_general.m_selectedLatitude, playerData.m_general.m_selectedLongitude );

		m_arthShip.transform.localPosition = shipPosition;

		var minimumElevation = float.MaxValue;
		var chosenAngle = 0;

		for ( var angle = 0; angle < 360; angle += 15 )
		{
			m_arthShip.transform.localRotation = Quaternion.Euler( 0.0f, angle, 0.0f );

			var maximumElevation = float.MinValue;

			for ( var z = m_arthShipGroundScanHeight * -0.5f; z <= m_arthShipGroundScanHeight * 0.5f; z += m_arthShipGroundScanInterval )
			{
				var modifiedScanWidth = m_arthShipGroundScanWidth * ( ( z >= ( m_arthShipGroundScanHeight * -0.075f ) ) ? 0.5f : 1.0f );

				for ( var x = modifiedScanWidth * -0.5f; x <= modifiedScanWidth * 0.5f; x += m_arthShipGroundScanInterval )
				{
					var groundScanPosition = m_arthShip.transform.TransformPoint( new Vector3( x, 0.0f, z ) );

					groundScanPosition = ApplyElevation( groundScanPosition, false );

					if ( groundScanPosition.y > maximumElevation )
					{
						maximumElevation = groundScanPosition.y;
					}
				}
			}

			// Debug.Log( "Angle = " + angle + ", maximum elevation = " + maximumElevation );

			if ( maximumElevation < minimumElevation )
			{
				minimumElevation = maximumElevation;
				chosenAngle = angle;
			}
		}

		// Debug.Log( "Minimum elevation = " + minimumElevation + ", angle chosen = " + chosenAngle );

		shipPosition.y = minimumElevation + m_arthShipElevationAboveGround;

		m_arthShip.transform.localPosition = shipPosition;
		m_arthShip.transform.localRotation = Quaternion.Euler( 0.0f, chosenAngle, 0.0f );
	}

	public Vector3 ApplyElevation( Vector3 worldCoordinates, bool updateWheelEfficiency )
	{
		if ( m_planetGenerator != null )
		{
			Tools.WorldToMapCoordinates( worldCoordinates, out var mapX, out var mapY, m_planetGenerator.m_textureMapWidth, m_planetGenerator.m_textureMapHeight );

			var groundElevation = m_planetGenerator.GetBicubicSmoothedElevation( mapX, mapY ) * m_terrainGrid.m_elevationScale;
			var waterElevation = m_planetGenerator.m_waterElevation * m_terrainGrid.m_elevationScale;

			worldCoordinates.y = Mathf.Max( waterElevation, groundElevation );
		}
		else
		{
			worldCoordinates.y = 0.0f;
		}

		return worldCoordinates;
	}
}
