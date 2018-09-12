
using UnityEngine;

public class PlanetController : MonoBehaviour
{
	public const int c_arthPlanetTypeId = 57;

	// the current planet this controller is controlling
	public Planet m_planet;

	// the current orbit angle
	public float m_orbitAngle;

	// access to the planet model
	public GameObject m_planetModel;

	// access to the starport model
	public GameObject m_starportModel;

	// the current scale
	public float m_scale;

	// the current rotation angle
	public float m_rotationAngle;

	// access to the mesh renderer (to get to the material)
	MeshRenderer m_meshRenderer;

	// set this to the material for this planet model
	Material m_material;

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

			float distance = Mathf.Lerp( 50.0f, 225.0f, m_planet.m_orbitPosition / 7.0f ) * ( 8192.0f / 256.0f );
			float angle = m_orbitAngle * 2.0f * Mathf.PI;

			// calculate the new position of the container
			transform.localPosition = new Vector3( -Mathf.Sin( angle ) * distance, 0.0f, Mathf.Cos( angle ) * distance );

			// update the rotation of the planet
			m_planetModel.transform.localRotation = Quaternion.AngleAxis( -30.0f, Vector3.right ) * Quaternion.AngleAxis( m_rotationAngle * 360.0f, Vector3.forward );

			// update the rotation of the starport
			if ( m_starportModel != null )
			{
				m_starportModel.transform.localRotation = Quaternion.Euler( -90.0f, 0.0f, m_rotationAngle * 360.0f * 20.0f );
			}
		}
	}

	// call this to get the distance to the player
	public float GetDistanceToPlayer()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// return the distance from the player to the planet
		return Vector3.Distance( playerData.m_starflight.m_systemCoordinates, transform.localPosition );
	}

	// call this to force this planet to update
	public void ForceUpdate()
	{
		Update();
	}

	// change the planet we are controlling
	public void SetPlanet( Planet planet )
	{
		// change the planet we are controlling
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

			// show or hide the starport model depending on whether or not this planet is Arth
			if ( m_starportModel != null )
			{
				m_starportModel.SetActive( ( planet.m_planetTypeId == c_arthPlanetTypeId ) );
			}

			// scale the planet based on its mass
			m_scale = Mathf.Lerp( 32.0f, 320.0f, Mathf.Sqrt( ( planet.m_mass - 6.0f ) / 500000.0f ) );
			m_planetModel.transform.localScale = new Vector3( m_scale, m_scale, m_scale );
			Debug.Log( "Planet " + planet.m_id + " mass is " + planet.m_mass + " so scale is " + m_scale );

			// move the planet to be just below the zero plane
			m_planetModel.transform.localPosition = new Vector3( 0.0f, -16.0f - m_scale, 0.0f );

			// generate the texture maps for this planet
			GenerateTextureMaps();
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

		m_material.SetTexture( "_MainTex", textureMap );
	}
}
