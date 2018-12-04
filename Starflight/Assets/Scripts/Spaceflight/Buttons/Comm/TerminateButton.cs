
public class TerminateButton : ShipButton
{
	public override string GetLabel()
	{
		return "Terminate";
	}

	public override bool Execute()
	{
		var comm = m_spaceflightController.m_encounter.FindComm( GD_Comm.Subject.Terminate, true );

		m_spaceflightController.m_encounter.SendComm( comm );

		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
