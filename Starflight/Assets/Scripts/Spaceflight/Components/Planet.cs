
using UnityEngine;

public class Planet : MonoBehaviour
{
	// the current planet this controller owns
	public GD_Planet m_planet;

	// access to the planet model
	public GameObject m_planetModel;

	// access to the planet clouds
	public MeshRenderer m_planetClouds;

	// access to the planet atmosphere
	public GameObject m_planetAtmosphere;

	// access to the starport model
	public GameObject m_starportModel;

	// access to the mesh renderer (to get to the material)
	MeshRenderer m_meshRenderer;

	// set this to the material for this planet model
	Material m_material;

	// whether or not we have finished generating maps for this planet
	bool m_mapsGenerated;

	// the planet generator
	PlanetGenerator m_planetGenerator;

	// the current rotation
	float m_currentRotationAngle;

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
		// update the rotation angle
		m_currentRotationAngle += Time.deltaTime * SpaceflightController.m_instance.m_planetRotationSpeed;

		if ( m_currentRotationAngle >= 360.0f )
		{
			m_currentRotationAngle -= 360.0f;
		}

		// update the rotation of the planet
		m_planetModel.transform.localRotation = Quaternion.AngleAxis( -30.0f, Vector3.right ) * Quaternion.AngleAxis( m_currentRotationAngle, Vector3.forward );

