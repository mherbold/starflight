
public class LaunchNoButton : ShipButton
{
	public override string GetLabel()
	{
		return "No";
	}

	public override bool Execute()
	{
		// restore the bridge buttons
		m_spaceflightController.m_buttonController.RestoreBridgeButtons();

		return false;
	}
}
