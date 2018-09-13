
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SpaceflightController : MonoBehaviour
{
	// set this to true to skip cinematics
	public bool m_skipCinematics;
	public bool m_forceResetToDockingBay;

	// set this to the maximum speed of the ship
	public float m_maximumShipSpeedHyperspace;
	public float m_maximumShipSpeedStarSystem;
	public float m_timeToReachMaximumShipSpeed;
	public float m_timeToStop;

	// the different components of the spaceflight scene
	public Player m_player;
	public DockingBay m_dockingBay;
	public StarSystem m_starSystem;
	public InOrbit m_inOrbit;
	public Hyperspace m_hyperspace;
	public SpaceflightUI m_spaceflightUI;

	// controllers
	public ButtonController m_buttonController;
	public DisplayController m_displayController;
	public SystemController m_systemController;

	// save game timer
	float m_timer;

	// unity awake
	void Awake()
	{
		// hide all components
		m_player.Hide();
		m_dockingBay.Hide();
		m_starSystem.Hide();
		m_hyperspace.Hide();

		// check if we loaded the persistent scene
		if ( DataController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			DataController.m_sceneToLoad = "Spaceflight";

			SceneManager.LoadScene( "Persistent" );
		}
	}

	// unity start
	void Start()
	{
		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// switch to the current location
		SwitchLocation( playerData.m_starflight.m_location );

		// fade in the scene
		SceneFadeController.m_instance.FadeIn();

		// reset the save game timer
		m_timer = 0.0f;
	}

	// unity update
	void Update()
	{
		// don't do anything if we have a panel open
		if ( PanelController.m_instance.HasActivePanel() )
		{
			return;
		}

		// update the game time
		PlayerData playerData = DataController.m_instance.m_playerData;

		playerData.m_starflight.UpdateGameTime( Time.deltaTime );

		// save the game once in a while
		m_timer += Time.deltaTime;

		if ( m_timer >= 30.0f )
		{
			m_timer -= 30.0f;

			DataController.m_instance.SaveActiveGame();
		}

		// when player hits cancel (esc) show the save game panel
		if ( InputController.m_instance.CancelWasPressed() )
		{
			PanelController.m_instance.m_saveGamePanel.SetCallbackObject( this );

			PanelController.m_instance.Open( PanelController.m_instance.m_saveGamePanel );
		}
	}

	// call this to switch to the correct location
	public void SwitchLocation( Starflight.Location newLocation )
	{
		// switch to the correct mode
		Debug.Log( "Switching to location " + newLocation );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// update the player data
		playerData.m_starflight.m_location = newLocation;

		// switching to starport is a special case
		if ( playerData.m_starflight.m_location == Starflight.Location.Starport )
		{
			// start fading out the spaceflight scene
			SceneFadeController.m_instance.FadeOut( "Starport" );
		}
		else
		{
			// hide all components
			m_player.Hide();
			m_dockingBay.Hide();
			m_starSystem.Hide();
			m_hyperspace.Hide();

			// make sure the map is visible
			m_spaceflightUI.FadeMap( 1.0f, 2.0f );

			// switch the location
			switch ( playerData.m_starflight.m_location )
			{
				case Starflight.Location.DockingBay:
					m_dockingBay.Show();
					break;
				case Starflight.Location.JustLaunched:
					SwitchToJustLaunched();
					break;
				case Starflight.Location.StarSystem:
					m_starSystem.Show();
					break;
				case Starflight.Location.InOrbit:
					m_inOrbit.Show();
					break;
				case Starflight.Location.Hyperspace:
					m_hyperspace.Show();
					break;
			}
		}

		// save the player data (since the location was most likely changed)
		DataController.m_instance.SaveActiveGame();
	}
	
	// call this when a the save game panel was closed
	public void PanelWasClosed()
	{
		// save the player data in case something has been updated
		DataController.m_instance.SaveActiveGame();
	}

	void SwitchToJustLaunched()
	{
		// show the star system
		m_starSystem.Show();

		// make sure the map overlay is black
		m_spaceflightUI.FadeMap( 0.0f, 0.0f );

		// update the message shown
		m_spaceflightUI.ChangeMessageText( "Starport clear.\nStanding by to maneuver." );
	}
}
