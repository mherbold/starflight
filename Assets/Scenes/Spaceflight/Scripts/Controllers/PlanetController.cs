
using UnityEngine;

public class PlanetController : MonoBehaviour
{
	// the current planet this controller is controlling
	public Planet m_planet;

	// the current orbit angle
	public float m_orbitAngle;

	// the current rotation angle
	private float m_rotationAngle;

	// set this to the material for this planet model
	private Material m_material;

	// the generated diffuse map
	private Texture2D m_diffuseMap;

	// convenient access to the spaceflight controller
	private SpaceflightController m_spaceflightController;

	// this is called by unity at the start of the level
	public void Start()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();

		// grab the material component
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		m_material = meshRenderer.material;
	}

	// this is called by unity every frame
	public void Update()
	{
		// if this planet is off then don't do anything
		if ( m_planet == null )
		{
			return;
		}

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// calculate number of days per year for each planet based on orbit number - orbit 3 should = 366 days like earth
		int daysPerYear = 122 * ( m_planet.m_orbitPosition + 1 );

		// update the orbit angle
		m_orbitAngle = ( playerData.m_starflight.m_gameTime + 1000.0f + ( m_planet.m_starId * 4 ) ) / daysPerYear;

		// update the rotation angle
		m_rotationAngle = ( playerData.m_starflight.m_gameTime + m_planet.m_orbitPosition );

		// update the planet model
		float distance = 2500.0f * ( m_planet.m_orbitPosition + 2 );
		float angle = m_orbitAngle * 2.0f * Mathf.PI;
		float planeOffset = -transform.localScale.y;
		Vector3 position = new Vector3( -Mathf.Sin( angle ) * distance, planeOffset, Mathf.Cos( angle ) * distance );
		Quaternion rotation = Quaternion.AngleAxis( 120.0f, Vector3.right ) * Quaternion.AngleAxis( m_rotationAngle * 360.0f, Vector3.forward );
		transform.SetPositionAndRotation( position, rotation );
	}

	// change the planet we are controlling
	public void SetPlanet( Planet planet )
	{
		// update the planet
		m_planet = planet;

		// if we are just turning off this planet then stop here
		if ( planet == null )
		{
			// hide the planet model
			gameObject.SetActive( false );
		}
		else
		{
			// make the planet model visible
			gameObject.SetActive( true );

			// generate the texture maps for this planet
			GenerateTextureMaps();

			// scale the planet based on its gravity
			float scale = 100.0f + planet.m_gravity / 5.0f;
			transform.localScale = new Vector3( scale, scale, scale );
		}
	}

	// generates the texture maps for this planet
	private void GenerateTextureMaps()
	{
		PlanetMap planetMap = DataController.m_instance.m_planetData.m_planetMapList[ m_planet.m_id ];

		if ( planetMap.m_map.Length == 0 )
		{
			return; // TODO: temporary - skip this planet because we have no map data for it yet
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
