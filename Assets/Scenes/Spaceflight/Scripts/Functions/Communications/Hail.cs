
using UnityEngine;
using UnityEngine.EventSystems;

public class HailFunction : ButtonFunction
{
	public override string GetButtonLabel()
	{
		return "Hail";
	}

	public override void Execute()
	{
		if ( m_spaceflightController.m_inDockingBay )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "There's no one to hail.";

			m_spaceflightController.UpdateButtonSprites();
		}
	}
}
