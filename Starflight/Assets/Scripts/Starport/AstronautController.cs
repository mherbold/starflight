
using UnityEngine;
using UnityEngine.AI;

public class AstronautController : MonoBehaviour
{
	// the door name controller
	public DoorNameController m_doorNameController;

	// the navmesh agent (used for moving and rotating the astronaut)
	public NavMeshAgent m_navMeshAgent;

	// the animator (used for making him run or be idle)
	public Animator m_animator;

	// starport panels
	public OperationsPanel m_operationsPanel;
	public PersonnelPanel m_personnelPanel;
	public CrewAssignmentPanel m_crewAssignmentPanel;
	public BankPanel m_bankPanel;
	public ShipConfigurationPanel m_shipConfigurationPanel;
	public TradeDepotPanel m_tradeDepotPanel;
	public DockingBayPanel m_dockingBayPanel;

	// the audio source (to use for playing footstep sounds)
	public AudioSource m_audioSource;

	// the audio clip to use for footsteps
	public AudioClip m_footstepAudioClip;

	// the speed at which we want to transition the height
	public float m_heightTransitionTime;

	// the time to wait before we start to dance
	public float m_danceWaitTime;

	// whether or not we were moving the astronaut
	bool m_astronautWasMoving;

	// direction we want to move the astronaut in
	Vector3 m_lastMoveVector;

	// whether or not we are transitioning the height
	bool m_transitioningHeight;

	// this is the original height above the floor
	float m_currentHeightAboveFloor;

	// this is the current height above the floor
	float m_targetHeightAboveFloor;

	// timer for height transitions
	float m_transitionTimer;

	// dance wait timer
	float m_danceWaitTimer;

	// unity start
	void Start()
	{
		// tell the NavMeshAgent component that we will be updating the astronaut position ourselves
		m_navMeshAgent.updatePosition = false;

		// reset height transition stuff
		m_transitioningHeight = false;
		m_currentHeightAboveFloor = 0.0f;
		m_targetHeightAboveFloor = 0.0f;

		// reset the timers
		m_transitionTimer = 0.0f;
		m_danceWaitTimer = 0.0f;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// put the astronaut where he should be
		m_navMeshAgent.Warp( playerData.m_general.m_lastStarportCoordinates );

		// make sure he is facing south
		transform.rotation = Quaternion.Euler( 0.0f, 180.0f, 0.0f );
	}

	// unity update
	void Update()
	{
		// update the dance wait timer
		m_danceWaitTimer += Time.deltaTime;

		// is it time to start dancing?
		if ( m_danceWaitTimer >= m_danceWaitTime )
		{
			// yes - dance!
			m_animator.SetBool( "Dance", true );

			m_danceWaitTimer = 0.0f;
		}

		// the astronaut can be moved only if a panel is not active and the astronaut is not transporting to the docking bay
		if ( !PanelController.m_instance.HasActivePanel() && !m_dockingBayPanel.IsTransporting() )
		{
			// get the controller stick position
			var x = InputController.m_instance.m_x;
			var z = InputController.m_instance.m_y;

			// create our 3d move vector from the controller position
			var moveVector = new Vector3( x, 0.0f, z );

			// check if the move vector will actually move the astronaut (controller is not centered)
			if ( moveVector.magnitude > 0.5f )
			{
				// reset the dance wait timer
				m_danceWaitTimer = 0.0f;

				// normalize the move vector to a length of 1.0 - so the astronaut will move the same distance in any direction
				moveVector.Normalize();

				// tell the NavMeshAgent component where we want to move the astronaut to
				m_navMeshAgent.SetDestination( transform.position + moveVector * 10.0f );

				// start the running animation (if not already running)
				m_animator.SetBool( "Run", true );

				// remember the move vector
				m_lastMoveVector = moveVector;

				// we are moving the astronaut
				m_astronautWasMoving = true;
			}
			else
			{
				// tell the astronaut to stop moving
				StopMoving();
			}

			// check if the player has pressed the fire button or the cancel button
			if ( InputController.m_instance.SubmitWasPressed() )
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
			else if ( InputController.m_instance.CancelWasPressed() )
			{
				PanelController.m_instance.m_saveGamePanel.SetCallbackObject( this );

				OpenPanel( PanelController.m_instance.m_saveGamePanel );
			}
		}
	}

