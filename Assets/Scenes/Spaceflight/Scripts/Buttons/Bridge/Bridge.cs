
using UnityEngine;
using UnityEngine.EventSystems;

public class BridgeButton : Button
{
	public override string GetLabel()
	{
		return "Bridge";
	}

	public override bool Execute()
	{
		// restore the bridge buttons
		m_spaceflightController.RestoreBridgeButtons();

		return false;
	}
}
