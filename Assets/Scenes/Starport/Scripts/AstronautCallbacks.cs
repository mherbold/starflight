
using UnityEngine;
using UnityEngine.AI;

public class AstronautCallbacks : MonoBehaviour
{
	// the door name controller
	public DoorNameController m_doorNameController;

	// the nav mesh agent
	public NavMeshAgent m_navMeshAgent;

	// the audio source (to use for playing footstep sounds)
	public AudioSource m_audioSource;

	// the audio clip to use for footsteps
	public AudioClip m_footstepAudioClip;

	// the speed at which we want to transition the height
	public float m_heightTransitionTime;

	// whether or not we are transitioning the height
	bool m_transitioningHeight;

	// this is the original height above the floor
	float m_currentHeightAboveFloor;

	// this is the current height above the floor
	float m_targetHeightAboveFloor;

	// timer for height transitions
	float m_timer;

	// unity start
	void Start()
	{
		// tell the NavMeshAgent component that we will be updating the astronaut position ourselves
		m_navMeshAgent.updatePosition = false;

		// reset height transition stuff
		m_transitioningHeight = false;
		m_currentHeightAboveFloor = 0.0f;
		m_targetHeightAboveFloor = 0.0f;
		m_timer = 0.0f;
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

				// reset the transition timer
				m_timer = 0.0f;
			}
		}

		// are we transitioning the height?
		if ( m_transitioningHeight )
		{
			// yes - update the timer
			m_timer += Time.deltaTime;

			// are we at the end of the transition?
			if ( m_timer >= m_heightTransitionTime )
			{
				// clamp the timer
				m_timer = m_heightTransitionTime;

				// update the current height above the floor
				m_currentHeightAboveFloor = m_targetHeightAboveFloor;

				// turn off the transition
				m_transitioningHeight = false;
			}

			// smooth out the astronaut's height above the floor changes (remove popping)
			float heightAboveFloor = Mathf.SmoothStep( m_currentHeightAboveFloor, m_targetHeightAboveFloor, m_timer / m_heightTransitionTime );

			// move the astronaut to where the NavMeshAgent component is telling us to move him to (except we are smoothing out the height changes)
			transform.position = new Vector3( m_navMeshAgent.nextPosition.x, heightAboveFloor, m_navMeshAgent.nextPosition.z );
		}
		else
		{
			// no - just move the astronaut to where the NavMeshAgent component is telling us to move him to
			transform.position = m_navMeshAgent.nextPosition;
		}
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

	// call this to play the footstep sound
	void PlayFootstepSound()
	{
		// play the footstep sound
		m_audioSource.PlayOneShot( m_footstepAudioClip, 1.0f );
	}
}
