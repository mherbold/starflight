
using UnityEngine;

public class Disembarked : MonoBehaviour
{
	// the clouds
	public MeshRenderer m_clouds;

	// the terrain grid
	public TerrainGrid m_terrainGrid;

	// the terrain vehicle
	public GameObject m_terrainVehicle;

	// the terrain vehicle wheels (fr, fl, mr, ml, rr, rl)
	public GameObject[] m_wheels;

	// the steering joints (fr, fl, mr, ml, rr, rl)
	public GameObject[] m_steeringJoints;

	// how fast to spin the wheels
	public float m_wheelTurnSpeed;

	// how fast the wheels steer
	public float m_wheelSteerSpeed;

	// the resting suspension height above the ground
	public float m_neutralSuspensionHeight;

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

	// keep track of the last direction (for steering the wheels)
	Vector3 m_lastDirection;

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

				// turn the wheels
				foreach ( var wheel in m_wheels )
				{
					wheel.transform.localRotation *= Quaternion.Euler( 0.0f, 0.0f, playerData.m_general.m_currentSpeed * m_wheelTurnSpeed * Time.deltaTime );
				}
			}

			// set the rotation of the terrain vehicle
			m_terrainVehicle.transform.localRotation = Quaternion.LookRotation( playerData.m_general.m_currentDirection, Vector3.up ) * Quaternion.Euler( -90.0f, 0.0f, 0.0f );

			// get the number of degrees we are turning the terrain vehicle (compared to the last frame)
			var steeringAngle = Vector3.SignedAngle( playerData.m_general.m_currentDirection, m_lastDirection, Vector3.up );

			// scale the angle enough so we actually see the wheels turning (but max it out at 45 degrees in either direction)
			steeringAngle = Mathf.Max( -45.0f, Mathf.Min( 45.0f, steeringAngle * 12.0f ) );

			// steer the wheels
			m_steeringJoints[ 0 ].transform.localRotation = Quaternion.Slerp( m_steeringJoints[ 0 ].transform.localRotation, Quaternion.Euler( 0.0f, -steeringAngle, 0.0f ), Time.deltaTime * m_wheelSteerSpeed );
			m_steeringJoints[ 1 ].transform.localRotation = Quaternion.Slerp( m_steeringJoints[ 1 ].transform.localRotation, Quaternion.Euler( 0.0f, -steeringAngle, 0.0f ), Time.deltaTime * m_wheelSteerSpeed );
			m_steeringJoints[ 4 ].transform.localRotation = Quaternion.Slerp( m_steeringJoints[ 4 ].transform.localRotation, Quaternion.Euler( 0.0f, steeringAngle, 0.0f ), Time.deltaTime * m_wheelSteerSpeed );
			m_steeringJoints[ 5 ].transform.localRotation = Quaternion.Slerp( m_steeringJoints[ 5 ].transform.localRotation, Quaternion.Euler( 0.0f, steeringAngle, 0.0f ), Time.deltaTime * m_wheelSteerSpeed );

			// update the last direction
			m_lastDirection = playerData.m_general.m_currentDirection;

			// get the current position of the terrain vehicle
			var tvPosition = ApplyElevation( playerData.m_general.m_coordinates );

			// update the tv game object position
			m_terrainVehicle.transform.localPosition = tvPosition;

			// reset the steering joints
			foreach ( var steeringJoint in m_steeringJoints )
			{
				steeringJoint.transform.localPosition = Vector3.zero;
			}

			// calculate the normal of the terrain between the center and the front wheels
			var wheel1 = ApplyElevation( m_wheels[ 1 ].transform.position );
			var wheel2 = ApplyElevation( m_wheels[ 0 ].transform.position );

			var side1 = wheel1 - tvPosition;
			var side2 = wheel2 - tvPosition;

			var normal1 = Vector3.Cross( side1, side2 );

			wheel1 = ApplyElevation( m_wheels[ 4 ].transform.position );
			wheel2 = ApplyElevation( m_wheels[ 5 ].transform.position );

			var normal2 = Vector3.Cross( side1, side2 );

			var averagedNormal = Vector3.Normalize( normal1 + normal2 );

			// update the attitude of the body based on the average normal
			m_terrainVehicle.transform.rotation = Quaternion.FromToRotation( Vector3.up, averagedNormal ) * m_terrainVehicle.transform.rotation;

			// move the tv wheels up and down depending on the height of the terrain under the wheels
			foreach ( var steeringJoint in m_steeringJoints )
			{
				var wheelPosition = ApplyElevation( steeringJoint.transform.position );

				var offset = wheelPosition.y - steeringJoint.transform.position.y;

				wheelPosition.x = 0.0f;
				wheelPosition.y = offset + m_neutralSuspensionHeight;
				wheelPosition.z = 0.0f;

				steeringJoint.transform.localPosition = wheelPosition;
			}

			// the terrain grid always follow the terrain vehicle (but snap to integral positions to avoid vertex popping)
			var gridPosition = tvPosition;

			gridPosition.y = 0.0f;

			gridPosition.x = Mathf.FloorToInt( gridPosition.x / 8.0f ) * 8.0f;
			gridPosition.z = Mathf.FloorToInt( gridPosition.z / 8.0f ) * 8.0f;

			m_terrainGrid.transform.localPosition = gridPosition;
		}
	}

	Vector3 ApplyElevation( Vector3 worldCoordinates )
	{
		var x = worldCoordinates.x * 0.25f + m_planetGenerator.m_textureMapWidth * 0.5f - 0.5f;
		var y = worldCoordinates.z * 0.25f + m_planetGenerator.m_textureMapHeight * 0.5f - 0.5f;

		worldCoordinates.y = m_planetGenerator.GetBicubicSmoothedElevation( x, y ) * m_terrainGrid.m_elevationScale / 2.0f;

		return worldCoordinates;
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
		SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( m_terrainVehicle, new Vector3( 0.0f, 75.0f, -75.0f ), Quaternion.identity, true );
		//SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( m_terrainVehicle, new Vector3( 0.0f, 20.0f, -20.0f ), Quaternion.identity, true );

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

		// jump start the last direction
		m_lastDirection = playerData.m_general.m_currentDirection;
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
