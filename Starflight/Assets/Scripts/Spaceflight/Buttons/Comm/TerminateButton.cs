
public class TerminateButton : ShipButton
{
	public override string GetLabel()
	{
		return "Terminate";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_encounter.AddComm( GD_Comm.Subject.Terminate, true );

		SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
