
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
		m_spaceflightController.RestoreBridgeButtons();

		return false;
	}
}
