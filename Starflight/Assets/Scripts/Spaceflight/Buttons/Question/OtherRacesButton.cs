
public class OtherRacesButton : ShipButton
{
	public override string GetLabel()
	{
		return "Other Races";
	}

	public override bool Execute()
	{
		var comm = m_spaceflightController.m_encounter.FindComm( GD_Comm.Subject.OtherRaces, true );

		m_spaceflightController.m_encounter.SendComm( comm );

		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
