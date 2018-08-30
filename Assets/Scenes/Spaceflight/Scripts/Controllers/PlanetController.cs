
using UnityEngine;

public class PlanetController : MonoBehaviour
{
	// the current planet this controller is controlling
	public Planet m_planet;

	// the current orbit angle
	public float m_orbitAngle;

	// access to the planet model
	public GameObject m_planetModel;

	// access to the starport model
	public GameObject m_starportModel;

	// the current rotation angle
	float m_rotationAngle;

	// access to the mesh renderer (to get to the material)
	MeshRenderer m_meshRenderer;

	// set this to the material for this planet model
	Material m_material;

	// the generated diffuse map
	Texture2D m_diffuseMap;

	// unity awake
	void Awake()
	{
		// mesh renderer component
		m_meshRenderer = m_planetModel.GetComponent<MeshRenderer>();

		// grab the material from the mesh renderer
		m_material = m_meshRenderer.material;
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
		// if this planet is off then don't do anything
		if ( m_planet != null )
		{
			// get to the player data
			PlayerData playerData = DataController.m_instance.m_playerData;

			// calculate number of days per year for each planet based on orbit number - orbit 3 should = 366 days like earth
			int daysPerYear = 122 * ( m_planet.m_orbitPosition + 1 );

			// update the orbit angle
			m_orbitAngle = ( playerData.m_starflight.m_gameTime + 1000.0f + ( m_planet.m_starId * 4 ) ) / daysPerYear;

			// update the rotation angle
			m_rotationAngle = ( playerData.m_starflight.m_gameTime + m_planet.m_orbitPosition );

			Quaternion rotation;

			float distance = Mathf.Lerp( 50.0f, 225.0f, m_planet.m_orbitPosition / 7.0f ) * ( 8192.0f / 256.0f );
			float angle = m_orbitAngle * 2.0f * Mathf.PI;
			float planeOffset;

			// is this the arth starport?
			if ( m_planet.m_planetTypeId == 57 )
			{
				// starport height is about 12 units
				planeOffset = -12.0f;
				rotation = Quaternion.AngleAxis( m_rotationAngle * 360.0f * 20.0f, Vector3.up );
			}
			else
			{
				// update the planet model
				planeOffset = -transform.localScale.y;
				rotation = Quaternion.AngleAxis( 120.0f, Vector3.right ) * Quaternion.AngleAxis( m_rotationAngle * 360.0f, Vector3.forward );
			}

			// update the position and rotation of the controller
			Vector3 position = new Vector3( -Mathf.Sin( angle ) * distance, planeOffset, Mathf.Cos( angle ) * distance );
			transform.SetPositionAndRotation( position, rotation );
		}
	}

	// call this to force this planet to update
	public void ForceUpdate()
	{
		Update();
	}

	// change the planet we are controlling
	public void SetPlanet( Planet planet )
	{
		// update the planet
		m_planet = planet;

		// if we are just turning off this planet then stop here
		if ( planet == null )
		{
			// hide this orbit
			gameObject.SetActive( false );
		}
		else
		{
			// show this orbit
			gameObject.SetActive( true );

			float scale;

			// check if this is arth station
			if ( planet.m_planetTypeId == 57 )
			{
				// yep - show the starport model
				m_starportModel.SetActive( true );

				// hide the planet model
				m_planetModel.SetActive( false );

				// starport scale
				scale = 0.5f;
			}
			else
			{
				// nope - show the planet model
				m_planetModel.SetActive( true );

				// does this orbit have a starport model?
				if ( m_starportModel != null )
				{
					// yep - hide it
					m_starportModel.SetActive( false );
				}

				// scale the planet based on its gravity
				scale = 32.0f + planet.m_gravity / 8.0f;

				// generate the texture maps for this planet
				GenerateTextureMaps();
			}

			// update the scale
			transform.localScale = new Vector3( scale, scale, scale );
		}
	}

	// generates the texture maps for this planet
	void GenerateTextureMaps()
	{
		PlanetMap planetMap = DataController.m_instance.m_planetData.m_planetMapList[ m_planet.m_id ];

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

				int originalColor = 0;

				if ( planetMap.m_map.Length == 0 )
				{
					originalColor = Random.Range( 0x00000000, 0x00FFFFFF );
				}
				else
				{
					originalColor = planetMap.m_map[ offset ];
				}

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
