
public class TerminateButton : ShipButton
{
	public override string GetLabel()
	{
		return "Terminate";
	}

	public override bool Execute()
	{
		m_spaceflightController.m_encounter.AddComm( GD_Comm.Subject.Terminate, true );

		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
