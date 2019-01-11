
public class StatementButton : ShipButton
{
	public override string GetLabel()
	{
		return "Statement";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_encounter.AddComm( GD_Comm.Subject.Statement, true );

		SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
