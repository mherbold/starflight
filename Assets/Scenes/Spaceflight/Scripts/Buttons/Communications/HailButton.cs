
using UnityEngine;
using UnityEngine.EventSystems;

public class HailButton : Button
{
	public override string GetLabel()
	{
		return "Hail";
	}

	public override bool Execute()
	{
		if ( m_spaceflightController.m_inDockingBay )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "There's no one to hail.";

			m_spaceflightController.m_buttonController.UpdateButtonSprites();

			return false;
		}

		return false;
	}
}
