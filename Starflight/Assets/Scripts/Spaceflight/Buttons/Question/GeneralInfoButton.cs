
public class GeneralInfoButton : ShipButton
{
	public override string GetLabel()
	{
		return "General Info";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_encounter.AddComm( GD_Comm.Subject.GeneralInfo, true );

		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
