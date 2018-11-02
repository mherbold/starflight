
using UnityEngine;

public class Planet : MonoBehaviour
{
	// various constants that control the planet generator
	const int c_numPolePaddingRows = 3;

	// the current planet this controller is controlling
	public GD_Planet m_planet;

	// access to the planet model
	public GameObject m_planetModel;

	// access to the planet clouds
	public MeshRenderer m_planetClouds;

	// access to the starport model
	public GameObject m_starportModel;

	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// access to the mesh renderer (to get to the material)
	MeshRenderer m_meshRenderer;

	// set this to the material for this planet model
	Material m_material;

	// whether or not we have finished generating maps for this planet
	bool m_mapsGenerated;

	// the planet generator
	PlanetGenerator m_planetGenerator;

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
			var playerData = DataController.m_instance.m_playerData;

			// set the current position of the planet
			transform.localPosition = m_planet.GetPosition();

			// update the rotation angle
			float rotationAngle = ( playerData.m_general.m_gameTime + m_planet.m_orbitPosition );

			// update the rotation of the planet
			m_planetModel.transform.localRotation = Quaternion.AngleAxis( -30.0f, Vector3.right ) * Quaternion.AngleAxis( rotationAngle * 360.0f, Vector3.forward );

			// update the rotation of the starport
			if ( m_starportModel != null )
			{
				m_starportModel.transform.localRotation = Quaternion.Euler( -90.0f, 0.0f, rotationAngle * 360.0f * 20.0f );
			}

			// set the position of the sun based on location
			Vector4 sunPosition = new Vector4( 0.0f, 2048.0f, 0.0f, 0.0f );

			m_material.SetVector( "_SunPosition", sunPosition );

			m_planetClouds.material.SetVector( "_SunPosition", sunPosition );
		}
	}

	// call this before you enable the planet
	public void InitializePlanet( GD_Planet planet )
	{
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
		m_planetClouds.transform.localScale = m_planetModel.transform.localScale * 1.01f;

		// move the planet to be just below the zero plane
		m_planetModel.transform.localPosition = new Vector3( 0.0f, -16.0f - m_planetModel.transform.localScale.y, 0.0f );
		m_planetClouds.transform.localPosition = m_planetModel.transform.localPosition;

		// set up the clouds
		SetupClouds( m_planetClouds );
	}

	// call this to get the distance to the player
	public float GetDistanceToPlayer()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// return the distance from the player to the planet
		return Vector3.Distance( playerData.m_general.m_starSystemCoordinates, transform.localPosition );
	}

	// get the material for this planet
	public Material GetMaterial()
	{
		return m_material;
	}

	// get the legend texture for this planet
	public Texture2D GetLegendTexture()
	{
		return m_planetGenerator.m_legendTexture;
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

		if ( m_planetGenerator.m_mapsGenerated )
		{
			m_mapsGenerated = true;

			m_material.SetTexture( "AlbedoMap", m_planetGenerator.m_albedoTexture );
			m_material.SetTexture( "SpecularMap", m_planetGenerator.m_specularTexture );
			m_material.SetTexture( "NormalMap", m_planetGenerator.m_normalTexture );
			m_material.SetTexture( "WaterMaskMap", m_planetGenerator.m_waterMaskTexture );

			m_meshRenderer.material = m_material;

			m_spaceflightController.m_inOrbit.MaterialUpdated();
		}

		return progress;
	}

	// sets up the clouds based on planet properties
	public void SetupClouds( MeshRenderer planetClouds )
	{
		// does the planet have an atmosphere?
		if ( m_planet.HasAtmosphere() )
		{
			// yes - show the clouds
			planetClouds.gameObject.SetActive( true );

			// get the atmosphere density
			var atmosphereDensity = m_planet.GetAtmosphereDensity();

			// set the density of the clouds
			planetClouds.material.SetFloat( "_Density", 1.5f - atmosphereDensity );

			// pick the color for the clouds
			Color color;

			if ( m_planet.IsMolten() )
			{
				color = new Color( 0.25f, 0.25f, 0.25f );
			}
			else
			{
				color = new Color( 0.75f, 0.75f, 0.75f );
			}

			// set the opactiy
			color.a = atmosphereDensity * 0.5f + 0.5f;

			// apply the color to the clouds
			planetClouds.material.SetColor( "_Color", color );
		}
		else
		{
			// no - hide the clouds
			planetClouds.gameObject.SetActive( false );
		}
	}
}
