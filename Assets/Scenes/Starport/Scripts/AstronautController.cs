
using UnityEngine;
using UnityEngine.AI;

public class AstronautController : MonoBehaviour
{
	// the transporter particle system
	public ParticleSystem m_transporterParticleSystem;

	// the navmesh agent (used for controlling the astronaut)
	public NavMeshAgent m_navMeshAgent;

	// the animator (used for making him run or be bored)
	public Animator m_animator;

	// unity awake
	void Awake()
	{
	}

	// this is called by unity every frame
	void Update()
	{
		// move the transporter particles along with the astronaut
		m_transporterParticleSystem.transform.position = transform.position;
	}

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
