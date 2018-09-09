
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

	// this is the current height above the floor
	float m_currentHeightAboveFloor;

	// this is called by unity once at the start of the level
	void Start()
	{
		// tell the NavMeshAgent component that we will be updating the astronaut position ourselves
		m_navMeshAgent.updatePosition = false;

		// remember the last height of the astronaut above the floor
		m_currentHeightAboveFloor = m_navMeshAgent.nextPosition.y;
	}

	// this is called by unity every frame
	void OnAnimatorMove()
	{
		// smooth out the astronaut's height above the floor changes (remove popping)
		float smooth = Mathf.Min( 1.0f, Time.deltaTime / 0.15f );
		m_currentHeightAboveFloor = Mathf.Lerp( m_currentHeightAboveFloor, m_navMeshAgent.nextPosition.y, smooth );

		// move the astronaut to where the NavMeshAgent component is telling us to move him to (except we are smoothing out the height changes)
		transform.position = new Vector3( m_navMeshAgent.nextPosition.x, m_currentHeightAboveFloor, m_navMeshAgent.nextPosition.z );
	}

	// this is called by the astronaut's CapsuleCollider component once when it detects a collision with a trigger
	void OnTriggerEnter( Collider other )
	{
		// change the text to show the tag of the trigger we have collided with (we conveniently set up each trigger's tag to be the door name)
		m_doorNameController.SetText( other.tag );
	}

	// this is called by the astronaut's CapsuleCollider component once when it no longer detects a collision with a trigger
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
