
using UnityEngine;
using UnityEngine.EventSystems;

public class DistressButton : Button
{
	public override string GetLabel()
	{
		return "Distress";
	}

	public override bool Execute()
	{
		if ( m_spaceflightController.m_inDockingBay )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "Transmit emergency distress call, please confirm.";

			return true;
		}

		return false;
	}
}
