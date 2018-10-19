
using UnityEngine;

public class InOrbit : MonoBehaviour
{
	// the nebula overlay
	public GameObject m_nebula;

	// the planet
	public MeshRenderer m_planet;

	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

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
		// slowly spin the planet
		m_spin += Time.deltaTime;

		// wrap the spin around to avoid FP issues
		if ( m_spin >= 360.0f )
		{
			m_spin -= 360.0f;
		}

		// calculate the new rotation quaternion
		var newRotation = Quaternion.Euler( 0.0f, 0.0f, m_spin );

		// apply it to the planet
		m_planet.transform.localRotation = newRotation;
	}

	// call this to hide the in orbit objects
	public void Hide()
	{
		if ( !gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the in orbit scene." );

		// hide the starsystem
		gameObject.SetActive( false );
	}

	// call this to show the in orbit objects
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the in orbit scene." );

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the star data
		var star = gameData.m_starList[ playerData.m_starflight.m_currentStarId ];

		// show the in orbit objects
		gameObject.SetActive( true );

		// get the planet controller
		var planetController = m_spaceflightController.m_starSystem.GetPlanetController( playerData.m_starflight.m_currentPlanetId );

		// set the scale of the planet
		m_planet.transform.localScale = planetController.m_planet.GetScale();

		// make sure the camera is at the right height above the zero plane
		var orbitHeight = Mathf.Lerp( 256.0f, 768.0f, m_planet.transform.localScale.z / 320.0f );

		m_spaceflightController.m_player.DollyCamera( orbitHeight );

		// move the player object
		m_spaceflightController.m_player.transform.position = new Vector3( 0.0f, 0.0f, 0.0f );

		// freeze the player
		m_spaceflightController.m_player.Freeze();

		// reset the buttons
		m_spaceflightController.m_buttonController.RestoreBridgeButtons();

		// fade in the map
		m_spaceflightController.m_spaceflightUI.FadeMap( 1.0f, 2.0f );

		// show / hide the nebula depending on if we are in one
		m_nebula.SetActive( star.m_insideNebula );

		// play the docking bay music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.InOrbit );

		// let the player know we've established orbit
		m_spaceflightController.m_spaceflightUI.ChangeMessageText( "<color=white>Orbit established.</color>" );

		// set the position of the sun
		m_planet.material.SetVector( "_SunPosition", new Vector4( -10000.0f, 5000.0f, 0.0f, 0.0f ) );

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
			var planetController = m_spaceflightController.m_starSystem.GetPlanetController( playerData.m_starflight.m_currentPlanetId );

			// apply the material to the planet model
			m_planet.material = planetController.GetMaterial();
		}
	}
}
