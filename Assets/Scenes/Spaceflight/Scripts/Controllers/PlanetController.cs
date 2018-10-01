
using UnityEngine;

public class PlanetController : MonoBehaviour
{
	const int c_textureWidth = 1024;
	const int c_textureHeight = 512;
	const float c_textureScale = (float) PlanetMap.c_width / (float) c_textureWidth;

	// the current planet this controller is controlling
	public Planet m_planet;

	// access to the planet model
	public GameObject m_planetModel;

	// access to the starport model
	public GameObject m_starportModel;

	// access to the mesh renderer (to get to the material)
	MeshRenderer m_meshRenderer;

	// set this to the material for this planet model
	Material m_material;

	// unity awake
	void Awake()
	{
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

			// set the current position of the planet
			transform.localPosition = m_planet.GetPosition();

			// update the rotation angle
			float rotationAngle = ( playerData.m_starflight.m_gameTime + m_planet.m_orbitPosition );

			// update the rotation of the planet
			m_planetModel.transform.localRotation = Quaternion.AngleAxis( -30.0f, Vector3.right ) * Quaternion.AngleAxis( rotationAngle * 360.0f, Vector3.forward );

			// update the rotation of the starport
			if ( m_starportModel != null )
			{
				m_starportModel.transform.localRotation = Quaternion.Euler( -90.0f, 0.0f, rotationAngle * 360.0f * 20.0f );
			}
		}
	}

	// call this before you enable the planet
	public void InitializePlanet( Planet planet )
	{
		// check if we have a planet
		if ( ( planet == null ) || ( planet.m_id == -1 ) )
		{
			// nope - forget this planet
			m_planet = null;

			// don't do anything more here
			return;
		}

		// yep - remember the planet we are controlling
		m_planet = planet;

		// get the mesh renderer component
		m_meshRenderer = m_planetModel.GetComponent<MeshRenderer>();

		// grab the material from the mesh renderer and make a clone of it
		m_material = new Material( m_meshRenderer.material );

		// set the cloned material on the mesh renderer
		m_meshRenderer.material = m_material;

		// get to the original SF1 planet map data
		//PlanetMap planetMap = DataController.m_instance.m_planetData.m_planetMapList[ m_planet.m_id ];

		//CreateOriginalMap( planetMap );

		//CreateAlbedoMap( planetMap );
		//CreateHeightMap( planetMap );
	}

	// disable a planet
	public void DisablePlanet()
	{
		// disable the game object
		gameObject.SetActive( false );
	}

	// change the planet we are controlling
	public void EnablePlanet()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// show this orbit
		gameObject.SetActive( true );

		// show or hide the starport model depending on whether or not this planet is Arth
		if ( m_starportModel != null )
		{
			m_starportModel.SetActive( m_planet.m_id == gameData.m_misc.m_arthPlanetId );
		}

		// scale the planet based on its mass
		m_planetModel.transform.localScale = m_planet.GetScale();

		// move the planet to be just below the zero plane
		m_planetModel.transform.localPosition = new Vector3( 0.0f, -16.0f - m_planetModel.transform.localScale.y, 0.0f );
	}

	// call this to get the distance to the player
	public float GetDistanceToPlayer()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// return the distance from the player to the planet
		return Vector3.Distance( playerData.m_starflight.m_systemCoordinates, transform.localPosition );
	}

	// get the material for this planet
	public Material GetMaterial()
	{
		return m_material;
	}
	/*

	// call this to create a height map
	void CreateHeightMap( PlanetMap planetMap )
	{
		float radius = 1.0f / c_textureScale;
		float damping = 0.5f / 8.0f;

		Texture2D textureMap = new Texture2D( c_textureWidth, c_textureHeight, TextureFormat.RGB24, false );

		for ( int y = 0; y < c_textureHeight; y++ )
		{
			for ( int x = 0; x < c_textureWidth; x++ )
			{
				float height = planetMap.GetFilteredHeightAt( x, y, radius, damping, c_textureScale );

				textureMap.SetPixel( x, c_textureHeight - y - 1, new Color( height, height, height ) );
			}
		}

		// compress the texture map
		textureMap.Compress( true );

		// set up uv wrapping modes
		textureMap.wrapModeU = TextureWrapMode.Repeat;
		textureMap.wrapModeV = TextureWrapMode.Clamp;

		// apply the texture map to the material
		m_material.SetTexture( "_ParallaxMap", textureMap );
	}
	*/
}
