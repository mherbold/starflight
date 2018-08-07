
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandFunction : ShipFunction
{
	public override void Execute()
	{
		m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Activate );
	}

	public override void Cancel()
	{
	}
}
