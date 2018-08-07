
using UnityEngine;
using UnityEngine.EventSystems;

public class BridgeFunction : ButtonFunction
{
	public override void Execute()
	{
		m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Deactivate );

		m_spaceflightController.RestoreBridgeButtons();
	}

	public override void Cancel()
	{
	}
}
