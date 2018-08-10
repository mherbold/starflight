
using UnityEngine;
using UnityEngine.EventSystems;

public class StarmapFunction : ButtonFunction
{
	public override string GetButtonLabel()
	{
		return "Starmap";
	}

	public override void Execute()
	{
		if ( !m_spaceflightController.m_inHyperspace )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "We're not in hyperspace, captain.";

			m_spaceflightController.UpdateButtonSprites();
		}
	}
}
