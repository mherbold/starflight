
public class ThemselvesButton : ShipButton
{
	public override string GetLabel()
	{
		return "Themselves";
	}

	public override bool Execute()
	{
		m_spaceflightController.m_encounter.AddComm( GD_Comm.Subject.Themselves, true );

		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
