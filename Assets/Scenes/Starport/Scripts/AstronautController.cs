
using UnityEngine;
using UnityEngine.AI;

public class AstronautController : MonoBehaviour
{
	// the navmesh agent (used for controlling the astronaut)
	public NavMeshAgent m_navMeshAgent;

	// the animator (used for making him run or be idle)
	public Animator m_animator;

	// call this to move the astronaut
	public void Move( Vector3 relativePosition )
	{
		m_navMeshAgent.SetDestination( transform.position + relativePosition );

		m_animator.SetBool( "Run", true );
	}

	// call this to tell the astronaut to transition to the idle animation
	public void TransitionToIdle()
	{
		m_animator.SetBool( "Run", false );
	}
}
