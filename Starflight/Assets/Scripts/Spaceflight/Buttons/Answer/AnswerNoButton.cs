
public class AnswerNoButton : ShipButton
{
	public override string GetLabel()
	{
		return "No";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_encounter.AddComm( GD_Comm.Subject.No, true );

		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
