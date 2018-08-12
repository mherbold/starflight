
using UnityEngine;
using UnityEngine.EventSystems;

public class LaunchNoButton : Button
{
	public override string GetLabel()
	{
		return "No";
	}

	public override bool Execute()
	{
		m_spaceflightController.m_buttonController.RestoreBridgeButtons();

		return false;
	}
}
