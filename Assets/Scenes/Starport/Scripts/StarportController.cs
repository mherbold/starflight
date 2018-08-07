
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StarportController : MonoBehaviour
{
	// the scene transition controller
	public ManualSceneTransitionController m_manualSceneTransitionController { get; private set; }

	// stuff shared by all the controllers
	public InputManager m_inputManager { get; private set; }
	public BasicSound m_basicSound { get; private set; }
	public UISoundController m_uiSoundController { get; protected set; }
	public AstronautController m_astronautController { get; private set; }
	public DoorNameController m_doorNameController { get; private set; }

	// door controllers
	private OperationsController m_operationsController;
	private PersonnelController m_personnelController;
	private CrewAssignmentController m_crewAssignmentController;
	private BankController m_bankController;
	private ShipConfigurationController m_shipConfigurationController;
	private TradeDepotController m_tradeDepotController;
	private DockingBayController m_dockingBayController;

	// keep track of which door is the current one
	private DoorController m_currentUIController;

	// keep track of whether or not the astronaut can be moved
	private bool m_astronautCanBeMoved;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the manual scene transition controller
		m_manualSceneTransitionController = GetComponent<ManualSceneTransitionController>();

		// get access to the input manager
		m_inputManager = GetComponent<InputManager>();

		// get access to the basic sound
		m_basicSound = GetComponent<BasicSound>();

		// get access to the ui sound controller
		m_uiSoundController = GetComponent<UISoundController>();

		// get access to the astronaut controller
		m_astronautController = GetComponent<AstronautController>();

		// get access to the door name controller
		m_doorNameController = GetComponent<DoorNameController>();

		// get access to the operations controller
		m_operationsController = GetComponent<OperationsController>();

		// get access to the personnel controller
		m_personnelController = GetComponent<PersonnelController>();

		// get access to the crew assignment controller
		m_crewAssignmentController = GetComponent<CrewAssignmentController>();

		// get access to the bank controller
		m_bankController = GetComponent<BankController>();

		// get access to the bank controller
		m_shipConfigurationController = GetComponent<ShipConfigurationController>();

		// get access to the trade depot controller
		m_tradeDepotController = GetComponent<TradeDepotController>();

		// get access to the docking bay controller
		m_dockingBayController = GetComponent<DockingBayController>();
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;

		// we have the focus
		TakeFocus();

		// check if we loaded the persistent scene
		if ( PersistentController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			PersistentController.m_sceneToLoad = "Starport";

			SceneManager.LoadScene( "Persistent" );
		}
	}

	// this is called by unity every frame
	private void Update()
	{
		// check if the astronaut can be moved
		if ( m_astronautCanBeMoved )
		{
			// we want to allow the player to move the astronaut only when the astronaut is not currently transitioning to the idle animation - this prevents weird looking feet sliding
			if ( !m_astronautController.IsTransitioningToIdle() )
			{
				Move();
			}

			// check if the player has pressed the fire button
			if ( m_inputManager.GetSubmitDown() )
			{
				Fire();
			}
		}
	}

	// handle controller stick input
	private void Move()
	{
		// get the controller stick position
		float x = m_inputManager.m_xRaw;
		float z = m_inputManager.m_yRaw;

		// create our 3d move vector from the controller position
		Vector3 moveVector = new Vector3( x, 0.0f, z );

		// check if the move vector will actually move the astronaut (controller is not centered)
		if ( moveVector.magnitude > 0.5f )
		{
			// normalize the move vector to a length of 1.0 - so the astronaut will move the same distance in any direction
			moveVector.Normalize();

			// tell the NavMeshAgent component where we want to move the astronaut to - 3 feet away in the direction of the controller
			m_astronautController.Move( moveVector * 3.0f );
		}
		else
		{
			// tell the Animator component to transition the astronaut to the idle animation (if not already idling)
			m_astronautController.TransitionToIdle();
		}
	}

	// handle fire button input
	private void Fire()
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
				LoseFocus( m_operationsController );
			}
			else if ( currentDoorName == "Personnel" )
			{
				// give control to the personnel controller
				LoseFocus( m_personnelController );
			}
			else if ( currentDoorName == "Crew Assignment" )
			{
				// give control to the personnel controller
				LoseFocus( m_crewAssignmentController );
			}
			else if ( currentDoorName == "Bank" )
			{
				// give control to the bank controller
				LoseFocus( m_bankController );
			}
			else if ( currentDoorName == "Ship Configuration" )
			{
				// give control to the ship configuration controller
				LoseFocus( m_shipConfigurationController );
			}
			else if ( currentDoorName == "Trade Depot" )
			{
				// give control to the trade depot controller
				LoseFocus( m_tradeDepotController );
			}
			else if ( currentDoorName == "Docking Bay" )
			{
				// see if we can teleport to the docking bay
				LoseFocus( m_dockingBayController );
			}
		}
	}

	// call this when we are taking the focus
	public void TakeFocus()
	{
		// we have the controller focus
		m_astronautCanBeMoved = true;

		// show the door name
		m_doorNameController.Show();

		// clear out the current ui controller
		m_currentUIController = null;

		// save the player data in case something has been updated
		if ( PersistentController.m_instance )
		{
			PersistentController.m_instance.SavePlayerData();
		}
	}

	// call this when we are losing the focus and giving it to one of the door ui controllers
	public void LoseFocus( DoorController uiController )
	{
		// we have the controller focus
		m_astronautCanBeMoved = false;

		// hide the door name
		m_doorNameController.Hide();

		// remember the current ui controller
		m_currentUIController = uiController;

		// call show on the ui controller
		m_currentUIController.Show();
	}

	// this is called by the ui animation callback
	public void FinishOpeningUI()
	{
		// let the currently active ui controller know we have finished opening the ui
		m_currentUIController.FinishOpeningUI();
	}

	// this is called by the ui animation callback
	public void FinishClosingUI()
	{
		// let the currently active ui controller know we have finished closing the ui
		m_currentUIController.FinishClosingUI();
	}
}
