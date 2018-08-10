
using UnityEngine;
using UnityEngine.EventSystems;

public class LogPlanetFunction : ButtonFunction
{
	public override string GetButtonLabel()
	{
		return "Log Planet";
	}

	public override void Execute()
	{
		if ( !m_spaceflightController.m_inOrbit )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "We're not in orbit.";

			m_spaceflightController.UpdateButtonSprites();
		}
	}
}
