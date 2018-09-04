
using UnityEngine;
using TMPro;

public class DockingBay : MonoBehaviour
{
	// the docking bay doors
	public Animator m_dockingBayDoorTop;
	public Animator m_dockingBayDoorBottom;

	// particle systems
	public ParticleSystem m_decompressionParticleSystem;

	// convenient access to the spaceflight controller
	SpaceflightController m_spaceflightController;

	// unity awake
	private void Awake()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();
	}

	// unity start
	void Start()
	{

	}

	// unity update
	void Update()
	{

	}

	// call this to hide the docking bay
	public void Hide()
	{
		// hide the docking bay
		gameObject.SetActive( false );
	}

	// call this to switch to the docking bay
	public void Show()
	{
		// show the docking bay
		gameObject.SetActive( true );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// put us in the arth system (just in case)
		playerData.m_starflight.m_currentStarId = gameData.m_misc.m_arthStarId;
		playerData.m_starflight.m_hyperspaceCoordinates = Tools.GameToWorldCoordinates( new Vector3( 125.0f, 0.0f, 100.0f ) );

		// put us in the right spot for the docking bay launch sequence
		playerData.m_starflight.m_systemCoordinates = Tools.GameToWorldCoordinates( new Vector3( 0.0f, 0.0f, 0.0f ) );

		// dolly the camera to the start of the cinematic sequence
		m_spaceflightController.m_player.DollyCamera( 2048.0f );

		// play the docking bay music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.DockingBay );
	}

	// call this to open the docking bay doors
	public void OpenDockingBayDoors()
	{
		// open the top docking bay door
		m_dockingBayDoorTop.Play( "Open" );

		// open the bottom docking bay door
		m_dockingBayDoorBottom.Play( "Open" );

		// fire up the particle system
		m_decompressionParticleSystem.Play();

		// play the docking bay door open sound
		SoundController.m_instance.PlaySound( SoundController.Sound.DockingBayDoorOpen );

		// play the decompression sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Decompression );
	}
}
