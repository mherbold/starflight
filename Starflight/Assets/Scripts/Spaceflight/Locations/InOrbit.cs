
using UnityEngine;

public class InOrbit : MonoBehaviour
{
	// the nebula overlay
	public GameObject m_nebula;

	// the planet model
	public MeshRenderer m_planetModel;

	// the planet cloud
	public MeshRenderer m_clouds;

	// the planet atmosphere
	public GameObject m_planetAtmosphere;

	// current planet spin
	float m_spin;

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
		// don't do anything if the game is paused
		if ( SpaceflightController.m_instance.m_gameIsPaused )
		{
			return;
		}

		// slowly spin the planet
		m_spin += Time.deltaTime * SpaceflightController.m_instance.m_planetRotationSpeed;

		// wrap the spin around to avoid FP issues
		if ( m_spin >= 360.0f )
		{
			m_spin -= 360.0f;
		}

		// calculate the new rotation quaternion
		var newRotation = Quaternion.Euler( 0.0f, 0.0f, m_spin );

		// apply it to the planet
		m_planetModel.transform.localRotation = newRotation;
	}

	// call this to hide the in orbit objects
	public void Hide()
	{
		if ( !gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the in orbit location." );

		// hide the in orbit location
		gameObject.SetActive( false );
	}

	// call this to show the in orbit objects
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the in orbit location." );

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the star data
		var star = gameData.m_starList[ playerData.m_general.m_currentStarId ];

		// show the in orbit objects
		gameObject.SetActive( true );

		// get the planet controller
		var planetController = SpaceflightController.m_instance.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

		// set the scale of the planet model (and clouds and atmosphere)
		var scale = planetController.m_planet.GetScale();
		m_planetModel.transform.localScale = scale * 3.0f;
		m_planetAtmosphere.transform.localScale = m_planetModel.transform.localScale * 1.055f;
		m_clouds.transform.localScale = m_planetModel.transform.localScale * 1.01f;

		// position the planet (and clouds and atmosphere)
		var position = Vector3.up * -m_planetModel.transform.localScale.y;
		m_planetModel.transform.localPosition = position;
		m_planetAtmosphere.transform.localPosition = position * 0.85f;
		m_clouds.transform.localPosition = position;

		// move the player object
		SpaceflightController.m_instance.m_playerShip.transform.position = playerData.m_general.m_coordinates = Vector3.zero;

		// we don't want the camera to follow anything
		SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( null, Vector3.zero, Quaternion.identity, false );

		// freeze the player
		SpaceflightController.m_instance.m_playerShip.Freeze();

		// reset the buttons (only if we aren't in the middle of launching)
		if ( playerData.m_general.m_lastLocation != PD_General.Location.Planetside )
		{
			SpaceflightController.m_instance.m_buttonController.RestoreBridgeButtons();
		}

		// fade in the map
		SpaceflightController.m_instance.m_viewport.StartFade( 1.0f, 2.0f );

		// show / hide the nebula depending on if we are in one
		m_nebula.SetActive( star.m_insideNebula );

		// play the docking bay music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.InOrbit );

		// let the player know we've established orbit
		SpaceflightController.m_instance.m_messages.ChangeText( "<color=white>Orbit established.</color>" );

		// set up the clouds and atmosphere
		planetController.SetupClouds( m_clouds, m_planetAtmosphere, true, false );

		// make sure skybox blend is off
		StarflightSkybox.m_instance.m_currentBlendFactor = 0.0f;

		// turn skybox autorotate off
		StarflightSkybox.m_instance.m_autorotateSkybox = false;

		// make sure skybox rotation is reset
		StarflightSkybox.m_instance.m_currentRotation = Quaternion.identity;

		// apply the material to the planet model
		MaterialUpdated();
	}

	public void MaterialUpdated()
	{
		if ( gameObject.activeInHierarchy )
		{
			// get to the player data
			var playerData = DataController.m_instance.m_playerData;

			// get the planet controller
			var planetController = SpaceflightController.m_instance.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

			// apply the material to the planet model
			m_planetModel.material = planetController.GetMaterial();
		}
	}
}
