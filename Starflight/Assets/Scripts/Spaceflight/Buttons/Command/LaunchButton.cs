
public class LaunchButton : ShipButton
{
	public override string GetLabel()
	{
		return "Launch";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_general.m_location == PD_General.Location.DockingBay )
		{
			SpaceflightController.m_instance.m_messages.ChangeText( "<color=yellow>Confirm launch?</color>" );

			SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Launch );

			return true;
		}

		return false;
	}
}
