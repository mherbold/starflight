
public class OldEmpireButton : ShipButton
{
	public override string GetLabel()
	{
		return "Old Empire";
	}

	public override bool Execute()
	{
		var comm = m_spaceflightController.m_encounter.FindComm( GD_Comm.Subject.OldEmpire, true );

		m_spaceflightController.m_encounter.SendComm( comm );

		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
