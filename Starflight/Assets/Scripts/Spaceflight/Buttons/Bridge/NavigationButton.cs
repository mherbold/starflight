
public class NavigationButton : ShipButton
{
	public override string GetLabel()
	{
		return "Navigation";
	}

	public override bool Execute()
	{
		// change the buttons
		SpaceflightController.m_instance.m_buttonController.SetManeuverButtons();

		return true;
	}
}
