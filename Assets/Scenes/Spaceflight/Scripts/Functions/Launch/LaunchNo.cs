
using UnityEngine;
using UnityEngine.EventSystems;

public class LaunchNoFunction : ButtonFunction
{
	public override string GetButtonLabel()
	{
		return "No";
	}

	public override bool Execute()
	{
		m_spaceflightController.RestoreBridgeButtons();

		return false;
	}
}
