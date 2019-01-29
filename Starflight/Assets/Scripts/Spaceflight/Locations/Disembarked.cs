
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

	// the planet generator
	PlanetGenerator m_planetGenerator;

	// also for gizmo drawing
	Vector3[] m_debugPoints;

	Disembarked()
	{
		m_debugPoints = new Vector3[ 10000 ];
	}

	private void Update()
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
	}

	// call this to show this location
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the disembarked location." );

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the star data
		var star = gameData.m_starList[ playerData.m_general.m_currentStarId ];

		// show the planetside objects
		gameObject.SetActive( true );

		// move the player to where we are in this location
		playerData.m_general.m_coordinates = playerData.m_general.m_lastDisembarkedCoordinates;

		// stop the camera animation
		SpaceflightController.m_instance.m_playerCamera.StopAnimation();

		// follow the terrain vehicle
		SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( m_terrainVehicle, new Vector3( 0.0f, 75.0f, -75.0f ), Quaternion.Euler( 45.0f, 0.0f, 0.0f ) );
		//SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( m_terrainVehicle, new Vector3( 0.0f, 20.0f, -20.0f ), Quaternion.Euler( 45.0f, 0.0f, 0.0f ) );

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

		// play the music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.InOrbit );

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

		// place the arth ship where the player landed
		var shipPosition = playerData.m_general.m_lastDisembarkedCoordinates;

		m_arthShip.transform.localPosition = shipPosition;

		var maximumElevation = 0.0f;
		var i = 0;

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

				if ( i < m_debugPoints.Length )
				{
					m_debugPoints[ i++ ] = groundScanPosition;
				}
			}
		}

		shipPosition.y = maximumElevation + m_arthShipElevationAboveGround;

		m_arthShip.transform.localPosition = shipPosition;
	}

	Vector3 ApplyElevation( Vector3 worldCoordinates, bool updateWheelEfficiency )
	{
		var x = worldCoordinates.x * 0.25f + m_planetGenerator.m_textureMapWidth * 0.5f - 0.5f;
		var y = worldCoordinates.z * 0.25f + m_planetGenerator.m_textureMapHeight * 0.5f - 0.5f;

		var groundElevation = m_planetGenerator.GetBicubicSmoothedElevation( x, y ) * m_terrainGrid.m_elevationScale;
		var waterElevation = m_planetGenerator.m_waterHeight * m_terrainGrid.m_elevationScale;

		worldCoordinates.y = Mathf.Max( waterElevation, groundElevation );

		return worldCoordinates;
	}

#if UNITY_EDITOR

	// draw gizmos to help debug the game
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		for ( var i = 0; i < m_debugPoints.Length; i++ )
		{
			if ( m_debugPoints[ i ] != null )
			{
				Gizmos.DrawWireCube( m_debugPoints[ i ], Vector3.one );
			}
		}
	}

#endif
}
