
public class GeneralInfoButton : ShipButton
{
	public override string GetLabel()
	{
		return "General Info";
	}

	public override bool Execute()
	{
		m_spaceflightController.m_encounter.AddComm( GD_Comm.Subject.GeneralInfo, true );

		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
