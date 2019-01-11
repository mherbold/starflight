
public class BridgeButton : ShipButton
{
	public override string GetLabel()
	{
		return "Bridge";
	}

	public override bool Execute()
	{
		// restore the bridge buttons
		SpaceflightController.m_instance.m_buttonController.RestoreBridgeButtons();

		return false;
	}
}
