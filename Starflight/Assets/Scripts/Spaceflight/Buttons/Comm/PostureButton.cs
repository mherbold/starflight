
public class PostureButton : ShipButton
{
	public override string GetLabel()
	{
		return "Posture";
	}

	public override bool Execute()
	{
		// change the buttons
		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Posture );

		return false;
	}
}
