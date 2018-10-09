
public class BridgeButton : ShipButton
{
	public override string GetLabel()
	{
		return "Bridge";
	}

	public override bool Execute()
	{
		// restore the bridge buttons
		m_spaceflightController.m_buttonController.RestoreBridgeButtons();

		return false;
	}
}
