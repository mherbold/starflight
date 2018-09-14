
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
	// the main camera
	public Camera m_camera;

	// the ship
	public Transform m_ship;

	// the missile launcher
	public GameObject m_missileLauncher;

	// the laser cannon
	public GameObject m_laserCannon;

	// the cargo pods
	public GameObject[] m_cargoPods;

	// the infinite starfield
	public InfiniteStarfield m_infiniteStarfield;

	// all of the different texture maps to use for the skybox
	public Texture[] m_humanSkyboxTextureList;
	public Texture[] m_elowanSkyboxTextureList;
	public Texture[] m_thrynnSkyboxTextureList;
	public Texture[] m_veloxiSkyboxTextureList;
	public Texture[] m_speminSkyboxTextureList;
	public Texture[] m_gazurtoidSkyboxTextureList;
	public Texture[] m_uhlekSkyboxTextureList;

	// the space warp effect
	public SpaceWarp m_spaceWarp;

	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// the maximum speed of the ship
	public float m_maximumShipSpeedHyperspace;
	public float m_maximumShipSpeedStarSystem;

	// the minimum time to reach the maximum speed
	public float m_minimumTimeToReachMaximumShipSpeed;

	// the time to slow down (coast) to a stop
	public float m_timeToStop;

	// keep track of the skybox rotation
	Matrix4x4 m_skyboxRotation = Matrix4x4.identity;

	// keep track of the current skybox blend factor
	float m_skyboxBlendFactor;

	// keep track of what textures we are using for the skybox
	string m_skyboxA;
	string m_skyboxB;

	// set to true when the engines are on (to accelerate) or off (to brake)
	bool m_enginesAreOn;

	// set to true to prevent the player from moving (temporarily)
	bool m_freezePlayer;

	// unity awake
	void Awake()
	{
		// get to the global skybox material
		Material skyboxMaterial = RenderSettings.skybox;

		// replace it with a copy (so when we call settexture it doesn't modify the material on disk)
		skyboxMaterial = new Material( skyboxMaterial );

		// set the copy into the global skybox
		RenderSettings.skybox = skyboxMaterial;
	}

	// unity start
	void Start()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// show only as many cargo pods as we have purchased
		for ( int cargoPodId = 0; cargoPodId < m_cargoPods.Length; cargoPodId++ )
		{
			m_cargoPods[ cargoPodId ].SetActive( cargoPodId < playerData.m_ship.m_numCargoPods );
		}

		// hide or show the missile launchers depending on if we have them
		m_missileLauncher.SetActive( playerData.m_ship.m_missileLauncherClass > 0 );

		// hide or show the missile launchers depending on if we have them
		m_laserCannon.SetActive( playerData.m_ship.m_laserCannonClass > 0 );
	}

	// unity update
	void Update()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the amount of enduruium remaining in storage
		ElementReference elementReference = playerData.m_ship.m_elementStorage.Find( 5 );

		// do this part only if we havent frozen the player (in flux travel)
		if ( !m_freezePlayer )
		{
			// are we out of fuel?
			if ( elementReference == null )
			{
				// yes - turn the engines off
				m_enginesAreOn = false;
			}

			// are the engines turned on?
			if ( m_enginesAreOn )
			{
				// calculate the maximum ship speed (it is the same for all engine classes)
				float maximumShipSpeed = ( playerData.m_starflight.m_location == Starflight.Location.Hyperspace ) ? m_maximumShipSpeedHyperspace : m_maximumShipSpeedStarSystem;

				// calculate the acceleration
				float acceleration = Time.deltaTime * playerData.m_ship.m_acceleration / ( m_minimumTimeToReachMaximumShipSpeed * 25.0f );

				// increase the current speed
				playerData.m_starflight.m_currentSpeed = Mathf.Lerp( playerData.m_starflight.m_currentSpeed, maximumShipSpeed, acceleration );

				// are we in hyperspace?
				if ( playerData.m_starflight.m_location == Starflight.Location.Hyperspace )
				{
					// get the engines
					Engines engines = playerData.m_ship.GetEngines();

					// calculate the amount of fuel used up
					playerData.m_ship.m_fuelUsed += ( playerData.m_starflight.m_currentSpeed * engines.m_fuelUsedPerCoordinate / 256.0f ) * Time.deltaTime;

					// have we used up more than 0.1 units?
					if ( playerData.m_ship.m_fuelUsed >= 0.1f )
					{
						// yes - deduct 0.1 unit from storage
						playerData.m_ship.m_elementStorage.Remove( 5, 1 );

						// recalculate the volume used up in the cargo bays
						playerData.m_ship.RecalculateVolumeUsed();

						// adjust fuel use
						playerData.m_ship.m_fuelUsed -= 0.1f;
					}
				}
			}
			else
			{
				// slow the ship to a stop
				playerData.m_starflight.m_currentSpeed = Mathf.Lerp( playerData.m_starflight.m_currentSpeed, 0.0f, Time.deltaTime / m_timeToStop );
			}

			// check if the ship is moving
			if ( playerData.m_starflight.m_currentSpeed >= 0.1f )
			{
				// calculate the new position of the player
				Vector3 newPosition = transform.position + (Vector3) playerData.m_starflight.m_currentDirection * playerData.m_starflight.m_currentSpeed * Time.deltaTime;

				// make sure the ship stays on the zero plane
				newPosition.y = 0.0f;

				// update the player position
				transform.position = newPosition;

				// update the player data (it will save out to disk eventually)
				if ( playerData.m_starflight.m_location != Starflight.Location.Hyperspace )
				{
					playerData.m_starflight.m_systemCoordinates = newPosition;
				}
				else
				{
					playerData.m_starflight.m_hyperspaceCoordinates = newPosition;
				}

				// set the rotation of the ship
				m_ship.rotation = Quaternion.LookRotation( playerData.m_starflight.m_currentDirection, Vector3.up );

				// figure out how fast to rotate the skybox
				float multiplier = ( playerData.m_starflight.m_location == Starflight.Location.Hyperspace ) ? ( 2.0f / 30.0f ) : ( 1.0f / 30.0f );

				// rotate the skybox
				RotateSkybox( playerData.m_starflight.m_currentDirection, playerData.m_starflight.m_currentSpeed * Time.deltaTime * multiplier );

				// update the map coordinates
				m_spaceflightController.m_spaceflightUI.UpdateCoordinates();
			}
		}

		// get to the global skybox material
		Material skyboxMaterial = RenderSettings.skybox;

		// update the skybox rotation on the material
		skyboxMaterial.SetMatrix( "_ModelMatrix", m_skyboxRotation );

		// get the current hyperspace coordinates (if in hyperspace get it from the player position due to flux travel not updating m_hyperspaceCoordinats)
		Vector3 hyperspaceCoordinates = ( playerData.m_starflight.m_location == Starflight.Location.Hyperspace ) ? transform.position : (Vector3) playerData.m_starflight.m_hyperspaceCoordinates;

		// figure out how far we are from each territory
		foreach ( Territory territory in gameData.m_territoryList )
		{
			territory.m_currentDistance = Vector3.Distance( hyperspaceCoordinates, territory.m_center );
			territory.m_currentDistance -= territory.m_size;
			territory.m_penetrationDistance = Mathf.Max( 0.0f, -territory.m_currentDistance );
			territory.m_currentDistance = Mathf.Max( 0.0f, territory.m_currentDistance );
		}

		// sort the results
		Array.Sort( gameData.m_territoryList );

		// situation A - we are not in any other race's territory
		// situation B - we are in one alien race's territory
		// situation C - we are in two alien race's territories
		if ( gameData.m_territoryList[ 0 ].m_currentDistance > 0.0f )
		{
			// are the skybox A textures currently human?
			if ( m_skyboxA != "human" )
			{
				// no - switch the skybox A texture maps to human
				m_skyboxA = "human";
				SwitchSkyboxTextures( "A", m_skyboxA );
			}

			// set the blend factor to 0 (show full human skybox)
			skyboxMaterial.SetFloat( "_BlendFactor", 0.0f );
		}
		else if ( gameData.m_territoryList[ 1 ].m_currentDistance > 0.0f )
		{
			// are the skybox A textures currently human?
			if ( m_skyboxA != "human" )
			{
				// no - switch the skybox A texture maps to human
				m_skyboxA = "human";
				SwitchSkyboxTextures( "A", m_skyboxA );
			}

			// are the skybox B textures currently the alien territory?
			if ( m_skyboxB != gameData.m_territoryList[ 0 ].m_name )
			{
				// no - switch the skybox B texture maps to that alien race
				m_skyboxB = gameData.m_territoryList[ 0 ].m_name;
				SwitchSkyboxTextures( "B", m_skyboxB );
			}

			// blend factor is simply how much we are penetrating into the alien territory
			float blendFactor = Mathf.Min( 1.0f, gameData.m_territoryList[ 0 ].m_penetrationDistance / 1024.0f );
			skyboxMaterial.SetFloat( "_BlendFactor", blendFactor );
		}
		else
		{
			// are the skybox A textures currently the second alien race?
			if ( m_skyboxA != gameData.m_territoryList[ 1 ].m_name )
			{
				// no - switch the skybox A texture maps to the second alien race
				m_skyboxA = gameData.m_territoryList[ 1 ].m_name;
				SwitchSkyboxTextures( "A", m_skyboxA );
			}

			// are the skybox B textures currently the first alien race?
			if ( m_skyboxB != gameData.m_territoryList[ 0 ].m_name )
			{
				// no - switch the skybox B texture maps to that alien race
				m_skyboxB = gameData.m_territoryList[ 0 ].m_name;
				SwitchSkyboxTextures( "B", m_skyboxB );
			}

			// blend factor is the ratio of penetration distances
			float blendFactorA = Mathf.Min( 1.0f, gameData.m_territoryList[ 1 ].m_penetrationDistance / 1024.0f );
			float blendFactorB = Mathf.Min( 1.0f, gameData.m_territoryList[ 0 ].m_penetrationDistance / 1024.0f );

			float blendFactor = ( blendFactorB * 0.5f ) - ( blendFactorA * 0.5f ) + 0.5f;

			skyboxMaterial.SetFloat( "_BlendFactor", blendFactor );
		}

		// figure out how far we are from each nebula
		foreach ( Nebula nebula in gameData.m_nebulaList )
		{
			nebula.m_currentDistance = Vector3.Distance( hyperspaceCoordinates, nebula.m_center );
			nebula.m_currentDistance -= nebula.m_size;
			nebula.m_penetrationDistance = Mathf.Max( 0.0f, -nebula.m_currentDistance );
			nebula.m_currentDistance = Mathf.Max( 0.0f, nebula.m_currentDistance );
		}

		// sort the results
		Array.Sort( gameData.m_nebulaList );
	}

	// call this to show the player (ship)
	public void Show()
	{
		if ( m_ship.gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the player (ship)." );

		// show the ship
		m_ship.gameObject.SetActive( true );
	}

	// call this to hide the player (ship)
	public void Hide()
	{
		if ( !m_ship.gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the player (ship)." );

		// hide the ship
		m_ship.gameObject.SetActive( false );
	}

	// call this to turn on the engines (accelerate)
	public void TurnOnEngines()
	{
		m_enginesAreOn = true;
	}

	// call this to turn off the engines (brake)
	public void TurnOffEngines()
	{
		m_enginesAreOn = false;
	}

	// call this to change the height of the camera above the zero plane
	public void DollyCamera( float distance )
	{
		// calculate the camera vector
		Vector3 cameraPosition = new Vector3( 0.0f, distance, 0.0f );

		// set it on the camera
		m_camera.transform.localPosition = cameraPosition;
	}

	// call this to get the current position of the player
	public Vector3 GetPosition()
	{
		return transform.position;
	}

	// call this to change when the infinite starfield becomes fully visible
	public void SetStarfieldFullyVisibleSpeed( float newSpeed )
	{
		m_infiniteStarfield.m_fullyVisibleSpeed = newSpeed;
	}

	// call this to temporarily stop the player from moving (e.g. while travelling in flux)
	public void Freeze()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// make sure the player has no momentum to start with
		playerData.m_starflight.m_currentSpeed = 0.0f;

		// freeze the player
		m_freezePlayer = true;
	}

	// call this to allow the player to move again
	public void Unfreeze()
	{
		// unfreeze the player
		m_freezePlayer = false;
	}

	// start the space warp effect
	public void StartSpaceWarp()
	{
		m_spaceWarp.EnterWarp();
	}

	// stop the space warp effect
	public void StopSpaceWarp()
	{
		m_spaceWarp.ExitWarp();
	}

	// call this to rotate the skybox by a certain amount in the given direction
	public void RotateSkybox( Vector3 direction, float amount )
	{
		// compute the rotation quaternion
		Quaternion rotation = Quaternion.LookRotation( direction, Vector3.up );

		// we want to rotate the skybox by the right vector
		Vector3 currentRightVector = rotation * Vector3.right;

		// compute the skybox rotation delta
		rotation = Quaternion.AngleAxis( amount, currentRightVector );

		// compute the new skybox rotation
		m_skyboxRotation = Matrix4x4.Rotate( rotation ) * m_skyboxRotation;

		// update the skybox rotation parameter on the material
		Material skyboxMaterial = RenderSettings.skybox;
		skyboxMaterial.SetMatrix( "_Rotation", m_skyboxRotation );
	}

	// utility to switch a set of skybox textures (which = "A" or "B")
	void SwitchSkyboxTextures( string which, string race )
	{
		Debug.Log( "Switching skybox " + which + " to " + race + "." );

		// figure out which set of textures we want to use
		Texture[] textureList;

		switch ( race )
		{
			case "elowan": textureList = m_elowanSkyboxTextureList; break;
			case "thrynn": textureList = m_thrynnSkyboxTextureList; break;
			case "veloxi": textureList = m_veloxiSkyboxTextureList; break;
			case "spemin": textureList = m_speminSkyboxTextureList; break;
			case "gazurtoid": textureList = m_gazurtoidSkyboxTextureList; break;
			case "uhlek": textureList = m_uhlekSkyboxTextureList; break;

			default:
				textureList = m_humanSkyboxTextureList;
				break;
		}

		// get to the global skybox material
		Material skyboxMaterial = RenderSettings.skybox;

		// switch the textures
		skyboxMaterial.SetTexture( "_FrontTex" + which, textureList[ 0 ] );
		skyboxMaterial.SetTexture( "_BackTex" + which, textureList[ 1 ] );
		skyboxMaterial.SetTexture( "_LeftTex" + which, textureList[ 2 ] );
		skyboxMaterial.SetTexture( "_RightTex" + which, textureList[ 3 ] );
		skyboxMaterial.SetTexture( "_UpTex" + which, textureList[ 4 ] );
		skyboxMaterial.SetTexture( "_DownTex" + which, textureList[ 5 ] );
	}
}
