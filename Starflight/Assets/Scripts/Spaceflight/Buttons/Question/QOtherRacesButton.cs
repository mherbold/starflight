
public class QOtherRacesButton : ShipButton
{
	public override string GetLabel()
	{
		return "Other Races";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_encounter.AddComm( GD_Comm.Subject.OtherRaces, true );

		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
