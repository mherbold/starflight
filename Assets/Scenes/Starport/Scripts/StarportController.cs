
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StarportController : MonoBehaviour
{
	// stuff shared by all the controllers
	public AstronautController m_astronautController;
	public DoorNameController m_doorNameController;

	// starport panels
	public OperationsPanel m_operationsPanel;
	public PersonnelPanel m_personnelPanel;
	public CrewAssignmentPanel m_crewAssignmentPanel;
	public BankPanel m_bankPanel;
	public ShipConfigurationPanel m_shipConfigurationPanel;
	public TradeDepotPanel m_tradeDepotPanel;
	public DockingBayPanel m_dockingBayPanel;

	// whether or not we were moving the astronaut
	bool m_astronautWasMoving;

	// direction we want to move the astronaut in
	Vector3 m_lastMoveVector;

	// unity awake
	void Awake()
	{
		// check if we loaded the persistent scene
		if ( DataController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			DataController.m_sceneToLoad = "Starport";

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

		// make sure we are in the right scene
		switch ( playerData.m_starflight.m_location )
		{
			case Starflight.Location.Starport:

				// start playing the starport music
				MusicController.m_instance.ChangeToTrack( MusicController.Track.Starport );

				// show the door name
				m_doorNameController.Show();

				// fade the scene in
				SceneFadeController.m_instance.FadeIn();

				break;

			default:
				SceneManager.LoadScene( "Spaceflight" );
				break;
		}
	}

	// unity update
	void Update()
	{
		// the astronaut can be moved only if a panel is not active and the astronaut is not transporting to the docking bay
		if ( !PanelController.m_instance.HasActivePanel() && !m_dockingBayPanel.IsTransporting() )
		{
			// let the user move the astronaut
			Move();

			// check if the player has pressed the fire button or the cancel button
			if ( InputController.m_instance.SubmitWasPressed() )
			{
				Fire();
			}
			else if ( InputController.m_instance.CancelWasPressed() )
			{
				PanelController.m_instance.m_saveGamePanel.SetCallbackObject( this );

				OpenPanel( PanelController.m_instance.m_saveGamePanel );
			}
		}
	}

	// handle controller stick input
	void Move()
	{
		// get the controller stick position
		float x = InputController.m_instance.m_x;
		float z = InputController.m_instance.m_y;

		// create our 3d move vector from the controller position
		Vector3 moveVector = new Vector3( x, 0.0f, z );

		// check if the move vector will actually move the astronaut (controller is not centered)
		if ( moveVector.magnitude > 0.5f )
		{
			// normalize the move vector to a length of 1.0 - so the astronaut will move the same distance in any direction
			moveVector.Normalize();

			// tell the NavMeshAgent component where we want to move the astronaut to
			m_astronautController.Move( moveVector * 10.0f );

			// remember the move vector
			m_lastMoveVector = moveVector;

			// we are moving the astronaut
			m_astronautWasMoving = true;
		}
		else if ( m_astronautWasMoving )
		{
			// tell the NavMeshAgent component where we want to move the astronaut to
			m_astronautController.Move( m_lastMoveVector * 1.5f );

			// tell the Animator component to transition the astronaut to the idle animation (if not already idling)
			m_astronautController.TransitionToIdle();

			// we are no longer moving the astronaut
			m_astronautWasMoving = false;
		}
	}

	// handle fire button input
	void Fire()
	{
		// check if we are showing a door name
		if ( m_doorNameController.IsShowingDoorName() )
		{
			// get the name of the current door
			string currentDoorName = m_doorNameController.GetCurrentDoorName();

			// figure out which controller to give control to
			if ( currentDoorName == "Operations" )
			{
				// give control to the operations controller
				OpenPanel( m_operationsPanel );
			}
			else if ( currentDoorName == "Personnel" )
			{
				// give control to the personnel controller
				OpenPanel( m_personnelPanel );
			}
			else if ( currentDoorName == "Crew Assignment" )
			{
				// give control to the personnel controller
				OpenPanel( m_crewAssignmentPanel );
			}
			else if ( currentDoorName == "Bank" )
			{
				// give control to the bank controller
				OpenPanel( m_bankPanel );
			}
			else if ( currentDoorName == "Ship Configuration" )
			{
				// give control to the ship configuration controller
				OpenPanel( m_shipConfigurationPanel );
			}
			else if ( currentDoorName == "Trade Depot" )
			{
				// give control to the trade depot controller
				OpenPanel( m_tradeDepotPanel );
			}
			else if ( currentDoorName == "Docking Bay" )
			{
				// see if we can teleport to the docking bay
				OpenPanel( m_dockingBayPanel );
			}
		}
	}

	// call this when we want to open a starport panel
	public void OpenPanel( Panel panel )
	{
		// hide the door name
		m_doorNameController.Hide();

		// tell the panel controller we want to open this panel
		PanelController.m_instance.Open( panel );
	}

	// call this when a starport panel was closed
	public void PanelWasClosed()
	{
		// show the door name
		m_doorNameController.Show();

		// save the player data in case something has been updated
		DataController.m_instance.SaveActiveGame();
	}
}
