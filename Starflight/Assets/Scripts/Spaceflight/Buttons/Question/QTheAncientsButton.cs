
public class QTheAncientsButton : ShipButton
{
	public override string GetLabel()
	{
		return "The Ancients";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_encounter.AddComm( GD_Comm.Subject.TheAncients, true );

		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
