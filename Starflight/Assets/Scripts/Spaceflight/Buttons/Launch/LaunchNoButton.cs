
public class LaunchNoButton : ShipButton
{
	public override string GetLabel()
	{
		return "No";
	}

	public override bool Execute()
	{
		// restore the bridge buttons
		SpaceflightController.m_instance.m_buttonController.RestoreBridgeButtons();

		return false;
	}
}
