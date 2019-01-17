
using UnityEngine;

public class Planetside : MonoBehaviour
{
	// the terrain grid
	public TerrainGrid m_terrainGrid;

	// the clouds
	public GameObject m_clouds;

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
	}

	// call this to hide the in orbit objects
	public void Hide()
	{
		if ( !gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the planetside location." );

		// hide the planetside location
		gameObject.SetActive( false );
	}

	// call this to show the planetside objects
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the planetside location." );

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the star data
		var star = gameData.m_starList[ playerData.m_general.m_currentStarId ];

		// show the planetside objects
		gameObject.SetActive( true );

		// get the planet controller
		var planetController = SpaceflightController.m_instance.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

		// move the player object
		SpaceflightController.m_instance.m_player.transform.position = playerData.m_general.m_coordinates = new Vector3( 0.0f, 0.0f, 0.0f );

		// play an animation to move the camera to the right place
		SpaceflightController.m_instance.m_playerCamera.StartAnimation( "On Planet" );

		// freeze the player
		SpaceflightController.m_instance.m_player.Freeze();

		// reset the buttons
		SpaceflightController.m_instance.m_buttonController.RestoreBridgeButtons();

		// fade in the map
		SpaceflightController.m_instance.m_viewport.StartFade( 1.0f, 2.0f );

		// play the docking bay music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.InOrbit );

		// set up the clouds and atmosphere
		// planetController.SetupClouds( m_clouds, null );

		// apply the material to the planet model
		// MaterialUpdated();
	}

	// set the landing coordinates
	public void SetLandingCoordinates( float latitude, float longitude )
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the planet controller
		var planetController = SpaceflightController.m_instance.m_starSystem.GetPlanetController( playerData.m_general.m_currentPlanetId );

		// get the planet generator
		var planetGenerator = planetController.GetPlanetGenerator();

		m_terrainGrid.SetLandingCoordinates( latitude, longitude, planetGenerator );
	}
}
