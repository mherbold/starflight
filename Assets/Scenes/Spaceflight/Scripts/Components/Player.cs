
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
	// the main camera
	public Camera m_camera;

	// the ship
	public Transform m_ship;

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
	SpaceWarp m_spaceWarp;

	// keep track of the skybox rotation
	Matrix4x4 m_skyboxRotation;

	// keep track of the current skybox blend factor
	float m_skyboxBlendFactor;

	// keep track of what textures we are using for the skybox
	string m_skyboxA;
	string m_skyboxB;

	// set to true to prevent the player from moving (temporarily)
	bool m_freezePlayer;

	// convenient access to the spaceflight controller
	SpaceflightController m_spaceflightController;

	// unity awake
	private void Awake()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();

		// get the space warp effect
		m_spaceWarp = m_camera.GetComponent<SpaceWarp>();
	}

	// unity start
	void Start()
	{
		// reset the skybox rotation
		m_skyboxRotation = Matrix4x4.identity;

		// make sure the player can move
		m_freezePlayer = false;
	}

	// unity update
	void Update()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the global skybox material
		Material skyboxMaterial = RenderSettings.skybox;

		// update the skybox rotation on the material
		skyboxMaterial.SetMatrix( "_ModelMatrix", m_spaceflightController.m_player.m_skyboxRotation );

		// figure out how far we are from each territory
		foreach ( Territory territory in gameData.m_territoryList )
		{
			territory.m_currentDistance = Vector3.Distance( playerData.m_starflight.m_hyperspaceCoordinates, territory.m_center );
			territory.m_currentDistance -= (float) territory.m_size;
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
	}

	// call this to hide the player (ship)
	public void Hide()
	{
		m_ship.gameObject.SetActive( false );
	}

	// call this to show the player (ship)
	public void Show()
	{
		m_ship.gameObject.SetActive( true );
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

	// call this to instantly change the position of the player
	public void SetPosition( Vector3 newPosition )
	{
		transform.position = newPosition;
	}

	// call this to instantly change the rotation of the player
	public void SetRotation( Quaternion newRotation )
	{
		m_ship.rotation = newRotation;
	}

	// call this to change when the infinite starfield becomes fully visible
	public void SetStarfieldFullyVisibleSpeed( float newSpeed )
	{
		m_infiniteStarfield.m_fullyVisibleSpeed = newSpeed;
	}

	// call this to find out if the player is currently frozen or not
	public bool IsFrozen()
	{
		return m_freezePlayer;
	}

	// call this to temporarily stop the player from moving (e.g. while travelling in flux)
	public void Freeze()
	{
		m_freezePlayer = true;
	}

	// call this to allow the player to move again
	public void Unfreeze()
	{
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
