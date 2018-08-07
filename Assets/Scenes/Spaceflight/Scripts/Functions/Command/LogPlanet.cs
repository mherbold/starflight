
using UnityEngine;
using UnityEngine.EventSystems;

public class LogPlanetFunction : ButtonFunction
{
	public override void Execute()
	{
		if ( !m_spaceflightController.m_inOrbit )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "We're not in orbit.";
		}
	}

	public override void Cancel()
	{
	}
}
