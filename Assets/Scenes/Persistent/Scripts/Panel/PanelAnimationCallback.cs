
using UnityEngine;

public class PanelAnimationCallback : StateMachineBehaviour
{
	// unity calls this when the animation controller has finished transitioning from one state to another
	override public void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
	{
		// call the original function
		base.OnStateExit( animator, stateInfo, layerIndex );

		// check the name of the state we just transitioned out of
		if ( stateInfo.IsName( "Hide UI" ) )
		{
			// we left the hide ui state and we are now in the show ui state
			PanelController.m_instance.Opened();
		}
		else
		{
			// we left the show ui state and we are now in the hide ui state
			PanelController.m_instance.Closed();
		}
	}
}
