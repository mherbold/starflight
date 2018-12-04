
public class StatementButton : ShipButton
{
	public override string GetLabel()
	{
		return "Statement";
	}

	public override bool Execute()
	{
		var comm = m_spaceflightController.m_encounter.FindComm( GD_Comm.Subject.Statement, true );

		m_spaceflightController.m_encounter.SendComm( comm );

		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
