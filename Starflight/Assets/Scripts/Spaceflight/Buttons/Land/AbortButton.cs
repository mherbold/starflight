
public class AbortButton : ShipButton
{
	public override string GetLabel()
	{
		return "Abort";
	}

	public override bool Execute()
	{
		// restore the bridge buttons
		SpaceflightController.m_instance.m_buttonController.SetBridgeButtons();

		return false;
	}
}
