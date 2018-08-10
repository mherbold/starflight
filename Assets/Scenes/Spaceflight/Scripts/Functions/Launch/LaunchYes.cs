
using UnityEngine;
using UnityEngine.EventSystems;

public class LaunchYesFunction : ButtonFunction
{
	public override string GetButtonLabel()
	{
		return "Yes";
	}

	public override void Execute()
	{
		m_spaceflightController.m_messages.text = "Opening docking bay doors...";

		// play the docking bay door open sound
		m_spaceflightController.m_basicSound.PlayOneShot( 0 );

		// open the top docking bay door
		Transform dockingBayDoorsTop = m_spaceflightController.m_map.transform.Find( "Camera/Docking Bay Doors Top" );
		Animator topAnimator = dockingBayDoorsTop.GetComponent<Animator>();
		topAnimator.Play( "Open" );

		// open the bottom docking bay door
		Transform dockingBayDoorsBottom = m_spaceflightController.m_map.transform.Find( "Camera/Docking Bay Doors Bottom" );
		Animator bottomAnimator = dockingBayDoorsBottom.GetComponent<Animator>();
		bottomAnimator.Play( "Open" );

		// play the decompression sound
		m_spaceflightController.m_basicSound.PlayOneShot( 1, 1 );

		// fire up the particle system
		Transform decompression = m_spaceflightController.m_map.transform.Find( "Camera/Decompression" );
		ParticleSystem particleSystem = decompression.GetComponent<ParticleSystem>();
		particleSystem.Play();
	}

	public override bool Update()
	{
		return true;
	}
}
