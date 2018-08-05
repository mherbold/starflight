
using UnityEngine;

public class PanelAnimationCallback : StateMachineBehaviour
{
/*
	// unity calls this when the animation controller is starting to transitioning from one state to another
	public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
	{
		// call the original function
		base.OnStateEnter( animator, stateInfo, layerIndex );

		// check the name of the state we are transitioning into
		if ( stateInfo.IsName( "Show UI" ) )
		{
			// we left the hide ui state and so we should play the compressed air sound
			animator.gameObject.GetComponent<BasicSound>().PlayOneShot( 0 );
		}
		else if ( stateInfo.IsName( "Hide UI" ) )
		{
			// we left the show ui state and so we should play the air escaping sound
			animator.gameObject.GetComponent<BasicSound>().PlayOneShot( 1 );
		}
	}
*/
	// unity calls this when the animation controller has finished transitioning from one state to another
	override public void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
	{
		// call the original function
		base.OnStateExit( animator, stateInfo, layerIndex );

		// check the name of the state we just transitioned out of
		if ( stateInfo.IsName( "Hide UI" ) )
		{
			// we left the hide ui state and we are now in the show ui state
			animator.gameObject.GetComponent<StarportControllerReference>().m_starportController.FinishOpeningUI();
		}
		else
		{
			// we left the show ui state and we are now in the hide ui state
			animator.gameObject.GetComponent<StarportControllerReference>().m_starportController.FinishClosingUI();
		}
	}
}
