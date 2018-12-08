
public class StatementButton : ShipButton
{
	public override string GetLabel()
	{
		return "Statement";
	}

	public override bool Execute()
	{
		m_spaceflightController.m_encounter.AddComm( GD_Comm.Subject.Statement, true );

		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
