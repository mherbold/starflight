
using UnityEngine;

public class Disembarked : MonoBehaviour
{
	// the clouds
	public MeshRenderer m_clouds;

	// the terrain grid
	public TerrainGrid m_terrainGrid;

	// the terrain vehicle
	public GameObject m_terrainVehicle;

	// the maximum speed of the player
	public float m_maximumSpeed;

	// the minimum time to reach the maximum speed
	public float m_minimumTimeToReachMaximumSpeed;

	// the time to slow down (coast) to a stop
	public float m_timeToStop;

	// the planet generator
	PlanetGenerator m_planetGenerator;

	// whether or not the tv engine is on or off
	bool m_enginesAreOn;

	// unity update
	void Update()
	{
		// update only if we have a planet generator (we could still be loading)
		if ( m_planetGenerator != null )
		{
			// get to the player data
			var playerData = DataController.m_instance.m_playerData;

			// get the controller stick position
			float x = InputController.m_instance.m_x;
			float z = InputController.m_instance.m_y;

			// create our 3d move vector from the controller position
			Vector3 moveVector = new Vector3( x, 0.0f, z );

			// check if the move vector will actually move the ship (that the controller is not centered)
			if ( moveVector.magnitude > 0.5f )
			{
				// normalize the move vector to a length of 1.0
				moveVector.Normalize();

				// update the direction
				playerData.m_general.m_currentDirection = Vector3.Slerp( playerData.m_general.m_currentDirection, moveVector, Time.deltaTime * 2.0f );

				// turn the engines on
				m_enginesAreOn = true;
			}
			else
			{
				// turn the engines off
				m_enginesAreOn = false;
			}

			// are the engines turned on?
			if ( m_enginesAreOn )
			{
				// calculate the acceleration
				var acceleration = Time.deltaTime * playerData.m_playerShip.m_acceleration / ( m_minimumTimeToReachMaximumSpeed * 25.0f );

				// increase the current speed
				playerData.m_general.m_currentSpeed = Mathf.Lerp( playerData.m_general.m_currentSpeed, m_maximumSpeed, acceleration );
			}
			else
			{
				// slow the ship to a stop
				playerData.m_general.m_currentSpeed = Mathf.Lerp( playerData.m_general.m_currentSpeed, 0.0f, Time.deltaTime / m_timeToStop );
			}

			// check if the ship is moving
			if ( playerData.m_general.m_currentSpeed >= 0.1f )
			{
				// calculate the new position of the player
				var newPosition = m_terrainVehicle.transform.localPosition + (Vector3) playerData.m_general.m_currentDirection * playerData.m_general.m_currentSpeed * Time.deltaTime;

				// update the player position
				m_terrainVehicle.transform.localPosition = newPosition;

				// update the player data (it will save out to disk eventually)
				playerData.m_general.m_coordinates = newPosition;

				// update the last known disembarked coordinates
				playerData.m_general.m_lastDisembarkedCoordinates = playerData.m_general.m_coordinates;
			}

			// set the rotation of the terrain vehicle
			m_terrainVehicle.transform.localRotation = Quaternion.LookRotation( playerData.m_general.m_currentDirection, Vector3.up ) * Quaternion.Euler( -90.0f, 0.0f, 0.0f );

			// get the current position of the terrain vehicle
			var tvPosition = playerData.m_general.m_coordinates;

			// convert from world coordinates to map coordinates
			var mapX = Mathf.FloorToInt( tvPosition.x / 4.0f + 1024.0f );
			var mapY = Mathf.FloorToInt( tvPosition.z / 4.0f + 512.0f );

			// get the height of the terrain at that point
			var elevation = m_planetGenerator.m_elevation[ mapY, mapX ];

			// update the tv position
			tvPosition.y = m_terrainGrid.m_elevationScale * elevation / 2.0f;

			// update the tv game object
			m_terrainVehicle.transform.localPosition = tvPosition;

			// the terrain grid always follow the terrain vehicle
			// TODO - add snapping to vertex points
			var tgPosition = tvPosition;
			tgPosition.y = 0.0f;
			m_terrainGrid.transform.localPosition = tgPosition;
		}
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
		//SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( m_terrainVehicle, new Vector3( 0.0f, -640.0f, 640.0f ), Quaternion.identity, true );
		SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( m_terrainVehicle, new Vector3( 0.0f, 120.0f, -120.0f ), Quaternion.identity, true );

		// get the planet controller
		var planetController = SpaceflightController.m_instance.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

		Debug.Log( "Showing planet number " + planetController.m_planet.m_id + "." );

		// freeze the player
		SpaceflightController.m_instance.m_playerShip.Freeze();

		// reset the buttons
		SpaceflightController.m_instance.m_buttonController.RestoreBridgeButtons();

		// fade in the map
		SpaceflightController.m_instance.m_viewport.StartFade( 1.0f, 2.0f );

		// play the music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.InOrbit );

		// set up the clouds and atmosphere
		planetController.SetupClouds( m_clouds, null, true, true );

		// turn skybox autorotate off
		StarflightSkybox.m_instance.m_autorotateSkybox = false;

		// make sure skybox rotation is reset
		StarflightSkybox.m_instance.m_currentRotation = Quaternion.identity;
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

		m_terrainGrid.SetElevationMap( elevationTexture );
	}
}
