
public class TheAncientsButton : ShipButton
{
	public override string GetLabel()
	{
		return "The Ancients";
	}

	public override bool Execute()
	{
		m_spaceflightController.m_encounter.AddComm( GD_Comm.Subject.TheAncients, true );

		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
