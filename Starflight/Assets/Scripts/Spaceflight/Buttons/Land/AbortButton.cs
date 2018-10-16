
public class AbortButton : ShipButton
{
	public override string GetLabel()
	{
		return "Abort";
	}

	public override bool Execute()
	{
		// restore the bridge buttons
		m_spaceflightController.m_buttonController.RestoreBridgeButtons();

		return false;
	}
}
