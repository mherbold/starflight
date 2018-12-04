
public class RespondButton : ShipButton
{
	public override string GetLabel()
	{
		return "Respond";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Respond );

		return true;
	}
}
