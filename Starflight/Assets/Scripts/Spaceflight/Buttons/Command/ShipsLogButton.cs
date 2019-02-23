
public class ShipsLogButton : ShipButton
{
	public override string GetLabel()
	{
		return "Ships Log";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.ShipsLog );

		return false;
	}
}
