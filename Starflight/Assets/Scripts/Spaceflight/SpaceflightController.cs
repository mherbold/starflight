
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SpaceflightController : MonoBehaviour
{
	// set this to true to skip cinematics
	public bool m_skipCinematics;
	public bool m_forceResetToDockingBay;

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

	// save game timer
	float m_timer;

	// remember whether or not we have faded in the scene already
	bool m_alreadyFadedIn;

	// unity awake
	void Awake()
	{
		// immediately hide any game objects that we might have left visible in the editor
		m_player.Hide();
		m_dockingBay.Hide();
		m_starSystem.Hide();
		m_inOrbit.Hide();
		m_hyperspace.Hide();

		// check if we loaded the persistent scene
		if ( DataController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			DataController.m_sceneToLoad = "Spaceflight";

			// debug info
			Debug.Log( "Loading scene Persistent" );

			// load the persistent scene
			SceneManager.LoadScene( "Persistent" );
		}
	}

	// unity start
	void Start()
	{
		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// switch to the current location
		SwitchLocation( playerData.m_starflight.m_location );

		// make sure the scene is blacked out
		SceneFadeController.m_instance.BlackOut();

		// reset the save game timer
		m_timer = 0.0f;
	}

	// unity update
	void Update()
	{
		// Debug.Log( "Update()" );

		// are we generating planets?
		if ( m_starSystem.GeneratingPlanets() )
		{
			// yes - continue generating planets
			var totalProgress = m_starSystem.GeneratePlanets();

			// show / update the pop up dialog
			PopupController.m_instance.ShowPopup( "Commencing System Penetration", totalProgress );
		}
		else
		{
			// hide the pop up dialog
			PopupController.m_instance.HidePopup();

			// fade in the scene if we haven't already
			if ( !m_alreadyFadedIn )
			{
				// fade in the scene
				SceneFadeController.m_instance.FadeIn();

				// remember that we have faded in the scene
				m_alreadyFadedIn = true;
			}
		}

		// don't do anything if we have a panel open
		if ( PanelController.m_instance.HasActivePanel() )
		{
			return;
		}

		// don't do anything if we have a pop up dialog open
		if ( PopupController.m_instance.IsActive() )
		{
			return;
		}

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// are we in the star system or hyperspace locations?
		if ( ( playerData.m_starflight.m_location == PD_General.Location.StarSystem ) || ( playerData.m_starflight.m_location == PD_General.Location.Hyperspace ) )
		{
			// yes - update the game time
			playerData.m_starflight.UpdateGameTime( Time.deltaTime );
		}

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
	public void SwitchLocation( PD_General.Location newLocation )
	{
		// switch to the correct mode
		Debug.Log( "Switching to location " + newLocation );

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// update the player data
		playerData.m_starflight.m_location = newLocation;

		// switching to starport is a special case
		if ( playerData.m_starflight.m_location == PD_General.Location.Starport )
		{
			// start fading out the spaceflight scene
			SceneFadeController.m_instance.FadeOut( "Starport" );
		}
		else
		{
			// make sure the map is visible
			m_spaceflightUI.FadeMap( 1.0f, 2.0f );

			// switch the location
			switch ( playerData.m_starflight.m_location )
			{
				case PD_General.Location.DockingBay:
					m_player.Hide();
					m_dockingBay.Show();
					m_starSystem.Hide();
					m_inOrbit.Hide();
					m_hyperspace.Hide();
					break;

				case PD_General.Location.JustLaunched:
					m_player.Hide();
					m_dockingBay.Hide();
					m_starSystem.Hide();
					m_inOrbit.Hide();
					m_hyperspace.Hide();
					m_spaceflightUI.FadeMap( 0.0f, 0.0f );
					m_spaceflightUI.ChangeMessageText( "<color=white>Starport clear.\nStanding by to maneuver.</color>" );
					break;

				case PD_General.Location.StarSystem:
					m_player.Show();
					m_dockingBay.Hide();
					m_starSystem.Initialize();
					m_starSystem.Show();
					m_inOrbit.Hide();
					m_hyperspace.Hide();
					break;

				case PD_General.Location.InOrbit:
					m_player.Hide();
					m_dockingBay.Hide();
					m_starSystem.Initialize();
					m_starSystem.Hide();
					m_inOrbit.Show();
					m_hyperspace.Hide();
					break;

				case PD_General.Location.Hyperspace:
					m_player.Show();
					m_dockingBay.Hide();
					m_starSystem.Hide();
					m_inOrbit.Hide();
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
}
