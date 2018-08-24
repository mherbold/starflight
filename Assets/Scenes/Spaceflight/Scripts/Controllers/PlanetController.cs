
using UnityEngine;

public class PlanetController
{
	// public stuff we want to set using the editor
	public Transform m_transform;
	public Material m_material;

	// private stuff we don't want the editor to see
	public int m_planetId;
	public PlanetGameData m_planetGameData;
	public float m_orbitAngle;
	private float m_rotationAngle;
	private Texture2D m_diffuseMap;

	// change the planet we are controlling
	public void Change( int planetId )
	{
		// update the planet id
		m_planetId = planetId;

		// check if we are turning on this planet
		if ( planetId != -1 )
		{
			// get to the game data
			GameData gameData = PersistentController.m_instance.m_gameData;

			// update the current planet
			m_planetGameData = gameData.m_planetList[ planetId ];

			// generate the texture maps for this planet
			GenerateTextureMaps();
		}
	}

	// this is called by unity every frame
	public void Update()
	{
		// if this planet is off then don't do anything
		if ( m_planetId == -1 )
		{
			return;
		}

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// calculate number of days per year for each planet based on orbit number - orbit 3 should = 366 days like earth
		int daysPerYear = 122 * ( m_planetGameData.m_orbitPosition + 1 );

		// update the orbit angle
		m_orbitAngle = ( playerData.m_starflight.m_gameTime + 1000.0f + ( m_planetGameData.m_starId * 4 ) ) / daysPerYear;

		// update the rotation angle
		m_rotationAngle = ( playerData.m_starflight.m_gameTime + m_planetGameData.m_orbitPosition );

		// update the planet model
		Vector3 position = new Vector3( Mathf.Sin( m_orbitAngle ) * 2500.0f, Mathf.Cos( m_orbitAngle ) * 2500.0f, 0.0f );
		Quaternion rotation = Quaternion.AngleAxis( m_rotationAngle * 360.0f, Vector3.up );
		m_transform.SetPositionAndRotation( position, rotation );
	}

	// generates the texture maps for this planet
	private void GenerateTextureMaps()
	{
		PlanetMapData planetMap = PersistentController.m_instance.m_planetData.m_planetMapList[ m_planetId ];

		if ( planetMap.m_map.Length == 0 )
		{
			return; // TODO: temporary - skip this planet because we have no map data for it
		}

		const int newWidth = 1024;
		const int newHeight = 512;

		Texture2D textureMap = new Texture2D( newWidth, newHeight, TextureFormat.RGB24, false );

		const int originalWidth = 48;
		const int originalHeight = 24;

		const float widthRatio = (float) originalWidth / (float) newWidth;
		const float heightRatio = (float) originalHeight / (float) newHeight;

		for ( int y = 0; y < newHeight; y++ )
		{
			float offsetY = (float) y * heightRatio;

			for ( int x = 0; x < newWidth; x++ )
			{
				float offsetX = (float) x * widthRatio;

				int offset = Mathf.FloorToInt( offsetY ) * originalWidth + Mathf.FloorToInt( offsetX );

				int originalColor = planetMap.m_map[ offset ];

				float r = ( ( originalColor >> 16 ) & 0xFF ) / 255.0f;
				float g = ( ( originalColor >> 8 ) & 0xFF ) / 255.0f;
				float b = ( ( originalColor >> 0 ) & 0xFF ) / 255.0f;

				Color color = new Color( r, g, b );

				textureMap.SetPixel( x, newHeight - y - 1, color );
			}
		}

		textureMap.Compress( true );

		textureMap.wrapModeU = TextureWrapMode.Repeat;
		textureMap.wrapModeV = TextureWrapMode.Clamp;

		m_diffuseMap = textureMap;

		m_material.SetTexture( "_MainTex", textureMap );
	}
}