		// update the rotation of the starport
		if ( m_starportModel != null )
		{
			m_starportModel.transform.localRotation = Quaternion.Euler( -90.0f, 0.0f, m_currentRotationAngle * 20.0f );
		}
	}

	// call this before you enable the planet
	public void InitializePlanet( GD_Planet planet )
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if we have a planet
		if ( ( planet == null ) || ( planet.m_id == -1 ) )
		{
			// nope - forget this planet
			m_planet = null;

			// we don't need to generate maps for this planet
			m_mapsGenerated = true;

			// don't do anything more here
			return;
		}

		// yep - remember the planet we are controlling
		m_planet = planet;

		// get the mesh renderer component
		m_meshRenderer = m_planetModel.GetComponent<MeshRenderer>();

		// grab the material from the mesh renderer and make a clone of it
		m_material = m_meshRenderer.material;

		// start the maps generation process
		m_planetGenerator = new PlanetGenerator();

		m_planetGenerator.Start( m_planet );

		m_mapsGenerated = false;

		// set the current position of the planet
		transform.localPosition = m_planet.GetPosition();

		// set the initial rotation angle of the planet
		m_currentRotationAngle = m_planet.m_orbitPosition;
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
		var gameData = DataController.m_instance.m_gameData;

		// show this orbit
		gameObject.SetActive( true );

		// show or hide the starport model depending on whether or not this planet is Arth
		if ( m_starportModel != null )
		{
			m_starportModel.SetActive( m_planet.m_id == gameData.m_misc.m_arthPlanetId );
		}

		// scale the planet based on its mass
		m_planetModel.transform.localScale = m_planet.GetScale();
		m_planetAtmosphere.transform.localScale = m_planetModel.transform.localScale * 1.055f;

		// move the planet to be just below the zero plane (and do the same to the atmosphere)
		var position = Vector3.up * ( -16.0f - m_planetModel.transform.localScale.y );
		m_planetModel.transform.localPosition = position;
		m_planetAtmosphere.transform.localPosition = position * 0.85f;

		// set up the clouds and atmosphere
		SetupClouds( m_planetClouds, m_planetAtmosphere, false, false );
	}

	// call this to get the distance to the player
	public float GetDistanceToPlayer()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// return the distance from the player to the planet
		return Vector3.Distance( playerData.m_general.m_coordinates, transform.localPosition );
	}

	// get the material for this planet
	public Material GetMaterial()
	{
		return m_material;
	}

	// get the surface id of this planet
	public int GetSurfaceId()
	{
		return m_planet.m_surfaceId;
	}

	// return true if all maps have been generated or false if not
	public bool MapsGenerated()
	{
		if ( m_planetGenerator == null )
		{
			return true;
		}

		return m_mapsGenerated;
	}

	// continue generating maps for this planet
	public float GenerateMaps()
	{
		float progress = m_planetGenerator.Process();

		if ( m_planetGenerator.m_abort )
		{
			m_mapsGenerated = true;
		}
		else if ( m_planetGenerator.m_mapsGenerated )
		{
			m_mapsGenerated = true;

			m_material.SetTexture( "_MainTex", m_planetGenerator.m_albedoTexture );
			m_material.SetTexture( "SF_SpecularMap", m_planetGenerator.m_specularTexture );
			m_material.SetTexture( "SF_NormalMap", m_planetGenerator.m_normalTexture );
			m_material.SetTexture( "SF_WaterMaskMap", m_planetGenerator.m_waterMaskTexture );

			m_meshRenderer.material = m_material;

			SpaceflightController.m_instance.m_inOrbit.MaterialUpdated();
		}

		return progress;
	}

	// get the current planet generator
	public PlanetGenerator GetPlanetGenerator()
	{
		return m_planetGenerator;
	}

	// sets up the clouds based on planet properties
	public void SetupClouds( MeshRenderer planetClouds, GameObject planetAtmosphere, bool updateSkybox, bool enableFog )
	{
		// does the planet have an atmosphere?
		if ( m_planet.HasAtmosphere() )
		{
			// yes - show the clouds
			planetClouds.gameObject.SetActive( true );

			// show the atmosphere
			if ( planetAtmosphere != null )
			{
				planetAtmosphere.SetActive( true );
			}

			// get the primary atmosphere of the planet
			var color = m_planet.GetAtmosphereColor();

			// get the atmosphere density
			var atmosphereDensity = m_planet.GetAtmosphereDensity();

			// set the density of the clouds
			planetClouds.material.SetFloat( "SF_Density", 2.0f - atmosphereDensity * 1.25f );

			// pick the color for the clouds
			if ( m_planet.IsMolten() )
			{
				planetClouds.material.SetColor( "SF_AlbedoColor", new Color( 0.01f, 0.01f, 0.01f ) );
				planetClouds.material.SetColor( "SF_SpecularColor", new Color( 0.25f, 0.01f, 0.01f ) );
			}
			else
			{
				planetClouds.material.SetColor( "SF_AlbedoColor", new Color( 1.0f, 1.0f, 1.0f ) );
				planetClouds.material.SetColor( "SF_SpecularColor", color );
			}

			// set the opacity of the clouds based on the density
			Tools.SetOpacity( planetClouds.material, atmosphereDensity * 0.5f + 0.5f );

			// apply the color to the atmosphere
			if ( planetAtmosphere != null )
			{
				planetAtmosphere.GetComponent<MeshRenderer>().material.SetColor( "SF_AlbedoColor", color );
			}

			// update the skybox if needed
			if ( updateSkybox )
			{
				// blend to the the planet skybox
				StarflightSkybox.m_instance.SwitchSkyboxTextures( "B", "planet" );

				// set the color tint
				StarflightSkybox.m_instance.m_colorTintB = color;
			}

			// also do fog settings
			if ( enableFog )
			{
				RenderSettings.fog = true;
				RenderSettings.fogColor = color;
				RenderSettings.fogMode = FogMode.Linear;
				RenderSettings.fogDensity = 1.0f;
				RenderSettings.fogStartDistance = 1000.0f - atmosphereDensity * 950.0f;
				RenderSettings.fogEndDistance = 1000.0f;

				// Debug.Log( "Fog enabled, atmospheric density = " + atmosphereDensity );
			}
		}
		else
		{
			// no - hide the clouds
			planetClouds.gameObject.SetActive( false );

			// hide the atmosphere
			if ( planetAtmosphere != null )
			{
				planetAtmosphere.SetActive( false );
			}

			if ( updateSkybox )
			{
				StarflightSkybox.m_instance.SwitchSkyboxTextures( "B", "planet" );

				StarflightSkybox.m_instance.m_colorTintB = Color.black;
			}

			// we want to turn off the fog
			enableFog = false;
		}

		// turn off the fog if it is not enabled
		if ( !enableFog )
		{
			RenderSettings.fog = false;
			RenderSettings.fogColor = Color.magenta;
			RenderSettings.fogMode = FogMode.Linear;
			RenderSettings.fogDensity = 0.0f;
			RenderSettings.fogStartDistance = 50.0f;
			RenderSettings.fogEndDistance = 1000.0f;
		}
	}
}
