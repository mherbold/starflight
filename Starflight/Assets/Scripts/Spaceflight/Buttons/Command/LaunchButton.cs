
public class LaunchButton : ShipButton
{
	public override string GetLabel()
	{
		return "Launch";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_messages.Clear();

		SpaceflightController.m_instance.m_messages.AddText( "<color=yellow>Confirm launch?</color>" );

		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Launch );

		return true;
	}
}
