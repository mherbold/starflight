
using UnityEngine;
using UnityEngine.EventSystems;

public class BridgeFunction : ButtonFunction
{
	public override string GetButtonLabel()
	{
		return "Bridge";
	}

	public override void Execute()
	{
		// restore the bridge buttons
		m_spaceflightController.RestoreBridgeButtons();
	}
}
