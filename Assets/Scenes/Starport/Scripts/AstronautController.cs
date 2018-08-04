
using UnityEngine;
using UnityEngine.AI;

public class AstronautController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public GameObject m_astronautGameObject;
	public GameObject m_transporterParticles;

	// private stuff we don't want the editor to see
	private NavMeshAgent m_navMeshAgent;
	private Animator m_animator;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the NavMeshAgent
		m_navMeshAgent = m_astronautGameObject.GetComponent<NavMeshAgent>();

		// get access to the animator
		m_animator = m_astronautGameObject.GetComponent<Animator>();
	}

	// this is called by unity every frame
	private void Update()
	{
		// move the transporter particles along with the astronaut
		m_transporterParticles.transform.position = m_astronautGameObject.transform.position;

	}

	// call this to move the astronaut
	public void Move( Vector3 relativePosition )
	{
		m_navMeshAgent.SetDestination( m_astronautGameObject.transform.position + relativePosition );

		m_animator.SetBool( "Move", true );
	}

	// call this to find out if we are in the middle of transitioning to the idle animation
	public bool IsTransitioningToIdle()
	{
		AnimatorTransitionInfo animatorTransitionInfo = m_animator.GetAnimatorTransitionInfo( 0 );

		return animatorTransitionInfo.IsUserName( "Stopping" );
	}

	// call this to tell the astronaut to transition to the idle animation
	public void TransitionToIdle()
	{
		m_animator.SetBool( "Move", false );
	}
}
