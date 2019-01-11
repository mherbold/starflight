
public class RespondButton : ShipButton
{
	public override string GetLabel()
	{
		return "Respond";
	}

	public override bool Execute()
	{
		// change the buttons
		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Respond );

		return true;
	}
}