	// call this when we want to open a starport panel
	void OpenPanel( Panel panel )
	{
		// hide the door name
		m_doorNameController.Hide();

		// tell the panel controller we want to open this panel
		PanelController.m_instance.Open( panel );

		// tell the astronaut to stop moving
		StopMoving();
	}

	// call this when a starport panel was closed
	public void PanelWasClosed()
	{
		// show the door name
		m_doorNameController.Show();

		// save the player data in case something has been updated
		DataController.m_instance.SaveActiveGame();
	}

	// call this to get the astronaut to stop moving
	void StopMoving()
	{
		// was the astronaut moving?
		if ( m_astronautWasMoving )
		{
			// tell the NavMeshAgent component where we want to move the astronaut to
			m_navMeshAgent.SetDestination( transform.position + m_lastMoveVector * 1.5f );

			// tell the animator component to transition the astronaut to the idle animation (if not already idling)
			m_animator.SetBool( "Run", false );

			// we are no longer moving the astronaut
			m_astronautWasMoving = false;
		}
	}

	// unity on animator move
	void OnAnimatorMove()
	{
		// did the height above the floor change?
		if ( !Tools.IsApproximatelyEqual( m_navMeshAgent.nextPosition.y, m_currentHeightAboveFloor, 0.1f ) )
		{
			// update the target height
			m_targetHeightAboveFloor = m_navMeshAgent.nextPosition.y;

			// have we started the transition?
			if ( !m_transitioningHeight )
			{
				// no - start it
				m_transitioningHeight = true;

				// reset the timer
				m_transitionTimer = 0.0f;
			}
		}

		// are we transitioning the height?
		if ( m_transitioningHeight )
		{
			// yes - update the timer
			m_transitionTimer += Time.deltaTime;

			// are we at the end of the transition?
			if ( m_transitionTimer >= m_heightTransitionTime )
			{
				// clamp the timer
				m_transitionTimer = m_heightTransitionTime;

				// update the current height above the floor
				m_currentHeightAboveFloor = m_targetHeightAboveFloor;

				// turn off the transition
				m_transitioningHeight = false;
			}

			// smooth out the astronaut's height above the floor changes (remove popping)
			var heightAboveFloor = Mathf.SmoothStep( m_currentHeightAboveFloor, m_targetHeightAboveFloor, m_transitionTimer / m_heightTransitionTime );

			// move the astronaut to where the NavMeshAgent component is telling us to move him to (except we are smoothing out the height changes)
			transform.position = new Vector3( m_navMeshAgent.nextPosition.x, heightAboveFloor, m_navMeshAgent.nextPosition.z );
		}
		else
		{
			// no - just move the astronaut to where the NavMeshAgent component is telling us to move him to
			transform.position = m_navMeshAgent.nextPosition;
		}

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// save the player position
		playerData.m_general.m_coordinates = m_navMeshAgent.nextPosition;
		playerData.m_general.m_lastStarportCoordinates = playerData.m_general.m_coordinates;
	}

	// unity on trigger enter
	void OnTriggerEnter( Collider other )
	{
		// change the text to show the tag of the trigger we have collided with (we conveniently set up each trigger's tag to be the door name)
		m_doorNameController.SetText( other.tag );
	}

	// unity on trigger exit
	void OnTriggerExit( Collider other )
	{
		// hide the door name text
		m_doorNameController.Hide();
	}

	// this is called via animation event
	void PlayFootstepSound()
	{
		// play the footstep sound
		m_audioSource.PlayOneShot( m_footstepAudioClip, 1.0f );
	}

	// this is called via animation event
	void DanceUpdate()
	{
		m_animator.SetBool( "Dance", false );

		m_danceWaitTimer = 0.0f;
	}
}
