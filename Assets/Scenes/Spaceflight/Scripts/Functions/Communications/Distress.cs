
using UnityEngine;
using UnityEngine.EventSystems;

public class DistressFunction : ButtonFunction
{
	public override string GetButtonLabel()
	{
		return "Distress";
	}

	public override void Execute()
	{
		if ( m_spaceflightController.m_inDockingBay )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "Transmit emergency distress call, please confirm.";
		}
	}
}
