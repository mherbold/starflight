
public class PostureButton : ShipButton
{
	public override string GetLabel()
	{
		return "Posture";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Posture );

		return false;
	}
}
