
public class ACThemselvesButton : ShipButton
{
	public override string GetLabel()
	{
		return "Themselves";
	}

	public override bool Execute()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// change the viewport label
		SpaceflightController.m_instance.m_viewport.UpdateLabel( "Ships Log - Themselves" );

		// show the ships log
		SpaceflightController.m_instance.m_shipsLog.Show( playerData.m_shipsLog.m_alienComms[ (int) PD_ShipsLog.AlienComm.Themselves ] );

		return false;
	}
}
