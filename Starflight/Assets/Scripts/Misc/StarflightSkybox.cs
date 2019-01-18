
using UnityEngine;

using System;

public class StarflightSkybox : MonoBehaviour
{
	// enable this to automatically blend between skyboxes based on territory
	public bool m_autoblendSkyboxes;

	// enable this to automatically rotate the skybox based on the player position
	public bool m_autorotateSkybox;

	// the game object to track for autorotation
	public GameObject m_playerObject;

	// constant rotation angles
	public Vector3 m_constantRotation;

	// all of the different texture maps to use for the skybox
	public Texture[] m_humanTextureList;
	public Texture[] m_elowanTextureList;
	public Texture[] m_thrynnTextureList;
	public Texture[] m_veloxiTextureList;
	public Texture[] m_speminTextureList;
	public Texture[] m_gazurtoidTextureList;
	public Texture[] m_uhlekTextureList;

	// the skybox material
	public Material m_material;

	// keep track of the current skybox blend factor
	public float m_currentBlendFactor;

	// keep track of what textures we are using for the skybox
	public string m_currentSkyboxA;
	public string m_currentSkyboxB;

	// keep track of the skybox rotation
	Quaternion m_currentRotation = Quaternion.identity;

	// unity awake
	void Awake()
	{
		// replace it with a copy (so when we call settexture it doesn't modify the material on disk)
		m_material = new Material( RenderSettings.skybox );

		// set the copy into the global skybox
		RenderSettings.skybox = m_material;
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if skybox autorotation is enabled
		if ( m_autorotateSkybox )
		{
			// yes - figure out how fast to rotate the skybox
			var multiplier = ( playerData.m_general.m_location != PD_General.Location.Encounter ) ? 8.0f : 0.5f;

			// calculate the amount to rotate the skybox by
			var amount = playerData.m_general.m_currentSpeed / playerData.m_general.m_currentMaximumSpeed * Time.deltaTime * multiplier;

			// compute the rotation quaternion
			var deltaRotation = Quaternion.LookRotation( playerData.m_general.m_currentDirection, Vector3.up );

			// we want to rotate the skybox by the right vector
			var currentRightVector = deltaRotation * Vector3.right;

			// compute the skybox rotation delta
			deltaRotation = Quaternion.AngleAxis( amount, currentRightVector );

			// compute the new skybox rotation
			m_currentRotation = deltaRotation * m_currentRotation;
		}

		// apply constant rotation
		m_currentRotation = Quaternion.Euler( m_constantRotation * Time.deltaTime ) * m_currentRotation;

		// check if skybox autoblending is enabled
		if ( m_autoblendSkyboxes )
		{
			// yes - get the current hyperspace coordinates (if in hyperspace get it from the player transform due to flux travel not updating m_hyperspaceCoordinates)
			var hyperspaceCoordinates = ( playerData.m_general.m_location == PD_General.Location.Hyperspace ) ? m_playerObject.transform.position : playerData.m_general.m_lastHyperspaceCoordinates;

			// figure out how far we are from each territory
			foreach ( var territory in gameData.m_territoryList )
			{
				territory.Update( hyperspaceCoordinates );
			}

			// sort the results
			Array.Sort( gameData.m_territoryList );

			// situation A - we are not in any other race's territory
			// situation B - we are in one alien race's territory
			// situation C - we are in two alien race's territories
			if ( gameData.m_territoryList[ 0 ].GetCurrentDistance() > 0.0f )
			{
				// switch the skybox A texture maps to human
				SwitchSkyboxTextures( "A", "human" );

				// set the blend factor to 0 (show full human skybox)
				m_currentBlendFactor = 0.0f;
			}
			else if ( gameData.m_territoryList[ 1 ].GetCurrentDistance() > 0.0f )
			{
				// switch the skybox A texture maps to human
				SwitchSkyboxTextures( "A", "human" );

				// switch the skybox B texture maps to that alien race
				SwitchSkyboxTextures( "B", gameData.m_territoryList[ 0 ].m_name );

				// the blend factor is simply how much we are penetrating into the alien territory
				m_currentBlendFactor = Mathf.Min( 1.0f, gameData.m_territoryList[ 0 ].GetPenetrationDistance() / 1024.0f );
			}
			else
			{
				// switch the skybox A texture maps to the first alien race
				SwitchSkyboxTextures( "A", gameData.m_territoryList[ 0 ].m_name );

				// switch the skybox B texture maps to that second alien race
				SwitchSkyboxTextures( "B", gameData.m_territoryList[ 1 ].m_name );

				// blend factor is the ratio of penetration distances
				var blendFactorA = Mathf.Min( 1.0f, gameData.m_territoryList[ 0 ].GetPenetrationDistance() / 1024.0f );
				var blendFactorB = Mathf.Min( 1.0f, gameData.m_territoryList[ 1 ].GetPenetrationDistance() / 1024.0f );

				m_currentBlendFactor = ( blendFactorB * 0.5f ) - ( blendFactorA * 0.5f ) + 0.5f;
			}
		}

		// update the skybox rotation on the material
		m_material.SetMatrix( "SF_ModelMatrix", Matrix4x4.Rotate( m_currentRotation ) );

		// update the material with the new blend factor
		m_material.SetFloat( "SF_BlendFactor", m_currentBlendFactor );
	}

	// utility to switch a set of skybox textures (which = "A" or "B")
	void SwitchSkyboxTextures( string which, string race )
	{
		// which skybox texture set?
		if ( which == "A" )
		{
			// are we already using the desired texture set for this skybox?
			if ( m_currentSkyboxA == race )
			{
				// yes - do nothing
				return;
			}

			// remember what we switched to
			m_currentSkyboxA = race;
		}
		else
		{
			// are we already using the desired texture set for this skybox?
			if ( m_currentSkyboxB == race )
			{
				// yes - do nothing
				return;
			}

			// remember what we switched to
			m_currentSkyboxB = race;
		}

		// get the set of textures we want to use now
		Texture[] textureList;

		switch ( race )
		{
			case "elowan": textureList = m_elowanTextureList; break;
			case "thrynn": textureList = m_thrynnTextureList; break;
			case "veloxi": textureList = m_veloxiTextureList; break;
			case "spemin": textureList = m_speminTextureList; break;
			case "gazurtoid": textureList = m_gazurtoidTextureList; break;
			case "uhlek": textureList = m_uhlekTextureList; break;

			default:
				textureList = m_humanTextureList;
				break;
		}

		// switch the textures
		m_material.SetTexture( "_FrontTex" + which, textureList[ 0 ] );
		m_material.SetTexture( "_BackTex" + which, textureList[ 1 ] );
		m_material.SetTexture( "_LeftTex" + which, textureList[ 2 ] );
		m_material.SetTexture( "_RightTex" + which, textureList[ 3 ] );
		m_material.SetTexture( "_UpTex" + which, textureList[ 4 ] );
		m_material.SetTexture( "_DownTex" + which, textureList[ 5 ] );
	}
}
