
public class QOldEmpireButton : ShipButton
{
	public override string GetLabel()
	{
		return "Old Empire";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_encounter.AddComm( GD_Comm.Subject.OldEmpire, true );

		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		return false;
	}
}
